using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection; // Assembly
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TimeCalc
{
    public partial class FrmTimeCalc : Form
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
        const int VK_MULTIPLY = 0x6A; // const int VK_OEM_2 = 0xBF; // #
        const uint KEYEVENTF_KEYUP = 0x0002;

        private const string decText = "Gesamtsumme als Dezimalzahl: ";
        //private readonly string oldText = string.Empty;
        private bool nothingToSave = true;
        private bool numPadDecimal = false;
        //private bool isFileLoading = false;
        private readonly IFormatProvider deCulture = new CultureInfo("de-DE", true);
        private readonly string winTitle = Application.ProductName; // "TimeCalc";
        private readonly Version curVersion = Assembly.GetExecutingAssembly().GetName().Version;
        //private ClsUtilities.DateDiff ddfSum = new();
        private int numRows = 9; // IntDigits(numRows + 1) != IntDigits(e.RowIndex + 1)
        private string currFile = string.Empty;
        //private StringFormat strFormat; // Used to format the grid rows.
        private readonly ArrayList arrColumnLefts = new(); // Used to save left coordinates of columns
        private readonly ArrayList arrColumnWidths = new(); // Used to save column widths
        private int iCellHeight = 0; // Used to get/set the datagridview cell height
        private int iTotalWidth = 0; //
        private int intRow = 0; // Used as counter
        private bool bFirstPage = false; // Used to check whether we are printing first page
        private bool bNewPage = false; // Used to check whether we are printing a new page
        private int iHeaderHeight = 0; // Used for the header height
        private bool isInEditMode;
        private readonly Regex rgxImportDate = new(@"([012]?[0-9]|3[01])\.(0?[1-9]|1[012])\.(\d){2,4}", RegexOptions.Compiled);
        private readonly Regex rgxImportTime = new(@"([01]?[0-9]|2[0-3]):([0-5]?[0-9])", RegexOptions.Compiled);
        private readonly Regex rgxValidTime = new(@"^\d{1,2}[\:\s\w]\d{2}$", RegexOptions.Compiled);
        readonly string dateFormat = "dd.MM.yyyy";
        readonly string timeFormat = "HH:mm";
        readonly string defaultCellStyleFontName;
        readonly int rowHeadersWidth;
        private readonly int ci_0Datum = 0;                 // ColumnIndex
        private readonly int ci_1Von = 1;                   // ColumnIndex
        private readonly int ci_2Bis = 2;                   // ColumnIndex
        private readonly int[] ci_12VonBis = { 1, 2 };      // von, bis, 0 = Datum
        private readonly int[] ci_123TimeAny = { 1, 2, 3 }; // von, bis, Pause
        private readonly int ci_3Pause = 3;                 // ColumnIndex
        private readonly int ci_4Faktor = 4;                // ColumnIndex
        private readonly List<int> ci_0123DateTimeAny = new() { 0, 1, 2, 3 };  // Index der Felder, in denen Datumsangaben möglich sind (z.B. mit "F5")
        private DataTable dtDays = new("Days");

        public FrmTimeCalc()
        {
            InitializeComponent();
            dGV.Columns[5].DefaultCellStyle.BackColor = dGV.Columns[6].DefaultCellStyle.BackColor = System.Drawing.Color.AntiqueWhite;
            dGV.Columns[5].DefaultCellStyle.Alignment = dGV.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            winTitle = winTitle + " " + new Regex(@"^\d+\.\d+").Match(curVersion.ToString()).Value;
            Text = "Unbenannt - " + winTitle;
            dGV.TopLeftHeaderCell.ToolTipText = "Tabelle";
            dGV.Columns[0].ToolTipText = "Drücken Sie <F5>, um das aktuelle Datum einzugeben.\nSie können den Wert mit der Minus- oder Plustaste ändern.";
            dGV.Columns[1].ToolTipText = "Anfangsuhrzeit";
            dGV.Columns[2].ToolTipText = "Enduhrzeit";
            dGV.Columns[3].ToolTipText = "Pausenstunden";
            dGV.Columns[4].ToolTipText = "Mit Hilfe des Multiplikators können mehrere Perso-\nnen gleichzeitig bei der Zeiterfassung berücksichtigt\nwerden. Wenn der Multiplikator '0' beträgt, wird die\nZeile von der Saldorechnung ausgeschlossen.";
            dGV.Columns[5].ToolTipText = "Zeitraum von Anfang- bis Enddatum";
            dGV.Columns[6].ToolTipText = "Aufsummierung der Zeiten";

            dGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            defaultCellStyleFontName = dGV.DefaultCellStyle.Font.Name;
            rowHeadersWidth = dGV.RowHeadersWidth;
        }

        private void DGV_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dGrid = sender as DataGridView;
            string rowText = (e.RowIndex + 1).ToString() + ". ";
            if (dGV.Rows[e.RowIndex].IsNewRow && IntDigits(numRows + 1) != IntDigits(e.RowIndex + 1))
            {// nur wenn sich die Anzahl der Stellen ändert
                numRows = e.RowIndex;
                SizeF Sz = e.Graphics.MeasureString(rowText, e.InheritedRowStyle.Font, 0, new StringFormat(StringFormatFlags.MeasureTrailingSpaces));
                dGrid.RowHeadersWidth = (int)Sz.Width + 25; // 25
            }
            var centerFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces) // default: exclude the space at the end of each line
            {
                Alignment = StringAlignment.Far, // Bei einem Layout mit Ausrichtung von links nach rechts ist die weit entfernte Position rechts.
                LineAlignment = StringAlignment.Center // vertikale Ausrichtung der Zeichenfolge 
            };
            Rectangle headerBounds = new(e.RowBounds.Left, e.RowBounds.Top, dGrid.RowHeadersWidth, e.RowBounds.Height);
            using SolidBrush sBrush = new(dGrid.RowHeadersDefaultCellStyle.ForeColor);
            // the using statement automatically disposes the brush
            e.Graphics.DrawString(rowText, e.InheritedRowStyle.Font, sBrush, headerBounds, centerFormat);
            //if (e.RowIndex % 2 != 0)
            //{// AlternatingCellStyle
            //    dGrid.Rows[e.RowIndex].Cells[0].Style.BackColor = dGrid.Rows[e.RowIndex].Cells[1].Style.BackColor = dGrid.Rows[e.RowIndex].Cells[2].Style.BackColor = dGrid.Rows[e.RowIndex].Cells[3].Style.BackColor = Color.WhiteSmoke;
            ////    if (dGV.Rows[e.RowIndex].Cells[dGV.CurrentCell.ColumnIndex].IsInEditMode)
            //}
        }

        public static int IntDigits(int number)
        {// number = Math.Abs(number); // if negative
            int length = 1;
            while ((number /= 10) >= 1) { length++; }
            return length;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.D | Keys.Control:
                    {
                        AddShortcutToDesktop();
                        return true;
                    }
                case Keys.P | Keys.Control:
                    {
                        ShowPrintDialog();
                        return true;
                    }
                case Keys.E | Keys.Control:
                    {
                        ToolStripMenuItemExcel_Click(null, null);
                        return true;
                    }
                case Keys.P | Keys.Shift | Keys.Control:
                    {
                        ToolStripMenuItemPrintPreview_Click(null, null); // printPreviewDialog.ShowDialog();
                        return true;
                    }
                case Keys.I | Keys.Control:
                    {
                        ImportToolStripMenuItem_Click(null, null);
                        return true;
                    }
                case Keys.W | Keys.Control:
                    {
                        WebseiteToolStripMenuItem_Click(null, null);
                        return true;
                    }
                case Keys.F5:
                    if (dGV.CurrentCell != null && dGV.CurrentCell.ColumnIndex < dGV.ColumnCount - 2 && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {
                        UhrzeitDatumToolStripMenuItem_Click(null, null);
                        return true;
                    }
                    else break;
                case Keys.V | Keys.Control:
                    if (ActiveControl == dGV && dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex))
                    {// EditMode ist ausgeschlossen => ActiveControl == null!
                        PasteToolStripMenuItem_Click(null, null);
                        return true;
                    }
                    else break;
                case Keys.F10 | Keys.Shift:
                case Keys.Apps:
                    if (ActiveControl == dGV && dGV.CurrentCell != null)
                    {// EditMode ist ausgeschlossen => ActiveControl == null!
                        DataGridViewCell ccell = dGV.CurrentCell;
                        Rectangle r = ccell.DataGridView.GetCellDisplayRectangle(ccell.ColumnIndex, ccell.RowIndex, false);
                        Point p = new(r.X + r.Width / 2, r.Y + r.Height / 2);
                        ShowContextMenu(ccell, ccell.DataGridView, p);
                        return true;
                    }
                    else break;
                case Keys.Add:
                case Keys.Oemplus:
                    if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex) && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {// nicht in letzten beiden Splaten
                        ChangeCellValueByKey(false, true); // Shift, Add
                        return true;
                    }
                    else break;
                case Keys.Subtract:
                case Keys.OemMinus:
                    if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex) && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {// nicht in letzten beiden Splaten
                        ChangeCellValueByKey(false, false); // Shift, Add
                        return true;
                    }
                    else break;
                case Keys.Add | Keys.Shift:
                case Keys.Oemplus | Keys.Shift:
                    if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex) && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {// nicht in letzten beiden Splaten
                        ChangeCellValueByKey(true, true); // Shift, Add
                        return true;
                    }
                    else break;
                case Keys.Subtract | Keys.Shift:
                case Keys.OemMinus | Keys.Shift:
                    if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex) && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {// nicht in letzten beiden Splaten
                        ChangeCellValueByKey(true, false); // Shift, Add
                        return true;
                    }
                    else break;
                case Keys.Return: // case Keys.Enter:
                case Keys.Tab:
                    if (dGV.CurrentCell != null && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {
                        EnterToolStripMenuItem_Click(null, null);
                        return true;
                    }
                    else break;
                case Keys.Delete:
                    if (ActiveControl == dGV && dGV.CurrentCell != null)
                    {// EditMode ist ausgeschlossen => ActiveControl == null!
                        if (dGV.SelectedRows.Count > 0) { DeleteRowToolStripMenuItem_Click(null, null); }
                        else { LöschenToolStripMenuItem_Click(null, null); }
                        return true;
                    }
                    else break; // Delete-Taste hat eine Funktion in EditMode!
                case Keys.Delete | Keys.Shift:
                    if (dGV.CurrentCell != null && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {
                        DeleteRowToolStripMenuItem_Click(null, null);
                    }
                    return true;
                case Keys.Delete | Keys.Control | Keys.Shift:
                    if (dGV.CurrentCell != null && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {
                        AllesLöschenContextMenuItem_Click(null, null);
                    }
                    return true;
                case Keys.Insert | Keys.Shift:
                    if (dGV.CurrentCell != null && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
                    {
                        InsertRowToolStripMenuItem_Click(null, null);
                    }
                    return true;
                case Keys.Tab | Keys.Control:
                case Keys.S | Keys.Control:
                    SpeichernToolStripMenuItem_Click(null, null);
                    return true;
                case Keys.S | Keys.Shift | Keys.Control:
                    SpeichernUnterToolStripMenuItem_Click(null, null);
                    return true;
                //case Keys.I | Keys.Control:
                //    infoToolStripMenuItem_Click(null, null);
                //    return true;
                case Keys.F1:
                    HelpMessageBoxShow();
                    return true;
                case Keys.Escape:
                    if (dGV.CurrentCell != null && dGV.IsCurrentCellInEditMode)
                    {
                        dGV.EndEdit();
                        dGV.CurrentCell.Selected = true;
                    }
                    else { Close(); }
                    return true;
            }// switch (keyData)
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeCellValueByKey(bool shiftMode, bool addMode)
        {
            DateTime dt; // = new DateTime();
            if (dGV.IsCurrentCellInEditMode)
            {
                string tbText = ((TextBox)dGV.EditingControl).Text;
                string dtFormat = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? timeFormat : dateFormat;
                if (ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex)) { dt = ClsUtilities.NormalizeTime(tbText.Length == 0 ? DateTime.Now.ToString(dtFormat) : tbText); }
                else if (dGV.CurrentCell.ColumnIndex == ci_3Pause) { dt = ClsUtilities.NormalizePause(tbText.Length == 0 ? DateTime.Today.ToString(dtFormat) : tbText); }
                else { dt = ClsUtilities.NormalizeDate(tbText.Length == 0 ? DateTime.Now.ToString(dtFormat) : tbText); }
                if (dt != DateTime.MinValue) { dGV.CurrentCell.Value = dt.ToString(dtFormat); }
                else { dGV.CurrentCell.Value = null; }
            }
            else { dGV.BeginEdit(true); }
            try
            {
                object cVal = dGV.CurrentCell.Value;
                {
                    string dtFormat = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? timeFormat : dateFormat; // letzteres für "Datum"
                    if (dGV.CurrentCell.ColumnIndex == ci_3Pause)
                    {
                        dt = DateTime.ParseExact(cVal == null || cVal.ToString().Length == 0 ? DateTime.Today.ToString(dtFormat) : cVal.ToString(), dtFormat, deCulture);
                    }
                    else
                    {
                        dt = DateTime.ParseExact(cVal == null || cVal.ToString().Length == 0 ? DateTime.Now.ToString(dtFormat) : cVal.ToString(), dtFormat, deCulture);
                    }
                    if (shiftMode)
                    {
                        if (addMode) { dt = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? dt.AddHours(1) : dt.AddMonths(1); }
                        else { dt = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? dt.AddHours(-1) : dt.AddMonths(-1); }
                    }
                    else
                    {
                        if (addMode) { dt = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? dt.AddMinutes(1) : dt.AddDays(1); }
                        else { dt = ci_123TimeAny.Contains(dGV.CurrentCell.ColumnIndex) ? dt.AddMinutes(-1) : dt.AddDays(-1); }
                    }
                    dGV.CurrentCell.Value = dt.ToString(dtFormat);
                }
                if (dGV.IsCurrentCellInEditMode)
                {// neuer Wert wird nicht automatisch angezeigt! Code wird nur ausgeführt, wenn das parsen funktioniert hat.
                    ((TextBox)dGV.EditingControl).Text = dGV.CurrentCell.Value.ToString();
                    ((TextBox)dGV.EditingControl).SelectAll();
                }
            }
            catch { Console.Beep(); }
        }

        private void FrmTimeCalc_Load(object sender, EventArgs e)
        {// Width = 444 //MessageBox.Show(dGV.Columns[2].Width.ToString()); // => primär 105
            //MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            string[] args = Environment.GetCommandLineArgs(); // The first element in the array contains the file name of the executing program. If the file name is not available, the first element is equal to string.Empty. 
            if (args.Length > 1 && args[1].Length > 0 && File.Exists(args[1])) // wenn args.Length == 2 dann existieren args[0] und args[1]
            {
                //isFileLoading = true;
                ReadTextFile(args[1]);
                Text = currFile + " - " + winTitle; // Text = Path.GetFileName(currFile) + " - " + winTitle;
                if (dGV.Rows[0].Cells[0].Value != null)
                {
                    //if (rgxValidDate.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success) { UpdateLastDateColumns(); }
                    if (rgxValidTime.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success) { UpdateLastTimeColumns(); }
                }
            }
            //string cmdPath = string.Empty;
            //for (int i = 1; i < args.Length; i++)
            //{
            //    cmdPath = string.Concat(cmdPath, " ", args[i]).TrimStart(); // Pfad mit Leerzeichen ohne Anführungsstriche
            //    //MessageBox.Show(i + ": " + cmdPath);
            //    if (cmdPath.Length > 0 && File.Exists(cmdPath))
            //    {
            //        isFileLoading = true;
            //        ReadTextFile(cmdPath);
            //        Text = currFile + " - " + winTitle; // Text = Path.GetFileName(currFile) + " - " + winTitle;
            //        if (dGV.Rows[0].Cells[0].Value != null)
            //        {
            //            if (rgxValidDate.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success)
            //            { UpdateLastDateColumns(); }
            //            else if (rgxValidTime.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success)
            //            { modusStunden = true; UpdateLastTimeColumns(); } // modusStunden = true;
            //        }
            //        break;
            //    }
            //}
            dGV.FirstDisplayedScrollingRowIndex = dGV.RowCount - 1;  //dGV.BeginEdit(true); <= besser NICHT (Cursor fehlt)
            //isFileLoading = false;
        }

        private void DGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            dGV.Tag = dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value; //Save old value to datagridview.Tag
            dGV.CurrentCell.Style.ForeColor = System.Drawing.Color.Empty;
            dGV.ClearSelection(); //  dGV.CurrentCell.Selected = true;
            toolStripButtonComplete.Enabled = true;
            isInEditMode = true;
            //dGV.AllowUserToAddRows = false;
        }

        private void DGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {// Event ist nicht in den letzten beiden Spalten auslösbar, da die diese READONLY sind. => CellLeave-Ereignis
            CalculateSaldo(dGV.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            if (dGV.Rows[e.RowIndex].Cells[6].Value != null && dGV.Rows[e.RowIndex].Cells[6].Value.ToString().Contains('-'))
            {
                for (int i = 1; i < 3; i++) { dGV.Rows[e.RowIndex].Cells[i].Value = null; }
            }


            string oldString = dGV.Tag == null ? "" : dGV.Tag.ToString();
            string newString = dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null ? "" : dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (nothingToSave && newString != oldString)
            {
                ShowCellChangeInTitle();
            }
            toolStripButtonComplete.Enabled = false;
            isInEditMode = false;
            //if (dGV.Rows[e.RowIndex].Cells[6].Value != null) { dGV.AllowUserToAddRows = true; }
            //else { dGV.AllowUserToAddRows = false; }
        }

        private void DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {// to detect changes in the DataGridView; CurrentCellDirtyStateChanged to checkbox type columns
            if (!isInEditMode && nothingToSave && ci_0123DateTimeAny.Contains(e.ColumnIndex) && e.RowIndex > -1) // bei Programmstart tritt das Ereignis 3x ein => e.RowIndex -1
            {
                ShowCellChangeInTitle();
            }
        }

        private void ShowCellChangeInTitle()
        {
            Text = Regex.Replace(Text, @" - " + winTitle + "$", "* - " + winTitle);
            nothingToSave = false;
        }

        private void CalculateSaldo(DataGridViewCell myCell)
        {// aktualisiert u.a. Spalte 'Saldo'
            DataGridViewCell ccell0 = dGV.Rows[myCell.RowIndex].Cells[0];
            DataGridViewCell ccell1 = dGV.Rows[myCell.RowIndex].Cells[1];
            DataGridViewCell ccell2 = dGV.Rows[myCell.RowIndex].Cells[2];
            DataGridViewCell ccell3 = dGV.Rows[myCell.RowIndex].Cells[3];

            if (myCell.ColumnIndex == ci_1Von && ccell1.Value != null)
            {// 1. Zelle normalisieren
                DateTime date1 = ClsUtilities.NormalizeTime(ccell1.Value.ToString());
                if (date1 != DateTime.MinValue) { ccell1.Value = date1.ToString(timeFormat); }
                else { TidyUpOnError(ccell1); }
            }
            else if (myCell.ColumnIndex == ci_2Bis && ccell2.Value != null)
            {// 2. Zelle normalisieren
                DateTime date2 = ClsUtilities.NormalizeTime(ccell2.Value.ToString());
                if (date2 != DateTime.MinValue) { ccell2.Value = date2.ToString(timeFormat); }
                else { TidyUpOnError(ccell2); }
            }
            else if (myCell.ColumnIndex == ci_3Pause && ccell3.Value != null)
            {// 3. Zelle normalisieren
                DateTime date3 = ClsUtilities.NormalizePause(ccell3.Value.ToString());
                if (date3 != DateTime.MinValue) { ccell3.Value = date3.ToString(timeFormat); }
                else { TidyUpOnError(ccell3); }
            }
            else if (myCell.ColumnIndex == ci_0Datum && ccell0.Value != null)
            {// 0. Zelle normalisieren
                DateTime date0 = ClsUtilities.NormalizeDate(ccell0.Value.ToString());
                if (date0 != DateTime.MinValue) { ccell0.Value = date0.ToString(dateFormat); }
                else { TidyUpOnError(ccell0); }
            }


            UpdateLastTimeColumns();
        }

        private void UpdateLastTimeColumns()
        {// Spalte 'Saldo' und 'Spanne' werden komplett aktualisiert!
            TimeSpan tsCell7 = TimeSpan.Zero, tsCell6; string foo; TimeSpan pause; // = TimeSpan.Zero;
            for (int row = 0; row < dGV.RowCount - 1; ++row)
            {
                DataGridViewCell dgvCell1 = dGV.Rows[row].Cells[1]; // von
                DataGridViewCell dgvCell2 = dGV.Rows[row].Cells[2]; // bis
                DataGridViewCell dgvCell3 = dGV.Rows[row].Cells[3]; // Pause
                DataGridViewCell dgvCell4 = dGV.Rows[row].Cells[4]; // Faktor
                DataGridViewCell dgvCell5 = dGV.Rows[row].Cells[5]; // Spanne

                if (dgvCell1.Value != null && dgvCell2.Value != null && dgvCell4.Value != null && rgxValidTime.Match(dgvCell1.Value.ToString()).Success
                    && rgxValidTime.Match(dgvCell2.Value.ToString()).Success && Int32.TryParse(dgvCell4.Value.ToString(), out int rowFactor))
                {// DateTime d1 = DateTime.MinValue; DateTime d2 = DateTime.MinValue; DateTime d3 = DateTime.MinValue;
                    DateTime d1, d2;
                    try
                    {// prüfen ob beide Zellen verwertbar sind
                        d1 = DateTime.ParseExact(dgvCell1.Value.ToString(), timeFormat, deCulture);
                        d2 = DateTime.ParseExact(dgvCell2.Value.ToString(), timeFormat, deCulture);
                        //d3 = DateTime.ParseExact(dgvCell3.Value.ToString(), timeFormat, deCulture);
                        if (d1 != DateTime.MinValue && d2 != DateTime.MinValue)
                        {
                            pause = DateTime.ParseExact(dgvCell3.Value.ToString(), timeFormat, deCulture).TimeOfDay;
                            if (DateTime.Compare(d2, d1) < 0) { d2 = d2.AddDays(1); } // Arbeitszeit über Nacht
                            TimeSpan ts = (d2 - d1).Subtract(pause);
                            dgvCell5.Value = string.Format("{0:00}:{1:00}", ts.Days * 24 + ts.Hours, ts.Minutes); // 24:00 ist keine gültige Zeitangabe! => hours = ts.Days * 24 + ts.Hours
                        }
                        else { dgvCell5.Value = null; }
                        int myMinute = 0; // 24:00 ist keine gültige Zeit! Deshalb wird ParseExact umgangen!
                        foo = dgvCell5.Value != null ? dgvCell5.Value.ToString() : string.Empty;
                        if (foo.Equals("24:00")) { foo = "23:59"; myMinute = 1; }
                        try { tsCell6 = foo.Length > 0 ? DateTime.ParseExact(foo, timeFormat, deCulture).TimeOfDay : TimeSpan.Zero; }
                        catch (FormatException) { tsCell6 = TimeSpan.Zero; }
                        //if (tsCell6 != TimeSpan.Zero) // && rowFactor > 0
                        //{
                        tsCell6 = tsCell6.Add(TimeSpan.FromMinutes(myMinute));
                        tsCell7 += TimeSpan.FromTicks(tsCell6.Ticks * rowFactor);

                        if (tsCell7 <= TimeSpan.Zero) // z.B. von '2' bis '2' Pause '2' ergäbe Saldo '00:-02'
                        {
                            TidyUpOnError(dgvCell2);
                            return;
                        }

                        dGV.Rows[row].Cells[6].Value = Math.Abs((int)tsCell7.TotalHours).ToString("00") + ":" + Math.Abs(tsCell7.Minutes).ToString("00");
                        //}
                        //else { dGV.Rows[row].Cells[6].Value = null; }
                    }
                    catch { dGV.Rows[row].Cells[5].Value = null; dGV.Rows[row].Cells[6].Value = null; }
                }
                else { dGV.Rows[row].Cells[5].Value = null; dGV.Rows[row].Cells[6].Value = null; }
            }
            if (tsCell7.TotalHours != 0)
            {
                linkLblDecText.Text = decText + tsCell7.TotalHours.ToString("#0.00");
                linkLblDecText.ToolTipText = "Ergebnis in die Zwischenablage kopieren";
                linkLblDecText.IsLink = true;
            }
            else
            {
                linkLblDecText.Text = string.Empty;
                linkLblDecText.ToolTipText = string.Empty;
                linkLblDecText.IsLink = false;
            }
            //if (!oldText.Equals(string.Empty) && !oldText.Equals(linkLblDecText.Text)) { nothingToSave = false; }
            //oldText = linkLblDecText.Text;
        }

        //    private void UpdateLastDateColumns()
        //    {// Spalte 'Saldo' und 'Spanne' werden komplett aktualisiert!
        //        int intCell7 = 0, intCell6 = 0;
        //        DateTime d1, d2; // = DateTime.MinValue; //DateTime d1 = new DateTime(); DateTime d2 = new DateTime();
        //        ddfSum.years = 0; ddfSum.months = 0; ddfSum.days = 0;
        //        for (int row = 0; row < dGV.RowCount - 1; ++row)
        //        {
        //            DataGridViewCell dgvCell1 = dGV.Rows[row].Cells[0];
        //            DataGridViewCell dgvCell2 = dGV.Rows[row].Cells[1];
        //            DataGridViewCell dgvCell3 = dGV.Rows[row].Cells[2]; // Pause
        //            DataGridViewCell dgvCell4 = dGV.Rows[row].Cells[3];
        //            DataGridViewCell dgvCell6 = dGV.Rows[row].Cells[5];
        //            if (dgvCell1.Value != null && dgvCell2.Value != null && dgvCell4.Value != null && rgxValidDate.Match(dgvCell1.Value.ToString()).Success
        //                && rgxValidDate.Match(dgvCell2.Value.ToString()).Success && int.TryParse(dgvCell4.Value.ToString(), out int rowFactor))
        //            {
        //                try
        //                {
        //                    d1 = DateTime.ParseExact(dgvCell1.Value.ToString(), dateFormat, deCulture);
        //                    d2 = DateTime.ParseExact(dgvCell2.Value.ToString(), dateFormat, deCulture);
        //                    ClsUtilities.DateDiff ddf = ClsUtilities.CalcDateDiff(d1, d2);
        //                    if (d1 != DateTime.MinValue && d2 != DateTime.MinValue)
        //                    {
        //                        TimeSpan ts = d2 - d1; // if (Convert.ToInt32(dGV.Rows[i].Cells[4].Value) != ts.Days) // ((int)dGV.Rows[i].Cells[4].Value funkt nicht
        //                        if (dgvCell3 != null && dgvCell3.Value != null && int.TryParse(dgvCell3.Value.ToString(), out int pause))
        //                        {// MessageBox.Show(pause.ToString());
        //                            ts = ts.Subtract(TimeSpan.FromDays(pause));
        //                        }
        //                        dgvCell6.Value = ts.Days; // MessageBox.Show(i.ToString() + ".: " + ddfSum.years.ToString() + " | " + ddfSum.months.ToString() + " | " + ddfSum.days.ToString());
        //                        if (rowFactor != 0) // if (ts != TimeSpan.Zero && rowFactor != 0)
        //                        {
        //                            ddfSum.years += ddf.years;
        //                            ddfSum.months += ddf.months;
        //                            ddfSum.days += ddf.days;
        //                            if (ddfSum.months > 12) { ddfSum.years++; ddfSum.months -= 12; }
        //                            else if (ddfSum.months < -12) { ddfSum.years--; ddfSum.months += 12; }
        //                        }
        //                        try { intCell6 = Convert.ToInt32(dgvCell6.Value); }
        //                        catch (Exception ex) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        //                        //if (intCell6 != 0)
        //                        //{
        //                        intCell7 += intCell6 * rowFactor;
        //                        dGV.Rows[row].Cells[6].Value = intCell7;
        //                        //}
        //                    }
        //                    else { dgvCell6.Value = null; dGV.Rows[row].Cells[6].Value = null; }
        //                }
        //                catch //  (Exception ex)
        //                {//  MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //                    dgvCell6.Value = null; dGV.Rows[row].Cells[6].Value = null;
        //                }
        //            }
        //            else { dgvCell6.Value = null; dGV.Rows[row].Cells[6].Value = null; }
        //        }
        //        if (intCell7 != 0) // irgendwo gibt es bereits einen Summenwert für Tage
        //        {
        //            linkLblDecText.Text = "" +
        //(!ddfSum.years.Equals(0) ? ddfSum.years.ToString() + ((Math.Abs(ddfSum.years).Equals(1) ? " Jahr" : " Jahre") +
        //(ddfSum.months.Equals(0) && ddfSum.days.Equals(0) ? "" : ", ")) : "") +
        //(!ddfSum.months.Equals(0) ? ddfSum.months.ToString() + ((Math.Abs(ddfSum.months).Equals(1) ? " Monat" : " Monate") +
        //(ddfSum.days.Equals(0) ? "" : ", ")) : "") +
        //(!ddfSum.days.Equals(0) ? ddfSum.days.ToString() + (Math.Abs(ddfSum.days).Equals(1) ? " Tag" : " Tage") : "");
        //            linkLblDecText.ToolTipText = "Ergebnis in die Zwischenablage kopieren";
        //            linkLblDecText.IsLink = true;
        //        }
        //        else
        //        { // nirgendwo gibt es einen Summenwert in 4. Spalte
        //            linkLblDecText.Text = string.Empty;
        //            linkLblDecText.ToolTipText = string.Empty;
        //            linkLblDecText.IsLink = false;
        //        }
        //    }

        private void TidyUpOnError(DataGridViewCell myCell)
        {
            if (myCell != null & ci_123TimeAny.Contains(myCell.ColumnIndex)) { myCell.Style.ForeColor = Color.Red; }
            dGV.Rows[myCell.RowIndex].Cells[5].Value = null; // Spanne
            foreach (DataGridViewRow myRow in dGV.Rows)
            {
                myRow.Cells[6].Value = null; // Saldo
                //if (myRow.Index % 2 != 0)
                //{// AlternatingCellStyle 
                //    dGV.Rows[myRow.Index].Cells[0].Style.BackColor = dGV.Rows[myRow.Index].Cells[1].Style.BackColor = dGV.Rows[myRow.Index].Cells[2].Style.BackColor = Color.WhiteSmoke;
                //}
                //else
                //{// nach dem Löschen von 1,3 oder einer anderen ungeraden Zahl von Rows
                //    dGV.Rows[myRow.Index].Cells[0].Style.BackColor = dGV.Rows[myRow.Index].Cells[1].Style.BackColor = dGV.Rows[myRow.Index].Cells[2].Style.BackColor = Color.White;
                //}
            }
        }

        private void DGV_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //foreach (DataGridViewRow myRow in dGV.Rows)
            //{// AlternatingCellStyle 
            //    if (myRow.Index % 2 == 0) { dGV.Rows[myRow.Index].Cells[0].Style.BackColor = dGV.Rows[myRow.Index].Cells[1].Style.BackColor = dGV.Rows[myRow.Index].Cells[2].Style.BackColor = Color.White; }
            //    else { dGV.Rows[myRow.Index].Cells[0].Style.BackColor = dGV.Rows[myRow.Index].Cells[1].Style.BackColor = dGV.Rows[myRow.Index].Cells[2].Style.BackColor = Color.WhiteSmoke; }
            //}
            //if (modusStunden) { UpdateLastTimeColumns(); }
            //else { UpdateLastDateColumns(); }
            UpdateLastTimeColumns();
        }

        private void HelpMessageBoxShow()
        {
            StringBuilder messageBoxCS = new();
            messageBoxCS.Append("Bei der Eingabe von Daten gelten folgende Zeichen als Separatoren:" +
                Environment.NewLine + "Punkt, Doppelpunkt, Komma, Semikolon, Schrägstrich sowie Stern-," +
                Environment.NewLine + "und Minuszeichen. Unvollständige Eingaben werden komplettiert!");
            messageBoxCS.AppendLine();
            messageBoxCS.AppendLine();
            messageBoxCS.Append("Durch Drücken der Enter- oder Tab-Taste gelangen Sie zur nächsten" +
                Environment.NewLine + "Zelle. Dabei erfolgt automatisch eine Interpretation der Eingabe. Die" +
                Environment.NewLine + "Umwandlung in ein gültiges Zeit- beziehungsweise Datumsformat" +
                Environment.NewLine + "kann auch durch Eingabe eines Doppelkreuzes (#) ausgelöst werden.");
            messageBoxCS.AppendLine();
            messageBoxCS.AppendLine();
            messageBoxCS.Append("Die aktuelle Uhrzeit bzw. das aktuelle Datum lässt sich durch Drü-" +
                Environment.NewLine + "cken der Tasten \"F5\" oder \"*\" oder \"#\" eintragen." +  //\"J\" (\"Jetzt\"), 
                Environment.NewLine + "Ein bereits eingegebenes Datum kann durch Drücken der Plus- oder" +
                Environment.NewLine + "Minus-Taste verändert werden. Wenn zusätzlich die Shift-Taste ge-" +
                Environment.NewLine + "drückt wird, ändert sich der größere Wert (z.B. Stunde statt Minute).");
            messageBoxCS.AppendLine();
            messageBoxCS.AppendLine();
            messageBoxCS.Append("Die errechnete Gesamtstundenzahl kann durch Anklicken des Links" +
                Environment.NewLine + "in der Statuszeile kurzerhand in die Zwischenablage kopiert werden.");
            MessageBox.Show(messageBoxCS.ToString(), "Hilfe - " + winTitle);
        }


        private static DateTime GetBuildDate(Assembly assembly)
        {
            // Add this to the .csproj file: <SourceRevisionId>build$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))</SourceRevisionId>
            // The value of SourceRevisionId is added to the metadata section of the version (after the +). The value is of the following form: 1.2.3+build20180101120000
            const string BuildVersionMetadataPrefix = "+build";
            AssemblyInformationalVersionAttribute attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                string value = attribute.InformationalVersion;
                int index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value[(index + BuildVersionMetadataPrefix.Length)..];
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)) { return result; }
                }
            }
            return default;
        }

        //private static DateTime RetrieveLinkerTimestamp() // lässt sich nicht in Utilities.cs verlagern
        //{
        //    string filePath = Assembly.GetCallingAssembly().Location;
        //    const int c_PeHeaderOffset = 60;
        //    const int c_LinkerTimestampOffset = 8;
        //    byte[] b = new byte[2048];
        //    Stream s = null;
        //    try
        //    {
        //        s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //        s.Read(b, 0, 2048);
        //    }
        //    finally { s?.Close(); } //  if (s != null)
        //    int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
        //    int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
        //    DateTime dt = new(1970, 1, 1, 0, 0, 0);
        //    dt = dt.AddSeconds(secondsSince1970);
        //    dt = dt.AddHours(TimeZoneInfo.Local.GetUtcOffset(dt).Hours);
        //    return dt;
        //}

        private void DGV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {// ContextMenu bei RightClick anzeigen
            int eRow = e.RowIndex; int eCol = e.ColumnIndex;
            if (eRow != -1 && e.Button == MouseButtons.Right) // Ignore if a column is clicked; 
            {// MessageBox.Show(eRow.ToString() + " | " + eCol.ToString());
                eCol = eCol == -1 ? 0 : eCol; // allows click to RowHeader
                DataGridViewCell clickedCell = dGV.Rows[eRow].Cells[eCol];
                ShowContextMenu(clickedCell, dGV, dGV.PointToClient(Cursor.Position));
            }
            else if (eCol == -1 && eRow == -1)
            {// DataGridViewTopLeftHeaderCell
                if (dGV.IsCurrentCellInEditMode)
                {
                    dGV.EndEdit();
                    dGV.CurrentCell.Selected = true;
                }
            }
        }

        private void ShowContextMenu(DataGridViewCell cCell, DataGridView ctrl, Point pos)
        {
            if (cCell != null && !cCell.IsInEditMode)
            {
                dGV.EndEdit();
                dGV.ClearSelection();
                cCell.Selected = true;
                dGV.CurrentCell = cCell;
                foreach (ToolStripItem item in contextMenuStrip.Items) { item.Enabled = true; } // Reset contextMenuStrip
                if (dGV.Rows.Count == 1 && dGV.CurrentRow.Cells[0].Value == null && dGV.CurrentRow.Cells[1].Value == null)
                {// reduziertes Menü zeigen wenn Click auf NeuerZeile als einziger Zeile ohne Inhalt erfolgt
                    allesLöschenContextMenuItem.Enabled = false;
                    zeileLöschenContextMenuItem.Enabled = false;
                }
                if (dGV.Rows[dGV.CurrentRow.Index].IsNewRow)
                { zeileLöschenContextMenuItem.Enabled = false; }
                contextMenuStrip.Show(ctrl, pos);
            }
        }

        private void AllesSpeichernContextMenuItem_Click(object sender, EventArgs e)
        {
            if (currFile.Length > 0) { SaveTextFile(currFile); }
            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK) { SaveTextFile(saveFileDialog.FileName); } //  { AskForDeskShortcut(); } }
            }
        }

        private void AllesLöschenContextMenuItem_Click(object sender, EventArgs e)
        {
            nothingToSave = false;
            dGV.Rows.Clear();
            dGV.Refresh();
            dGV.CurrentCell = dGV.Rows[0].Cells[0];
            dGV.BeginEdit(true);
        }

        private void ZeileEinfügenContextMenuItem_Click(object sender, EventArgs e)
        {
            dGV.Rows.Insert(dGV.CurrentCell.RowIndex, 1);
        }

        private void ZeileLöschenContextMenuItem_Click(object sender, EventArgs e)
        {// if (!dGV.Rows[dGV.CurrentRow.Index].IsNewRow) //if (dGV.CurrentCell.RowIndex != dGV.Rows.Count - 1)
            DeleteRowToolStripMenuItem_Click(null, null);
        }

        private void DGV_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            dGV.CurrentCell = dGV[0, dGV.CurrentRow.Index];
            dGV.BeginEdit(true);
        }

        private void LinkLblDecText_Click(object sender, EventArgs e)
        {
            if (linkLblDecText.IsLink == true)
            {
                string cbText = linkLblDecText.Text.Replace(decText, "");
                try
                {
                    if (cbText.Length > 0)
                    {
                        Clipboard.SetText(cbText, TextDataFormat.Text);
                        MessageBox.Show(cbText, "Inhalt der Zwischenablage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else { MessageBox.Show("Es liegt keine Berechnung vor!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }
        }

        private void DGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                dGV.CurrentCell.Style.BackColor = System.Drawing.Color.MistyRose;
                toolStripButtonIncrease.Enabled = true;
                toolStripButtonIncreaseMuch.Enabled = true;
                toolStripButtonDecrease.Enabled = true;
                toolStripButtonDecreaseMuch.Enabled = true;
                toolStripButtonNow.Enabled = true;
            }
        }

        private void DGV_CellLeave(object sender, DataGridViewCellEventArgs e)
        {// wenn ein Wert in den letzten beiden Spalten gelöscht wurde, soll aktualisiert werden
            if (dGV.CurrentCell.ColumnIndex > dGV.ColumnCount - 2) // in letzten beiden Spalten 
            {// MessageBox.Show(dGV.CurrentCell.Value.ToString() + " | " + e.RowIndex.ToString() + " | " + e.ColumnIndex.ToString());
                CalculateSaldo(dGV.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            }
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                dGV.CurrentCell.Style.BackColor = dGV.DefaultCellStyle.BackColor;
            }
            if (dGV.CurrentCell.ColumnIndex == ci_3Pause && (dGV.Rows[e.RowIndex].Cells[0].Value != null || dGV.Rows[e.RowIndex].Cells[1].Value != null)) // Pause
            {
                dGV.CurrentCell.Value = dGV.CurrentCell.Value == null || !rgxValidTime.Match(dGV.CurrentCell.Value.ToString()).Success ? "0:00" : dGV.CurrentCell.Value;
            }
            if (dGV.CurrentCell.ColumnIndex == ci_4Faktor && (dGV.Rows[e.RowIndex].Cells[0].Value != null || dGV.Rows[e.RowIndex].Cells[1].Value != null) && dGV.CurrentCell.Value == null) // Faktor
            {
                dGV.CurrentCell.Value = "1";
            }
            //if (dGV.CurrentCell.ColumnIndex == ci_4Faktor && (dGV.Rows[e.RowIndex].Cells[0].Value != null || dGV.Rows[e.RowIndex].Cells[1].Value != null)) // Faktor
            //{
            //    dGV.CurrentCell.Value = dGV.CurrentCell.Value == null ? "1" : dGV.CurrentCell.Value;
            //}
            toolStripButtonIncrease.Enabled = false;
            toolStripButtonIncreaseMuch.Enabled = false;
            toolStripButtonDecrease.Enabled = false;
            toolStripButtonDecreaseMuch.Enabled = false;
            toolStripButtonNow.Enabled = false;
        }

        private void DGV_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {// MessageBox.Show(dGV.SelectionMode.ToString());
            if (dGV.IsCurrentCellInEditMode)
            {
                dGV.EndEdit();
                dGV.CurrentCell.Selected = true;
            }
            if (dGV.SelectedRows.Count > 0) { dGV.ClearSelection(); }
            if ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (dGV.CurrentCell.ColumnIndex == e.ColumnIndex)
                {// erst disselect
                    dGV.CurrentCell.Selected = false;
                }// dann erneut select
                for (int r = 0; r < dGV.RowCount; r++)
                {
                    if (dGV[e.ColumnIndex, r].Selected) { dGV[e.ColumnIndex, r].Selected = false; }
                    else { dGV[e.ColumnIndex, r].Selected = true; }
                }
            }
            else
            {
                if (dGV.CurrentCell.ColumnIndex != e.ColumnIndex)
                {// CurrentCell darf nie null sein!
                    dGV.CurrentCell = dGV[e.ColumnIndex, 0];
                }
                for (int r = 0; r < dGV.RowCount; r++) { dGV[e.ColumnIndex, r].Selected = true; }
            }
        }

        private void NeuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGridViewIfNecessary();
            {
                AllesLöschenContextMenuItem_Click(null, null);
                currFile = string.Empty;
                Text = "Unbenannt - " + winTitle;
            }
        }

        private void SaveGridViewIfNecessary()
        {
            if (!nothingToSave) // && !clsUtilities.isDGVEmpty(dGV))
            {
                if (currFile.Length > 0)
                {
                    if (MessageBox.Show("Möchten Sie die Änderungen an " + currFile + " speichern?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    { nothingToSave = SaveTextFile(currFile); }
                }
                else
                {
                    if (MessageBox.Show("Möchten Sie die Eingaben speichern?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK) { SaveTextFile(saveFileDialog.FileName); } //) { AskForDeskShortcut(); } }
                    }
                }
            }
        }

        private void SpeichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFile.Length > 0) { SaveTextFile(currFile); }
            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK) { SaveTextFile(saveFileDialog.FileName); }; // { AskForDeskShortcut(); } }
            }
        }

        private void SpeichernUnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currFile.Length > 0)
            {
                saveFileDialog.FileName = Path.GetFileName(currFile); // set a default file name
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(currFile);
            }
            else { saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
            if (saveFileDialog.ShowDialog() == DialogResult.OK) { SaveTextFile(saveFileDialog.FileName); }
            //    string tempFile = currFile; // saveTextFile aktualisiert currFile
            //    if (SaveTextFile(saveFileDialog.FileName))
            //    {
            //        if (saveFileDialog.FileName != tempFile) { AskForDeskShortcut(); }
            //    }
            //}
        }

        //private void AskForDeskShortcut()
        //{
        //    if (MessageBox.Show("Möchten Sie eine Desktopverknüpfung anlegen?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //    {
        //        AddShortcutToDesktop();
        //    }
        //}

        private void BeendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UhrzeitDatumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && dGV.CurrentCell.ColumnIndex < dGV.ColumnCount - 2 && (ActiveControl == dGV || dGV.IsCurrentCellInEditMode))
            {
                keybd_event(VK_MULTIPLY, 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event(VK_MULTIPLY, 0, KEYEVENTF_KEYUP, 0); // VK_OEM_2 kann je nach Tastatur variieren. #
            }
        }

        private void InsertRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.IsCurrentCellInEditMode) { dGV.EndEdit(); }
            dGV.Rows.Insert(dGV.CurrentCell.RowIndex, 1);
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {// NICHT wenn Cursor in letzter (neu angelegter) Zeile steht!
            if (dGV.CurrentCell.RowIndex != dGV.Rows.Count - 1)
            {
                if (dGV.IsCurrentCellInEditMode) { dGV.EndEdit(); }
                //int oldRowIndex = dGV.CurrentCellAddress.X;
                if (dGV.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dGV.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < 3; i++)
                            {// nur wg. nothingToSave
                                if (row.Cells[i].Value != null)
                                {
                                    nothingToSave = false;
                                    break;
                                }
                            }
                            dGV.Rows.Remove(row);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {// nur wg. nothingToSave
                        if (dGV.CurrentCell.OwningRow.Cells[i].Value != null)
                        {
                            nothingToSave = false;
                            break;
                        }
                    }
                    dGV.Rows.RemoveAt(dGV.CurrentCell.RowIndex);
                }
                TidyUpOnError(dGV.CurrentCell);
                dGV.BeginEdit(true); // Workaround damit neue Zelle validiert wird
                dGV.EndEdit();
                //if (oldRowIndex < dGV.RowCount) { dGV.CurrentCell = dGV.Rows[oldRowIndex + 1].Cells[0]; } // CurrentRow eine Zeile tiefer
                dGV.CurrentCell.Selected = true;
            }
            else { Console.Beep(); }
        }

        private void AusschneidenToolStripMenuItem_Click(object sender, EventArgs e)
        {// dGV.Focus(); darf hier nicht stehen weil Zelle in EditMode sonst den Fokus verliert!
            if (!dGV.CurrentRow.Selected && !dGV.IsCurrentCellInEditMode)
            {
                if (dGV.CurrentCell.Value != null)
                {
                    Clipboard.SetText(dGV.CurrentCell.Value.ToString(), TextDataFormat.Text);
                    dGV.CurrentCell.Value = null;
                    TidyUpOnError(dGV.CurrentCell);
                    dGV.BeginEdit(true);
                    nothingToSave = false;
                }
                else { Console.Beep(); }
            }
            else if (dGV.IsCurrentCellInEditMode)
            {
                if (((TextBox)dGV.EditingControl).SelectionLength > 0)
                {
                    ((TextBox)dGV.EditingControl).Cut();
                }
            }
        }

        private void KopierenToolStripMenuItem_Click(object sender, EventArgs e)
        {// dGV.Focus(); darf hier nicht stehen weil Zelle in EditMode sonst den Fokus verliert!
            if (!dGV.CurrentRow.Selected && !dGV.IsCurrentCellInEditMode)
            {
                if (dGV.CurrentCell.Value != null)
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(dGV.CurrentCell.Value.ToString(), TextDataFormat.Text);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                }
                else { Console.Beep(); }
            }
            else if (dGV.IsCurrentCellInEditMode)
            {// Ensure that text is selected in the text box.
                if (((TextBox)dGV.EditingControl).SelectionLength > 0)
                {// SendKeys.Send("^{c}");
                    ((TextBox)dGV.EditingControl).Copy();
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {// dGV.Focus(); darf hier nicht stehen weil Zelle in EditMode sonst den Fokus verliert!
            if (!dGV.IsCurrentCellInEditMode)
            {
                int iCol = dGV.CurrentCell.ColumnIndex;
                int iRow = dGV.CurrentCell.RowIndex;
                if (iCol < 2 && iRow == dGV.Rows.Count - 1)
                {
                    dGV.Rows.Add();
                    dGV.CurrentCell = dGV.Rows[dGV.CurrentCell.RowIndex - 1].Cells[iCol];
                    dGV.CurrentCell.Selected = true;
                }
                dGV.CurrentCell.Value = null;
                try
                {
                    dGV.CurrentCell.Value = Clipboard.GetText();
                    dGV.BeginEdit(true);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                dGV.RefreshEdit();
            }
            else if (dGV.IsCurrentCellInEditMode)
            {// Determine if there is any text in the Clipboard to paste into the text box.
                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
                {
                    ((TextBox)dGV.EditingControl).Paste();
                }
            }
        }

        private void LöschenToolStripMenuItem_Click(object sender, EventArgs e)
        {// dGV.Focus(); darf hier nicht stehen weil Zelle in EditMode sonst den Fokus verliert!
            if (dGV.IsCurrentCellInEditMode)
            {// Ensure that text is selected in the text box.   
                if (((TextBox)dGV.EditingControl).SelectionLength > 0)
                {// nothingToSave = false;
                    ((TextBox)dGV.EditingControl).SelectedText = "";
                }
            }
            else
            {
                foreach (DataGridViewCell cell in dGV.SelectedCells)
                {
                    //if (cell.Value != null) { nothingToSave = false; }
                    cell.Value = null;
                    //if (dGV.CurrentCell.ColumnIndex != 2 && dGV.CurrentCell.ColumnIndex != 4) { TidyUpOnError(cell); } // nicht in Datumspalte
                }
                //dGV.Refresh();
                dGV.ClearSelection();
                dGV.CurrentCell.Selected = true;
                dGV.BeginEdit(true);
            }
        }

        private void HilfeAnzeigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpMessageBoxShow();
        }

        private void DGV_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dGV.IsCurrentCellInEditMode)
            {
                dGV.EndEdit();
                dGV.CurrentCell.Selected = true;
            }
        }

        private void AllesAuswählenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dGV.SelectAll();
        }

        private void WertErhöhenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                ChangeCellValueByKey(false, true); // Shift, Add
            }
        }

        private void WertVerringernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                ChangeCellValueByKey(false, false); // Shift, Add
            }
        }

        private void GroßenWertErhöhenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                ChangeCellValueByKey(true, true); // Shift true, Add true
            }
        }

        private void GroßenWertVerringernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten
            {
                ChangeCellValueByKey(true, false); // Shift true, Add false
            }
        }

        private void AllesLöschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllesLöschenContextMenuItem_Click(null, null);
        }

        private void ToolStripMenuItemShortcut_Click(object sender, EventArgs e) { AddShortcutToDesktop(); }

        private void MainMenuStrip_MenuDeactivate(object sender, EventArgs e)
        {
            ausschneidenToolStripMenuItem.Enabled = false;
            kopierenToolStripMenuItem.Enabled = false;
            löschenToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            wertErhöhenToolStripMenuItem.Enabled = false;
            wertVerringernToolStripMenuItem.Enabled = false;
            großenWertErhöhenToolStripMenuItem.Enabled = false;
            großenWertVerringernToolStripMenuItem.Enabled = false;
            deleteRowToolStripMenuItem.Enabled = false;
            uhrzeitDatumToolStripMenuItem.Enabled = false;
        }

        private void MainMenuStrip_MenuActivate(object sender, EventArgs e)
        {
            if (dGV.IsCurrentCellInEditMode)
            {
                enterToolStripMenuItem.Text = "&Verlassen";
                enterToolStripMenuItem.ShortcutKeyDisplayString = "Eingabtaste";
                enterToolStripMenuItem.Image = Properties.Resources.GoToNext;
            }
            else
            {
                enterToolStripMenuItem.Text = "E&ditieren";
                enterToolStripMenuItem.ShortcutKeyDisplayString = "F2 oder Eingabtaste";
                enterToolStripMenuItem.Image = Properties.Resources.EditTable;
            }
            if (dGV.CurrentCell.ColumnIndex != 2) // dGV.CurrentCell.EditType == typeof(DataGridViewComboBoxEditingControl funkt nicht in EditMode
            {
                uhrzeitDatumToolStripMenuItem.Enabled = true;
                DateTime dt; // = new DateTime();
                dGV.CurrentCell ??= dGV.Rows[0].Cells[0]; // Fehler wenn CurrentCell nicht existiert: 
                string dtFormat = ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex) ? timeFormat : dateFormat; //string[] dtFormats = { "H:mm", "d.M.yyyy" };

                if (ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex))
                {
                    wertErhöhenToolStripMenuItem.Text = "&Minute steigern";
                    wertVerringernToolStripMenuItem.Text = "M&inute mindern";
                    großenWertErhöhenToolStripMenuItem.Text = "&Stunde steigern";
                    großenWertVerringernToolStripMenuItem.Text = "S&tunde mindern";
                    //uhrzeitDatumToolStripMenuItem.Text = "A&kt. Uhrzeit";
                }
                else
                {
                    wertErhöhenToolStripMenuItem.Text = "&Tag steigern";
                    wertVerringernToolStripMenuItem.Text = "Ta&g mindern";
                    großenWertErhöhenToolStripMenuItem.Text = "&Monat steigern";
                    großenWertVerringernToolStripMenuItem.Text = "M&onat mindern";
                    //uhrzeitDatumToolStripMenuItem.Text = "A&kt. Datum";
                }

                if (dGV.IsCurrentCellInEditMode && ((TextBox)dGV.EditingControl).SelectionLength > 0)
                {
                    ausschneidenToolStripMenuItem.Enabled = true;
                    kopierenToolStripMenuItem.Enabled = true;
                    löschenToolStripMenuItem.Enabled = true;
                }
                else if (dGV.CurrentCell.Value != null)
                {
                    ausschneidenToolStripMenuItem.Enabled = true;
                    kopierenToolStripMenuItem.Enabled = true;
                    löschenToolStripMenuItem.Enabled = true;
                }

                if (dGV.IsCurrentCellInEditMode)
                {
                    string tbText = ((TextBox)dGV.EditingControl).Text;
                    if (ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex)) { dt = ClsUtilities.NormalizeTime(tbText.Length == 0 ? DateTime.Now.ToString(dtFormat) : tbText); }
                    else { dt = ClsUtilities.NormalizeDate(tbText.Length == 0 ? DateTime.Now.ToString(dtFormat) : tbText); }
                    if (dt != DateTime.MinValue)
                    {
                        wertErhöhenToolStripMenuItem.Enabled = true;
                        wertVerringernToolStripMenuItem.Enabled = true;
                        großenWertErhöhenToolStripMenuItem.Enabled = true;
                        großenWertVerringernToolStripMenuItem.Enabled = true;
                    }
                }
                else if (!dGV.IsCurrentCellInEditMode)
                {
                    if (dGV.CurrentCell.Value == null || DateTime.TryParseExact(dGV.CurrentCell.Value.ToString(), dtFormat, deCulture, DateTimeStyles.None, out _))
                    {
                        wertErhöhenToolStripMenuItem.Enabled = true;
                        wertVerringernToolStripMenuItem.Enabled = true;
                        großenWertErhöhenToolStripMenuItem.Enabled = true;
                        großenWertVerringernToolStripMenuItem.Enabled = true;
                    }
                }

                if (Clipboard.ContainsText()) { pasteToolStripMenuItem.Enabled = true; }

                if (!dGV.Rows[dGV.CurrentRow.Index].IsNewRow) { deleteRowToolStripMenuItem.Enabled = true; }
            }
        }

        private void EnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iCol = dGV.CurrentCell.ColumnIndex;
            int iRow = dGV.CurrentCell.RowIndex;
            if (iCol < dGV.ColumnCount - 2) // nicht in letzten beiden Spalten
            {
                if (dGV.IsCurrentCellInEditMode)
                {
                    if (iCol == dGV.ColumnCount - 3) // in 3. = letzte EditSpalte
                    {
                        if (iRow + 1 == dGV.RowCount) { dGV.Rows.Add(); }
                        dGV.CurrentCell = dGV[0, iRow + 1];
                    }
                    else { dGV.CurrentCell = dGV[iCol + 1, iRow]; }
                }
                else { dGV.BeginEdit(true); }
            }
            else // in den letzten beiden Spalten
            {// CurrentCell soll in jedem Fall in neuer Zeile starten
                if (iRow + 1 == dGV.RowCount) { dGV.Rows.Add(); }
                dGV.CurrentCell = dGV[0, iRow + 1];
            }
        }

        private void ÖffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGridViewIfNecessary();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dGV.Rows.Clear(); //dGV.AllowUserToAddRows = false; // wird später wieder auf true gesetzt
                File.SetLastAccessTime(openFileDialog.FileName, DateTime.Now); // The NtfsDisableLastAccessUpdate registry setting is enabled by default
                ReadTextFile(openFileDialog.FileName);
                //if (dGV.Rows[0].Cells[0].Value != null && rgxValidDate.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success) // Regex.Match(dGV.Rows[0].Cells[0].Value.ToString(), @"^\d{1,2}\.\d{1,2}\.\d{4}$", RegexOptions.Compiled).Success)
                //{ modusStunden = false; }
                //else if (dGV.Rows[0].Cells[0].Value != null && rgxValidTime.Match(dGV.Rows[0].Cells[0].Value.ToString()).Success) // Regex.Match(dGV.Rows[0].Cells[0].Value.ToString(), @"^\d{1,2}\:\d{2}$", RegexOptions.Compiled).Success)
                //{ modusStunden = true; }
            }
        }

        private void FrmTimeCalc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!nothingToSave && e.CloseReason == CloseReason.UserClosing) // && !clsUtilities.isDGVEmpty(dGV))
            {
                if (currFile.Length > 0)
                {
                    DialogResult dlgResult = MessageBox.Show("Möchten Sie die Änderungen an " + currFile + " speichern?", winTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (dlgResult)
                    {
                        case DialogResult.Yes:
                            if (!SaveTextFile(currFile)) { e.Cancel = true; }
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            e.Cancel = true; // cancel the closure of the form.
                            break;
                    }
                }
                else
                {
                    if (MessageBox.Show("Möchten Sie die Eingaben speichern?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (!SaveTextFile(saveFileDialog.FileName)) { e.Cancel = true; }
                            //string tempFile = currFile; // saveTextFile aktualisiert currFile
                            //if (SaveTextFile(saveFileDialog.FileName))
                            //{
                            //    if (saveFileDialog.FileName != tempFile) { AskForDeskShortcut(); }
                            //}
                            //else { e.Cancel = true; } // Speichern führte zu Fehler
                        }
                        else { e.Cancel = true; } // User möchte nicht speichern
                    }
                }
            }
            //strFormat.Dispose();
        }

        private DataTable CreateDataTable()
        {
            DataTable dtData = new("Data");
            for (int i = 0; i < dGV.ColumnCount - 2; ++i)
            {
                dtData.Columns.Add(new DataColumn(dGV.Columns[i].Name));
                dGV.Columns[i].DataPropertyName = dGV.Columns[i].Name;
            }
            for (int row = 0; row < dGV.RowCount - 1; row++)
            {
                DataRow dr = dtData.NewRow();
                for (int col = 0; col < dGV.Columns.Count - 2; col++) { dr[col] = dGV.Rows[row].Cells[col].Value != null ? dGV.Rows[row].Cells[col].Value.ToString() : ""; }
                dtData.Rows.Add(dr);
            }
            return dtData;
        }

        private bool SaveTextFile(string fullFilename)
        {
            try
            {
                if (dGV.IsCurrentCellInEditMode)
                {
                    dGV.EndEdit();
                    dGV.CurrentCell.Selected = true;
                }
                ClsUtilities.RemoveEmptyRows(dGV, ci_0123DateTimeAny);
                DataTable dtData = CreateDataTable();
                using (DataSet ds = new()) //Finally the save part:
                {
                    ds.DataSetName = "TimeCalc";
                    ds.Tables.Add(dtData);
                    ds.Tables.Add(dtDays);
                    fullFilename = Path.ChangeExtension(fullFilename, ".tcf");
                    XmlTextWriter xmlSave = new(fullFilename, Encoding.UTF8) { Formatting = Formatting.Indented };
                    xmlSave.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
                    ds.WriteXml(xmlSave);
                    xmlSave.Close();
                }
                dtData.Clear();
                nothingToSave = true;
                currFile = fullFilename;
                Text = currFile + " - " + winTitle;  // Text = Path.GetFileName(fullFilename) + " - " + winTitle;
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return false;
        }

        private bool ReadTextFile(string fullFilename)
        {
            try
            {
                dGV.Rows.Clear();
                DataTable dtTemp = null;
                DataTable data = new("Data");
                using DataSet dataSet = new();
                dataSet.ReadXml(fullFilename);
                data = dataSet.Tables["Data"];
                foreach (DataRow dr in data.Rows) { dGV.Rows.Add(dr.ItemArray); }
                UpdateLastTimeColumns();
                data.Clear();
                dtTemp = dataSet.Tables["Days"];
                if (dtTemp != null) { dtDays = dtTemp.Copy(); } // "DataTable already belongs to another DataSet."
                currFile = fullFilename;
                ShowCellChangeInTitle();
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return false;
        }

        private void DGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {// Kein Semikolon in TextBoxen erlauben
            e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress); // remove an existing event-handler, if present
            e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress); // add the event handler
            e.Control.KeyDown -= new KeyEventHandler(Control_KeyDown); // viel Aufwand um NumpadDecimal-Komma mit Punkt zu ersetzen
            e.Control.KeyDown += new KeyEventHandler(Control_KeyDown); // s.o.
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            var tB = sender as DataGridViewTextBoxEditingControl;
            if (e.KeyValue == 110) // NumpadDecimal
            {
                numPadDecimal = true;
                int iPos = tB.SelectionStart; // Cursorpositon in TextBox
                if (tB.SelectionLength > 0)
                {// falls Text markiert ist, wird er gelöscht
                    tB.Text = tB.Text.Remove(iPos, tB.SelectionLength);
                }
                tB.Text = tB.Text.Insert(iPos, ".");
                tB.SelectionStart = iPos + 1; // reposition cursor
            }
        }

        private void Control_KeyPress(object sender, KeyPressEventArgs e)
        {// This event occurs after the KeyDown event and can be used to prevent characters from entering the control
            if (numPadDecimal == true)
            {// Stop the character from being entered into the control
                e.Handled = true;
                numPadDecimal = false;
            }
            if (dGV.CurrentCell.EditType == typeof(DataGridViewComboBoxEditingControl))
            {
                ComboBox cmbBox = (ComboBox)sender;
                cmbBox.SelectedIndex = 1; //dGV.BeginEdit(true); ((ComboBox)dGV.EditingControl).DroppedDown = true;
            }
            else if (dGV.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl))
            {
                TextBox txtBox = (TextBox)sender;
                if (e.KeyChar == ';') { e.KeyChar = ','; } //{ Console.Beep(); e.Handled = true; } // if (char.IsNumber(e.KeyChar))
                if (e.KeyChar == '#' || e.KeyChar == '*') // NormalizeDate manuell auslösen
                {
                    DateTime dt; // = new DateTime();
                    string dtFormat = ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex) || dGV.CurrentCell.ColumnIndex == ci_3Pause ? timeFormat : dateFormat;
                    string strTxtBox = txtBox.TextLength == 0 ? DateTime.Now.ToString(dtFormat) : txtBox.Text;
                    if (ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex)) { dt = ClsUtilities.NormalizeTime(strTxtBox); }
                    else if (dGV.CurrentCell.ColumnIndex == ci_3Pause) { dt = ClsUtilities.NormalizePause("0"); }
                    else { dt = ClsUtilities.NormalizeDate(strTxtBox); }
                    if (dt != DateTime.MinValue)
                    {
                        e.Handled = true; // Doppelkreuz '#' weglassen
                        txtBox.Text = dt.ToString(dtFormat); // Textbox Inhalt ersetzten
                        txtBox.Select(txtBox.Text.Length, 0); // Cursor ans Ende setzten (Markierungslänge = 0)
                    }
                }
            }
        }

        private void WebseiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new("https://www.ophthalmostar.de/freeware/#timecalc") { UseShellExecute = true };
                Process.Start(psi);
            }
            catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mbText =
@"Das Programm ist Freeware. Sie dürfen es kostenlos nutzen
und  weitergeben, aber nicht verändern.
Die Benutzung erfolgt auf eigene Gefahr! Der Autor ist nicht
für Schäden verantwortlich, die durch Verwendung oder Ver-
breitung der Software verursacht werden.
In keinem Fall ist der Autor verantwortlich für entgangenen
Umsatz, Gewinn oder andere finanzielle Folgen, den Verlust
von Daten sowie unmittelbare oder mittelbare Folgeschäden,
die durch den Gebrauch der Software verursacht wurden.

Autor/Copyright: Dr. Wilhelm Happe, Kiel
";
            mbText = string.Concat(mbText, Environment.NewLine + "Programmversion: " + curVersion + ", Build vom " + GetBuildDate(Assembly.GetExecutingAssembly()).ToLocalTime().ToString("d.M.yyyy")
                + Environment.NewLine + Environment.NewLine + "Installationspfad: " + Application.ExecutablePath);
            MessageBox.Show(mbText, winTitle, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void DGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].EditType == typeof(DataGridViewComboBoxEditingControl))
            {
                dGV.ClearSelection();
                dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "1";
                dGV.RefreshEdit(); // Aktualisiert den Wert der aktiven Zelle mit dem zugrunde liegenden Zellenwert.
            }
            else
            {
                MessageBox.Show("Fehler in Zeile " + (e.RowIndex + 1).ToString() + ", Spalte "
                    + (e.ColumnIndex + 1).ToString() + ".", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PanelHeader_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            LinearGradientBrush myBrush = new(new Point(0, 0), new Point(Width, Height), System.Drawing.Color.AliceBlue, System.Drawing.Color.LightSteelBlue);
            g.FillRectangle(myBrush, ClientRectangle);
        }

        private void ToolStripButtonNewFile_Click(object sender, EventArgs e) { NeuToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonSave_Click(object sender, EventArgs e) { SpeichernToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonOpenFile_Click(object sender, EventArgs e) { ÖffnenToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonImport_Click(object sender, EventArgs e) { ImportToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonSaveAs_Click(object sender, EventArgs e) { SpeichernUnterToolStripMenuItem_Click(null, null); }
        private void ToolStripButton2_Click(object sender, EventArgs e) { ToolStripMenuItemExcel_Click(null, null); }
        //private void ToolStripButtonCut_Click(object sender, EventArgs e) { AusschneidenToolStripMenuItem_Click(null, null); }
        //private void ToolStripButtonPaste_Click(object sender, EventArgs e) { PasteToolStripMenuItem_Click(null, null); }
        //private void ToolStripButtonCopy_Click(object sender, EventArgs e) { KopierenToolStripMenuItem_Click(null, null); }
        //private void ToolStripButtonDelete_Click(object sender, EventArgs e) { LöschenToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonIncrease_Click(object sender, EventArgs e) { WertErhöhenToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonIncreaseMuch_Click(object sender, EventArgs e) { GroßenWertErhöhenToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonDecrease_Click(object sender, EventArgs e) { WertVerringernToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonDecreaseMuch_Click(object sender, EventArgs e) { GroßenWertVerringernToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonNow_Click(object sender, EventArgs e) { UhrzeitDatumToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonNewRow_Click(object sender, EventArgs e) { InsertRowToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonDeleteRow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Möchten Sie die aktuelle Zeile löschen?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                DeleteRowToolStripMenuItem_Click(null, null);
            }
        }
        //private void ToolStripButtonSelectAll_Click(object sender, EventArgs e) { AllesAuswählenToolStripMenuItem_Click(null, null); }
        //private void ToolStripButtonDeleteAll_Click(object sender, EventArgs e) { AllesLöschenContextMenuItem_Click(null, null); }
        private void WeekStaticsToolStripMenuItem_Click(object sender, EventArgs e) { AnalyzeWeekData(); }
        private void ToolStripButtonModus_Click(object sender, EventArgs e) { AnalyzeMonthData(); }
        private void ToolStripButtonWeek_Click(object sender, EventArgs e) { AnalyzeWeekData(); }
        private void ToolStripButtonHelp_Click(object sender, EventArgs e) { HilfeAnzeigenToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonDeskLink_Click(object sender, EventArgs e) { AddShortcutToDesktop(); }
        private void ToolStripButtonPrint_Click(object sender, EventArgs e) { ShowPrintDialog(); }
        private void ToolStripButtonPreview_Click(object sender, EventArgs e) { ToolStripMenuItemPrintPreview_Click(null, null); }
        private void ToolStripButtonExit_Click(object sender, EventArgs e) { Close(); }
        private void ToolStripButtonInfo_Click(object sender, EventArgs e) { InfoToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonWeb_Click(object sender, EventArgs e) { WebseiteToolStripMenuItem_Click(null, null); }
        private void ToolStripButtonComplete_Click(object sender, EventArgs e)
        {
            if (dGV.CurrentCell != null && ci_0123DateTimeAny.Contains(dGV.CurrentCell.ColumnIndex)) // nicht in letzten beiden Splaten // dGV.CurrentCell.EditType == typeof(DataGridViewTextBoxEditingControl)
            {// if (!dGV.IsCurrentCellInEditMode) { dGV.BeginEdit(true); }
                var txtBox = (TextBox)dGV.EditingControl;
                DateTime dt; // = new DateTime();
                string dtFormat = ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex) ? timeFormat : dateFormat;
                string strTxtBox = txtBox.TextLength == 0 ? DateTime.Now.ToString(dtFormat) : txtBox.Text;
                if (ci_12VonBis.Contains(dGV.CurrentCell.ColumnIndex)) { dt = ClsUtilities.NormalizeTime(strTxtBox); }
                else { dt = ClsUtilities.NormalizeDate(strTxtBox); }
                if (dt != DateTime.MinValue)
                {
                    txtBox.Text = dt.ToString(dtFormat); // Textbox Inhalt ersetzten
                    txtBox.Select(txtBox.Text.Length, 0); // Cursor ans Ende setzten (Markierungslänge = 0)
                }
                else { Console.Beep(); }
            }
        }

        private void AddShortcutToDesktop()
        {
            string deskText = currFile.Length > 0 ? Path.GetFileName(currFile) : ClsUtilities.GetDescription();
            string linkFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), deskText + ".lnk");
            using ShellShortcut shellShortcut = new(linkFileName)
            {
                Path = Application.ExecutablePath,
                WorkingDirectory = Application.StartupPath,
                Arguments = currFile.Length > 0 ? currFile.Contains(' ') ? "\"" + currFile + "\"" : currFile : "",
                IconPath = Application.ExecutablePath,
                IconIndex = 0,
                Description = deskText,
            };
            try
            {
                shellShortcut.Save();
                MessageBox.Show("Die Desktopverknüpfung wurde erfolgreich angelegt:" + Environment.NewLine + linkFileName, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void ToolStripMenuItemPrintPreview_Click(object sender, EventArgs e)
        {
            if (dGV.IsCurrentCellInEditMode)
            {
                dGV.EndEdit();
                dGV.CurrentCell.Selected = true;
            }
            using FrmPrintPreview f2 = new(this);
            f2.Text = currFile.Length > 0 ? currFile + " - Seitenanzeige" : "Seitenanzeige";
            f2._printPreviewControl.Document = printDocument;
            f2.ShowDialog(this);
        }

        private void ToolStripMenuItemPrint_Click(object sender, EventArgs e)
        {
            ShowPrintDialog();
        }

        internal void ShowPrintDialog()
        {// how to call non-static method on form1 from form2
            if (dGV.IsCurrentCellInEditMode)
            {
                dGV.EndEdit();
                dGV.CurrentCell.Selected = true;
            }
            using FrmPrintConfig f3 = new(this);
            f3.ShowDialog(this);
        }

        internal void PrintNowFromChild()
        {// how to call non-static method on form1 from form2
            try
            {
                if (!ClsUtilities.IsDGVEmpty(dGV))
                {
                    printDocument.DocumentName = currFile.Length > 0 ? Path.GetFileName(currFile) : "";
                    printDocument.Print();
                }
                else { MessageBox.Show("Es gibt nichts zu drucken!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }
            catch (Exception) { MessageBox.Show("Fehler beim Drucken", "Fehler"); }
        }

        public void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                intRow = 0;
                bFirstPage = true;
                bNewPage = true;
                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dGV.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            StringFormat strFormat = new()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            try
            {
                int iLeftMargin = e.MarginBounds.Left;
                int iTopMargin = e.MarginBounds.Top;
                bool bMorePagesToPrint = false; // Whether more pages have to print or not
                int iTmpWidth = 0;
                if (bFirstPage)
                {// For the first page to print set the cell width and header height
                    foreach (DataGridViewColumn GridCol in dGV.Columns)
                    {
                        iTmpWidth = (int)Math.Floor((double)(GridCol.Width / (double)iTotalWidth * iTotalWidth * (e.MarginBounds.Width / (double)iTotalWidth)));
                        iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText, GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;
                        arrColumnLefts.Add(iLeftMargin); // Save width and height of headres
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }
                while (intRow <= dGV.Rows.Count - 2)
                {// Loop till all the grid rows not get printed
                    DataGridViewRow GridRow = dGV.Rows[intRow];
                    iCellHeight = GridRow.Height + 5; // Set the cell height
                    int iCount = 0;
                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {// Check whether the current page settings allo more rows to print
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {// Draw Header
                            e.Graphics.DrawString(currFile, new System.Drawing.Font(dGV.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top - e.Graphics.MeasureString(currFile, new System.Drawing.Font(dGV.Font, FontStyle.Bold), e.MarginBounds.Width).Height - 13);
                            string strDate = DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString();
                            e.Graphics.DrawString(strDate, new System.Drawing.Font(dGV.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(strDate, new System.Drawing.Font(dGV.Font, FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top - e.Graphics.MeasureString("Customer Summary", new System.Drawing.Font(new System.Drawing.Font(dGV.Font, FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);
                            iTopMargin = e.MarginBounds.Top; // Draw Columns
                            foreach (DataGridViewColumn GridCol in dGV.Columns)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.LightGray), new Rectangle((int)arrColumnLefts[iCount], iTopMargin, (int)arrColumnWidths[iCount], iHeaderHeight));
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount], iTopMargin, (int)arrColumnWidths[iCount], iHeaderHeight));
                                e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font, new SolidBrush(GridCol.InheritedStyle.ForeColor), new RectangleF((int)arrColumnLefts[iCount], iTopMargin, (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;
                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {// Draw Columns Contents                
                            if (Cel.Value != null)
                            {
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font, new SolidBrush(Cel.InheritedStyle.ForeColor), new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin, (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                            }
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount], iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));
                            iCount++;
                        }
                    }
                    intRow++;
                    iTopMargin += iCellHeight;
                }
                if (bMorePagesToPrint) { e.HasMorePages = true; } // If more lines exist, print another page.
                else { e.HasMorePages = false; }

            }
            catch (Exception exc) { MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { strFormat.Dispose(); }
        }

        private void ToolStripMenuItemExcel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currFile)) { SpeichernUnterToolStripMenuItem_Click(null, null); }
            if (currFile.Length > 0)
            {
                string filePath = Path.Combine(Path.GetDirectoryName(currFile), Path.GetFileNameWithoutExtension(currFile) + "_Excel.csv");
                if (File.Exists(filePath) && MessageBox.Show("Die vorhandene CSV-Datei wird ersetzt.\n" + filePath, winTitle, MessageBoxButtons.OKCancel) == DialogResult.Cancel) { return; }
                try { File.Delete(filePath); }
                catch (IOException ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                DataTable dt = new();
                dt.Columns.Add("Datum");
                dt.Columns.Add("von");
                dt.Columns.Add("bis");
                dt.Columns.Add("Pause");
                dt.Columns.Add("x");
                dt.Columns.Add("Spanne");
                dt.Columns.Add("Saldo");
                for (int row = 0; row < dGV.RowCount; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 0; col < dGV.Columns.Count; col++) { dr[col] = dGV.Rows[row].Cells[col].Value != null ? dGV.Rows[row].Cells[col].Value.ToString() : ""; }
                    dt.Rows.Add(dr);
                }
                //int count = 50; // Datei muss mind. 7 KB groß sein, sonst meldet Microsoft Excel Fehler
                //if (dt.Rows.Count < count)
                //{
                //    count -= dt.Rows.Count;
                //    for (int i = 0; i < count; i++) { dt.Rows.Add("", "", "", "", "", "", ""); }
                //}

                try
                {
                    //CreateExcelFile.CreateExcelDocument(dt, filePath);

                    StringBuilder sb = new();
                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(";", columnNames));
                    foreach (DataRow row in dt.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                        sb.AppendLine(string.Join(";", fields));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                }
                catch (FileNotFoundException excel) { MessageBox.Show(excel.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                if (File.Exists(filePath))
                {
                    try
                    {
                        ProcessStartInfo psi = new(filePath) { UseShellExecute = true }; // for non-executables
                        Process.Start(psi);
                    }
                    catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using FrmImportIntro IntroForm = new();
            if (IntroForm.ShowDialog() == DialogResult.OK)
            {

                if (IntroForm.impFromFile)
                {
                    if ((IntroForm.impIndex == 1 && !ImportTextFileFLEX(IntroForm.ImportForm_fileDialog.FileName)) ||
                        (IntroForm.impIndex == 2 && !ImportTextFileStempel(IntroForm.ImportForm_fileDialog.FileName)) ||
                        (IntroForm.impIndex == 3 && !ImportTextFileTimeCalc(IntroForm.ImportForm_fileDialog.FileName)) ||
                        (IntroForm.impIndex == 0 && !ImportTextFileBMAS(IntroForm.ImportForm_fileDialog.FileName)))
                    { MessageBox.Show("Die Datei »" + IntroForm.ImportForm_fileDialog.FileName + "« enthält keine verwertbare Daten.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
                else
                {
                    if ((IntroForm.impIndex == 1 && !ImportClipboardTextFLEX()) ||
                        (IntroForm.impIndex == 2 && !ImportClipboardTextStempel()) ||
                        (IntroForm.impIndex == 3 && !ImportClipboardTextTimeCalc()) ||
                        (IntroForm.impIndex == 0 && !ImportClipboardTextBMAS()))
                    { MessageBox.Show("Die Windows-Zwischenablage enthält keine für den Import geeignete Daten.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
            }
        }

        private bool ImportTextFileTimeCalc(string fullFilename)
        {
            try
            {
                lblWait.Visible = true; // PseudoSplashScreen
                Application.DoEvents();
                bool oldFormat = false;
                int index = 0;
                using StreamReader sReader = new(fullFilename);
                // You're better off using the using keyword; then you don't need to explicitly close anything.
                string sLine = "";
                sLine = sReader.ReadLine(); // erste Zeile lesen (gelangt nicht ins DataGridView)
                oldFormat = sLine.Contains(";Notiz;"); // stellt Kompatiblität zu alter Version her
                sLine = sReader.ReadLine(); // zweite Zeile
                while (sLine != null)
                {
                    if (oldFormat)
                    {
                        string[] tokens = sLine.Split(';');
                        sLine =
                            DateTime.Parse(tokens[4]).ToString("dd.MM.yyyy") + ";" +
                            tokens[0][..tokens[0].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[0][^2..] + ";" +
                            tokens[1][..tokens[1].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[1][^2..] + ";" +
                            tokens[2][..tokens[2].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[2][^2..] + ";" +
                            tokens[3];
                    }
                    dGV.Rows.Add(sLine.Split(';', '\t'));
                    sLine = sReader.ReadLine();
                    index++;
                }
                if (index != 0)
                {
                    UpdateLastTimeColumns();
                    ShowCellChangeInTitle();
                    nothingToSave = false;
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportClipboardTextTimeCalc()
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                bool oldFormat = false;
                int index = 0;
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);
                    if (text.Length > 1)
                    {
                        text = text.Replace("\r\n", "\r").Replace("\n", "\r"); //  unify all line breaks to \r
                        string[] textArray = text.Split('\r'); //  create an array of lines
                        oldFormat = textArray[0].Contains(";Notiz;"); // stellt Kompatiblität zu alter Version her
                        foreach (string sLine in textArray)
                        {
                            if (string.IsNullOrEmpty(sLine)) { continue; }
                            else if (sLine.Contains("von")) { continue; }
                            else if (oldFormat)
                            {
                                string[] tokens = sLine.Split(';');
                                string token =
                                    DateTime.Parse(tokens[4]).ToString("dd.MM.yyyy") + ";" +
                                    tokens[0][..tokens[0].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[0][^2..] + ";" +
                                    tokens[1][..tokens[1].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[1][^2..] + ";" +
                                    tokens[2][..tokens[2].IndexOf(':')].PadLeft(2, '0') + ":" + tokens[2][^2..] + ";" +
                                    tokens[3];
                                dGV.Rows.Add(token.Split(';'));
                            }
                            else { dGV.Rows.Add(sLine.Split(';').SkipLast(2)); }
                            index++;
                        }
                    }
                    if (index != 0)
                    {
                        UpdateLastTimeColumns();
                        ShowCellChangeInTitle();
                        nothingToSave = false;
                        return true;
                    }
                    else { return false; }
                }
                else { MessageBox.Show("Die Zwischenablage enhält keinen Text!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); return true; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportTextFileStempel(string fullFilename)
        {
            try
            {
                lblWait.Visible = true; // PseudoSplashScreen
                Application.DoEvents();
                int index = 0;
                Regex rgxValidDate = new(@"(\d){2,4}-(0?[1-9]|1[012])-([012]?[0-9]|3[01])", RegexOptions.CultureInvariant);
                using (StreamReader sReader = new(fullFilename))
                {
                    string sLine;
                    Match match;
                    int intVon = -1, intBis = -1, intPause = -1;
                    string varVon = string.Empty, varBis = string.Empty, varDate1 = string.Empty, varDate2 = string.Empty, varPause = string.Empty;
                    DateTime result = DateTime.MinValue;
                    while ((sLine = sReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(sLine) && sLine.Contains("Start Datum/Uhrzeit") && sLine.Contains("End Datum/Uhrzeit") && sLine.Contains("Pause"))
                        {
                            string[] splitted = sLine.Split(';', StringSplitOptions.TrimEntries); //MessageBox.Show(string.Join(Environment.NewLine, splitted));
                            for (int i = 0; i < splitted.Length; i++)
                            {
                                if (splitted[i] == "Start Datum/Uhrzeit") { intVon = i; }
                                else if (splitted[i] == "End Datum/Uhrzeit") { intBis = i; }
                                else if (splitted[i] == "Pause") { intPause = i; }
                            }
                        }
                        else if (!string.IsNullOrEmpty(sLine) && intVon >= 0 && intBis >= 0 && intPause >= 0)
                        {
                            string[] splitted = sLine.Split(';', StringSplitOptions.TrimEntries); //MessageBox.Show(string.Join(Environment.NewLine, splitted));
                            if (!string.IsNullOrEmpty(splitted[intVon])) // Kommen
                            {
                                match = rgxValidDate.Match(splitted[intVon].Split(' ').First()); //[..splitted[intVon].IndexOf(' ')]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varDate1 = result.ToString(dateFormat);
                                }
                                match = rgxImportTime.Match(splitted[intVon].Split(' ').Last()); // [..^3]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varVon = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(splitted[intBis])) // Gehen
                            {
                                match = rgxValidDate.Match(splitted[intBis].Split(' ').First()); //[..splitted[intVon].IndexOf(' ')]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varDate2 = result.ToString(dateFormat);
                                }
                                match = rgxImportTime.Match(splitted[intBis].Split(' ').Last()); // [..^3]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varBis = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(splitted[intPause])) // Pause
                            {
                                match = rgxImportTime.Match(splitted[intPause]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varPause = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(varVon) && !string.IsNullOrEmpty(varBis))
                            {
                                varPause = string.IsNullOrEmpty(varPause) ? "00:00" : varPause;
                                string varDatum = string.IsNullOrEmpty(varDate1) ? "" : (varDate1 == varDate2 ? varDate1 : varDate1.Substring(0, 2) + "/" + varDate2);
                                dGV.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                                index++;
                            }
                            varVon = varBis = varPause = varDate1 = varDate2 = string.Empty;
                        }
                    }
                } //dGV.AllowUserToAddRows = true; // fügt leere Bearbeitungszeile am Ende hinzu
                if (index != 0)
                {
                    UpdateLastTimeColumns();
                    ShowCellChangeInTitle();
                    nothingToSave = false;
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportClipboardTextStempel()
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                int index = 0;
                Regex rgxValidDate = new(@"(\d){2,4}-(0?[1-9]|1[012])-([012]?[0-9]|3[01])", RegexOptions.CultureInvariant);
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);
                    Match match;
                    if (text.Length > 1)
                    {
                        text = text.Replace("\r\n", "\r").Replace("\n", "\r"); //  unify all line breaks to \r
                        string[] textArray = text.Split('\r'); //  create an array of lines
                        int intVon = -1, intBis = -1, intPause = -1;
                        string varVon = string.Empty, varBis = string.Empty, varDate1 = string.Empty, varDate2 = string.Empty, varPause = string.Empty;
                        DateTime result = DateTime.MinValue;
                        foreach (string sLine in textArray)
                        {

                            if (!string.IsNullOrEmpty(sLine) && sLine.Contains("Start Datum/Uhrzeit") && sLine.Contains("End Datum/Uhrzeit") && sLine.Contains("Pause"))
                            {
                                string[] splitted = sLine.Split(';', StringSplitOptions.TrimEntries); //MessageBox.Show(string.Join(Environment.NewLine, splitted));
                                for (int i = 0; i < splitted.Length; i++)
                                {
                                    if (splitted[i] == "Start Datum/Uhrzeit") { intVon = i; }
                                    else if (splitted[i] == "End Datum/Uhrzeit") { intBis = i; }
                                    else if (splitted[i] == "Pause") { intPause = i; }
                                }
                            }
                            else if (!string.IsNullOrEmpty(sLine) && intVon >= 0 && intBis >= 0 && intPause >= 0)
                            {
                                string[] splitted = sLine.Split(';', StringSplitOptions.TrimEntries); //MessageBox.Show(string.Join(Environment.NewLine, splitted));
                                if (!string.IsNullOrEmpty(splitted[intVon])) // Kommen
                                {
                                    match = rgxValidDate.Match(splitted[intVon].Split(' ').First()); //[..splitted[intVon].IndexOf(' ')]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varDate1 = result.ToString(dateFormat);
                                    }
                                    match = rgxImportTime.Match(splitted[intVon].Split(' ').Last()); // [..^3]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varVon = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(splitted[intBis])) // Gehen
                                {
                                    match = rgxValidDate.Match(splitted[intBis].Split(' ').First()); //[..splitted[intVon].IndexOf(' ')]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varDate2 = result.ToString(dateFormat);
                                    }
                                    match = rgxImportTime.Match(splitted[intBis].Split(' ').Last()); // [..^3]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varBis = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(splitted[intPause])) // Pause
                                {
                                    match = rgxImportTime.Match(splitted[intPause]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varPause = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(varVon) && !string.IsNullOrEmpty(varBis))
                                {
                                    varPause = string.IsNullOrEmpty(varPause) ? "00:00" : varPause;
                                    string varDatum = string.IsNullOrEmpty(varDate1) ? "" : (varDate1 == varDate2 ? varDate1 : varDate1.Substring(0, 2) + "/" + varDate2);
                                    dGV.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                                    index++;
                                }
                                varVon = varBis = varPause = varDate1 = varDate2 = string.Empty;
                            }
                        }
                    }
                    if (index != 0)
                    {
                        UpdateLastTimeColumns();
                        ShowCellChangeInTitle();
                        nothingToSave = false;
                        return true;
                    }
                    else { return false; }
                }
                else { MessageBox.Show("Die Zwischenablage enhält keinen Text!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); return true; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportTextFileFLEX(string fullFilename)
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                int index = 0;
                using (StreamReader sReader = new(fullFilename))
                {
                    string sLine;
                    Match match;
                    string varVon = string.Empty, varBis = string.Empty, varDatum = string.Empty, varPause = string.Empty;
                    DateTime result = DateTime.MinValue;
                    char[] separators = new char[] { ';', ';', '/', '\t' };
                    while ((sLine = sReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(sLine) && sLine[..sLine.IndexOf(';')] is "Montag" or "Dienstag" or "Mittwoch" or "Donnerstag" or "Freitag" or "Samstag" or "Sonntag")
                        {
                            string[] splitted = sLine.Split(separators, StringSplitOptions.TrimEntries); //MessageBox.Show(string.Join(Environment.NewLine, splitted));
                            if (!string.IsNullOrEmpty(splitted[1])) // Datum
                            {
                                match = rgxImportDate.Match(splitted[1]); // Regex.Match(sLine, @"(?<=([^;]*;){2,2})[\d:]+(?=\s)").Value
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varDatum = result.ToString(dateFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(splitted[2])) // Kommen
                            {
                                match = rgxImportTime.Match(splitted[2]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varVon = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(splitted[3])) // Gehen
                            {
                                match = rgxImportTime.Match(splitted[3]);
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varBis = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(splitted[7])) // Pause
                            {
                                //if (Regex.Match(splitted[7].Trim(), @"(\d+\.\d{2})").Success) { splitted[7] = TimeSpan.FromHours(Convert.ToDouble(splitted[7].Trim())).ToString("h\\:mm"); }
                                //MessageBox.Show(splitted[7]);
                                match = rgxImportTime.Match(splitted[7].Replace(" Std ", ":").Replace("Min", ""));
                                if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                {
                                    varPause = result.ToString(timeFormat);
                                }
                            }
                            if (!string.IsNullOrEmpty(varVon) && !string.IsNullOrEmpty(varBis))
                            {
                                varPause = string.IsNullOrEmpty(varPause) ? "00:00" : varPause;
                                dGV.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                                index++;
                            }
                            varVon = varBis = varPause = varDatum = string.Empty;
                        }
                    }
                } //dGV.AllowUserToAddRows = true; // fügt leere Bearbeitungszeile am Ende hinzu
                if (index != 0)
                {
                    UpdateLastTimeColumns();
                    ShowCellChangeInTitle();
                    nothingToSave = false;
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportClipboardTextFLEX()
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                int index = 0;
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);
                    Match match;
                    if (text.Length > 1)
                    {
                        text = text.Replace("\r\n", "\r").Replace("\n", "\r"); //  unify all line breaks to \r
                        string[] textArray = text.Split('\r'); //  create an array of lines
                        string varVon = string.Empty, varBis = string.Empty, varDatum = string.Empty, varPause = string.Empty;
                        DateTime result = DateTime.MinValue;
                        char[] separators = new char[] { ';', ';', '/', '\t' };
                        foreach (string sLine in textArray)
                        {
                            if (!string.IsNullOrEmpty(sLine) && sLine[..sLine.IndexOf(';')] is "Montag" or "Dienstag" or "Mittwoch" or "Donnerstag" or "Freitag" or "Samstag" or "Sonntag")
                            {
                                string[] splitted = sLine.Split(separators, StringSplitOptions.TrimEntries);
                                if (!string.IsNullOrEmpty(splitted[1])) // Datum
                                {
                                    match = rgxImportDate.Match(splitted[1]); // Regex.Match(sLine, @"(?<=([^;]*;){2,2})[\d:]+(?=\s)").Value
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varDatum = result.ToString(dateFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(splitted[2])) // Kommen
                                {
                                    match = rgxImportTime.Match(splitted[2]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varVon = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(splitted[3])) // Gehen
                                {
                                    match = rgxImportTime.Match(splitted[3]);
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varBis = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(splitted[7])) // Pause
                                {
                                    //if (Regex.Match(splitted[7], @"(.*\.\d{2})").Success) { splitted[7] = TimeSpan.FromHours(Convert.ToDouble(splitted[7].Trim())).ToString("h\\:mm"); }
                                    match = rgxImportTime.Match(splitted[7].Replace(" Std ", ":").Replace("Min", ""));
                                    if (match.Success && DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varPause = result.ToString(timeFormat);
                                    }
                                }
                                if (!string.IsNullOrEmpty(varVon) && !string.IsNullOrEmpty(varBis))
                                {
                                    varPause = string.IsNullOrEmpty(varPause) ? "00:00" : varPause; // 0.ToString("00:00") 
                                    dGV.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                                    index++;
                                }
                                varVon = varBis = varPause = varDatum = string.Empty;
                            }
                        }
                    }
                    if (index != 0)
                    {
                        UpdateLastTimeColumns();
                        ShowCellChangeInTitle();
                        nothingToSave = false;
                        return true;
                    }
                    else { return false; }
                }
                else { MessageBox.Show("Die Zwischenablage enhält keinen Text!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); return true; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportTextFileBMAS(string fullFilename)
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                int index = 0;

                DataTable dt = new();
                for (int i = 0; i < dGV.ColumnCount; ++i)
                {
                    dt.Columns.Add(new DataColumn(dGV.Columns[i].Name));
                    dGV.Columns[i].DataPropertyName = dGV.Columns[i].Name;
                }

                using (StreamReader sReader = new(fullFilename))
                {
                    string sLine; Match match;
                    string varVon = string.Empty, varBis = string.Empty, varDatum = string.Empty, varPause = string.Empty;
                    bool newBlock = false;
                    DateTime result = DateTime.MinValue;
                    while ((sLine = sReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(sLine) && sLine.Contains("Datum:"))
                        {
                            newBlock = true;
                            match = rgxImportDate.Match(sLine);
                            if (match.Success)
                            {
                                if (DateTime.TryParse(match.ToString(), out result))
                                {
                                    varDatum = result.ToString(dateFormat);
                                }
                            }
                        }
                        else if (newBlock && !string.IsNullOrEmpty(sLine) && sLine.Contains("Arbeitsbeginn:"))
                        {
                            match = rgxImportTime.Match(sLine);
                            if (match.Success)
                            {
                                if (DateTime.TryParse(match.ToString(), out result))
                                {
                                    varVon = result.ToString(timeFormat);
                                }
                            }
                        }
                        else if (newBlock && !string.IsNullOrEmpty(sLine) && sLine.Contains("Arbeitsende:"))
                        {
                            match = rgxImportTime.Match(sLine);
                            if (match.Success)
                            {
                                if (DateTime.TryParse(match.ToString(), out result))
                                {
                                    varBis = result.ToString(timeFormat);
                                }
                            }
                        }
                        else if (newBlock && !string.IsNullOrEmpty(sLine) && sLine.Contains("Pausendauer:"))
                        {// MessageBox.Show(varVon + "|" + varBis + "|" + varDatum);
                            match = rgxImportTime.Match(sLine);
                            if (match.Success)
                            {
                                if (DateTime.TryParse(match.ToString(), out result))
                                {
                                    varPause = result.ToString(timeFormat);
                                }
                            }
                            else { varPause = "00:00"; }
                            dt.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                            //dGV.Rows.Add(varVon, varBis, varPause, "1", varDatum);
                            //UpdateLastTimeColumns();
                            newBlock = false;
                            index++;
                        }
                    }
                } //dGV.AllowUserToAddRows = true; // fügt leere Bearbeitungszeile am Ende hinzu

                dGV.SuspendLayout();
                foreach (DataRow dr in dt.Rows) { dGV.Rows.Add(dr.ItemArray); }
                dGV.ResumeLayout();

                if (index != 0)
                {
                    UpdateLastTimeColumns();
                    ShowCellChangeInTitle();
                    nothingToSave = false;
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private bool ImportClipboardTextBMAS()
        {
            try
            {
                lblWait.Visible = true;
                Application.DoEvents();
                int index = 0;
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);
                    Match match;
                    if (text.Length > 1)
                    {
                        text = text.Replace("\r\n", "\r").Replace("\n", "\r"); //  unify all line breaks to \r
                        string[] textArray = text.Split('\r'); //  create an array of lines
                        string varVon = string.Empty, varBis = string.Empty, varDatum = string.Empty, varPause = string.Empty;
                        DateTime result = DateTime.MinValue;
                        bool newBlock = false;
                        foreach (string sLine in textArray)
                        {
                            if (sLine.Contains("Datum:"))
                            {
                                newBlock = true;
                                match = rgxImportDate.Match(sLine);
                                if (match.Success)
                                {
                                    if (DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varDatum = result.ToString("d.M.yyyy");
                                    }
                                }
                            }
                            else if (newBlock && sLine.Contains("Arbeitsbeginn:"))
                            {
                                match = rgxImportTime.Match(sLine);
                                if (match.Success)
                                {
                                    if (DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varVon = result.ToString("H:mm");
                                    }
                                }
                            }
                            else if (newBlock && sLine.Contains("Arbeitsende:"))
                            {
                                match = rgxImportTime.Match(sLine);
                                if (match.Success)
                                {
                                    if (DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varBis = result.ToString("H:mm");
                                    }
                                }
                            }
                            else if (newBlock && sLine.Contains("Pausendauer:"))
                            {// MessageBox.Show(varVon + "|" + varBis + "|" + varDatum);
                                match = rgxImportTime.Match(sLine);
                                if (match.Success)
                                {
                                    if (DateTime.TryParse(match.ToString(), out result))
                                    {
                                        varPause = result.ToString("H:mm");
                                    }
                                }
                                else { varPause = "00:00"; }
                                dGV.Rows.Add(varDatum, varVon, varBis, varPause, "1");
                                newBlock = false;
                                index++;
                            }
                        }
                    }
                    if (index != 0)
                    {
                        UpdateLastTimeColumns();
                        ShowCellChangeInTitle();
                        nothingToSave = false;
                        return true;
                    }
                    else { return false; }
                }
                else { MessageBox.Show("Die Zwischenablage enhält keinen Text!", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); return true; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { lblWait.Visible = false; }
            return true;
        }

        private void DGV_SizeChanged(object sender, EventArgs e)
        {
            float scale = Size.Width / (float)MinimumSize.Width;
            dGV.DefaultCellStyle.Font = new(defaultCellStyleFontName, 10F * scale, GraphicsUnit.Point);
            dGV.ColumnHeadersDefaultCellStyle.Font = new(dGV.Font.FontFamily, 10F * scale, GraphicsUnit.Point);
            dGV.RowHeadersWidth = (int)(rowHeadersWidth * scale);
        }

        private void AnalyzeMonthData()
        {
            if (ClsUtilities.IsDGVEmpty(dGV)) { MessageBox.Show("Keine Daten vorhanden.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            else
            {
                DataTable dt = new();
                dt.Columns.Add("Datum", typeof(DateTime));
                dt.Columns.Add("Spanne", typeof(int)); ;
                for (int row = 0; row < dGV.RowCount - 1; row++)
                {
                    if (DateTime.TryParse(dGV.Rows[row].Cells[0].Value.ToString(), out DateTime myDate) && TimeSpan.TryParse(dGV.Rows[row].Cells[5].Value.ToString(), out TimeSpan myTime))
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = myDate;
                        dr[1] = (int)myTime.TotalMinutes;
                        dt.Rows.Add(dr);
                    }
                }
                var monthQuery = dt.Rows.Cast<DataRow>().GroupBy(r => ((DateTime)r[0]).Month).Select(g => new
                {
                    g.Key,
                    Sum = g.Sum(r => (int)r[1]),
                    Average = g.Average(r => (int)r[1]),
                });

                string result = string.Empty;
                TimeSpan total = TimeSpan.Zero;
                int totalMinutes = 0;
                foreach (var v in monthQuery)
                {
                    totalMinutes += v.Sum;
                    TimeSpan summary = TimeSpan.FromMinutes(v.Sum);
                    TimeSpan average = TimeSpan.FromMinutes(v.Average);
                    total = total.Add(summary);
                    result += CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(v.Key).PadRight(14) + " \t" +
                        string.Format("{0:00}:{1:00}", summary.TotalHours, summary.Minutes) + " \t\t" +
                        string.Format("{0:00}:{1:00}", average.TotalHours, average.Minutes) + Environment.NewLine;
                }
                TimeSpan totalMinAverage = TimeSpan.FromMinutes(totalMinutes / monthQuery.Count());
                using QuerySummary ReportForm = new();
                ReportForm.WSTextBox.Text = "Monat\t\tSumme \t\tDurchschnitt" + Environment.NewLine + Environment.NewLine + result.TrimEnd(Environment.NewLine.ToCharArray());
                ReportForm.WSLabel.Text = "Durchschnittliche Arbeitszeit pro Monat: " + string.Format("{0:00}:{1:00}", totalMinAverage.TotalHours, totalMinAverage.Minutes);
                ReportForm.Text = "Auswertung nach Monaten (" + monthQuery.Count() + ")";
                ReportForm.ShowDialog();
            }
        }

        private void AnalyzeWeekData()
        {
            if (ClsUtilities.IsDGVEmpty(dGV)) { MessageBox.Show("Keine Daten vorhanden.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            else
            {
                bool isNotOrdered = false;
                for (int i = 1; i < dGV.RowCount - 1; ++i)
                {
                    if (new RowComparer(SortOrder.Ascending).Compare(dGV.Rows[i - 1], dGV.Rows[i]) > 0) // IsOrderedDescending could be implemented easily by changing > 0 to < 0.
                    {
                        isNotOrdered = true;
                        break;
                    }
                }
                if (isNotOrdered)
                {
                    if (MessageBox.Show("Bevor die Statistik angezeigt werden kann,\nmüssen die Daten sortiert werden.\n\nMöchten Sie jetzt aufsteigend sortieren?", winTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { dGV.Sort(new RowComparer(SortOrder.Ascending)); }
                    else { return; }
                }
                DataTable dt = new();
                dt.Columns.Add("Datum", typeof(DateTime));
                dt.Columns.Add("Spanne", typeof(int)); ;
                for (int row = 0; row < dGV.RowCount - 1; row++)
                {
                    if (DateTime.TryParse(dGV.Rows[row].Cells[0].Value.ToString(), out DateTime myDate) && TimeSpan.TryParse(dGV.Rows[row].Cells[5].Value.ToString(), out TimeSpan myTime))
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = myDate;
                        dr[1] = (int)myTime.TotalMinutes;
                        dt.Rows.Add(dr);
                    }
                }
                var weekQuery = dt.Rows.Cast<DataRow>().GroupBy(row =>
                  ISOWeek.GetWeekOfYear(Convert.ToDateTime((DateTime)row[0]))).Select(group => new
                  {
                      group.Key,
                      Date = group.First()[0].ToString(),
                      Sum = group.Sum(r => (int)r[1]),
                      Average = group.Average(r => (int)r[1]),
                  });
                string result = string.Empty;
                TimeSpan total = TimeSpan.Zero;
                int totalMinutes = 0;
                foreach (var v in weekQuery)
                {
                    int year = Convert.ToDateTime(v.Date).Year;
                    DateTime foo = ISOWeek.ToDateTime(year, v.Key, DayOfWeek.Monday);
                    DateTime bar = ISOWeek.ToDateTime(year, v.Key, DayOfWeek.Sunday);

                    totalMinutes += v.Sum;
                    TimeSpan summary = TimeSpan.FromMinutes(v.Sum);
                    TimeSpan average = TimeSpan.FromMinutes(v.Average);
                    total = total.Add(summary);
                    result += (v.Key.ToString() + " (" + string.Format("{0:d/M.}", foo) + "-" + string.Format("{0:d/M.}", bar) + ")").PadRight(14) + " \t" +
                        string.Format("{0:00}:{1:00}", summary.TotalHours, summary.Minutes) + " \t\t" +
                        string.Format("{0:00}:{1:00}", average.TotalHours, average.Minutes) + Environment.NewLine;
                }
                TimeSpan totalMinAverage = TimeSpan.FromMinutes(totalMinutes / weekQuery.Count());
                using QuerySummary ReportForm = new();
                ReportForm.WSTextBox.Text = "Woche\t\tSumme \t\tDurchschnitt" + Environment.NewLine + Environment.NewLine + result.TrimEnd(Environment.NewLine.ToCharArray());
                ReportForm.WSLabel.Text = "Durchschnittliche Arbeitszeit pro Woche: " + string.Format("{0:00}:{1:00}", totalMinAverage.TotalHours, totalMinAverage.Minutes);
                ReportForm.Text = "Auswertung nach Wochen (" + weekQuery.Count() + ")";
                ReportForm.ShowDialog();
            }
        }

        private void SortByDateToolStripMenuItem_Click(object sender, EventArgs e) { dGV.Sort(new RowComparer(SortOrder.Ascending)); }

        private void DescendingToolStripMenuItem_Click(object sender, EventArgs e) { dGV.Sort(new RowComparer(SortOrder.Descending)); }

        private void FindDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int matches = 0;
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                DataGridViewRow current = dGV.Rows[i];
                string[] abc = new string[] { current.Cells[0].Value.ToString(), current.Cells[1].Value.ToString(), current.Cells[2].Value.ToString(), current.Cells[3].Value.ToString(), current.Cells[4].Value.ToString() };
                for (int j = i + 1; j < dGV.RowCount; j++)
                {
                    DataGridViewRow compare = dGV.Rows[j];
                    if (compare.Cells[0].Value != null && compare.Cells[1].Value != null && compare.Cells[2].Value != null && compare.Cells[3].Value != null && compare.Cells[4].Value != null)
                    {
                        string[] def = new string[] { compare.Cells[0].Value.ToString(), compare.Cells[1].Value.ToString(), compare.Cells[2].Value.ToString(), compare.Cells[3].Value.ToString(), compare.Cells[4].Value.ToString() };
                        if (abc.SequenceEqual(def))
                        {
                            dGV.Rows.Remove(compare);
                            matches++;
                            j--;
                        }
                    }
                }
            }
            if (matches > 0) { MessageBox.Show("Es wurd" + (matches > 1 ? "en " + matches + " Zeilen" : "e " + matches + " Zeile") + " entfernt.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            else { MessageBox.Show("Es wurden keine Duplikate gefunden.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private void CombineDaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblWait.Visible = true;
            Application.DoEvents();

            int matches = 0;
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                try
                {
                    DataGridViewRow current = dGV.Rows[i];
                    for (int j = i + 1; j < dGV.RowCount; j++)
                    {
                        DataGridViewRow compare = dGV.Rows[j];
                        if (current.Cells[0].Value != null && current.Cells[1].Value != null && current.Cells[2].Value != null &&
                            compare.Cells[0].Value != null && compare.Cells[1].Value != null && compare.Cells[2].Value != null &&
                            current.Cells[0].Value.ToString() == compare.Cells[0].Value.ToString() &&
                            DateTime.TryParse(current.Cells[1].Value.ToString(), out DateTime currentStart) &&
                            DateTime.TryParse(current.Cells[2].Value.ToString(), out DateTime currentStopp) &&
                            DateTime.TryParse(compare.Cells[1].Value.ToString(), out DateTime compareStart) &&
                            DateTime.TryParse(compare.Cells[2].Value.ToString(), out DateTime compareStopp))
                        {
                            TimeSpan currentPause = DateTime.ParseExact(current.Cells[3].Value.ToString(), timeFormat, deCulture).TimeOfDay; // vorhandene Pause
                            TimeSpan comparePause = DateTime.ParseExact(compare.Cells[3].Value.ToString(), timeFormat, deCulture).TimeOfDay; // vorhandene Pause
                            TimeSpan completePause = currentPause + comparePause;

                            if (currentStart < compareStart)
                            {
                                current.Cells[2].Value = compareStopp.ToString(timeFormat); // Ende (Cells[2]) nach hinten verlegen
                                if (compareStart > currentStopp) { completePause = (compareStart - currentStopp).Add(completePause); }
                            }
                            else if (currentStart == compareStart)
                            {
                                current.Cells[2].Value = compareStopp > currentStopp ? compareStopp.ToString(timeFormat) : currentStopp.ToString(timeFormat);
                            }
                            else if (currentStart > compareStart)
                            {
                                current.Cells[1].Value = compareStart.ToString(timeFormat); // Start (Cells[1]) nach vorn verlegen
                                if (currentStart > compareStopp) { completePause = (currentStart - compareStopp).Add(completePause); }
                            }
                            current.Cells[3].Value = string.Format("{0:00}:{1:00}", completePause.Days * 24 + completePause.Hours, completePause.Minutes); // 24:00 ist keine gültige Zeitangabe! => hours = ts.Days * 24 + ts.Hours
                            dGV.Rows.Remove(compare);
                            matches++;
                            j--;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Fehlermeldung", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { lblWait.Visible = false; }
            }
            UpdateLastTimeColumns();
            lblWait.Visible = false;
            if (matches > 0) { MessageBox.Show("Es wurd" + (matches > 1 ? "en " + matches + " Zeilen" : "e " + matches + " Zeile") + " in eine andere Zeile integriert.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            else { MessageBox.Show("Es wurden keine Tagesmehrfacheinträge gefunden.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private void BreaksAutoFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> _9Indices = new();
            List<int> _6Indices = new();
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                if (dGV.Rows[i].Cells[3].Value.ToString() == "00:00" && dGV.Rows[i].Cells[5].Value != null && TimeSpan.TryParse(dGV.Rows[i].Cells[5].Value.ToString(), out TimeSpan _9Time) && _9Time > TimeSpan.FromHours(9)) { _9Indices.Add(i); }
                else if (dGV.Rows[i].Cells[3].Value.ToString() == "00:00" && dGV.Rows[i].Cells[5].Value != null && TimeSpan.TryParse(dGV.Rows[i].Cells[5].Value.ToString(), out TimeSpan _6Time) && _6Time > TimeSpan.FromHours(6)) { _6Indices.Add(i); }
            }
            if ((_9Indices.Count > 0 || _6Indices.Count > 0) && MessageBox.Show("Bei einer Arbeitszeit > 6 Stunden ist eine 30-Minuten-Pause verpflichtend. Bei einer Arbeitszeit > 9 Stunden: 45 Minuten.\n\nIn " + (_9Indices.Count + _6Indices.Count) + " Zeile(n) fehlt eine entsprechende Pause.\nSoll diese jetzt hinzufügt werden?", winTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                foreach (int j in _9Indices) { dGV.Rows[j].Cells[3].Value = "00:45"; }
                foreach (int j in _6Indices) { dGV.Rows[j].Cells[3].Value = "00:30"; }
                UpdateLastTimeColumns();
            }
            else if (!_9Indices.Any() & !_6Indices.Any()) { MessageBox.Show("Bei einer Arbeitszeit > 6 Stunden ist eine 30-Minuten-Pause verpflichtend. Bei einer Arbeitszeit > 9 Stunden: 45 Minuten.\n\nDiese Tabelle enhält bereits alle erforderliche Pausen!", winTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information); }

        }

        private void DGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) { dGV.FirstDisplayedScrollingRowIndex = dGV.Rows[^1].Index; }

        private void DGV_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dGV.Rows[e.RowIndex].IsNewRow && dGV.RowCount > 1)
            {
                int row = dGV.RowCount - 2;
                if ((dGV.Rows[row].Cells[0].Value == null || dGV.Rows[row].Cells[0].Value.ToString() == "") && (dGV.Rows[row].Cells[1].Value == null || dGV.Rows[row].Cells[1].Value.ToString() == ""))
                {
                    BeginInvoke(new Action(delegate { dGV.Rows.RemoveAt(row); })); // Operation cannot be performed in this event handler
                }
            }
        }

        private void ShowStatisticsMenuItem_Click(object sender, EventArgs e) { AnalyzeMonthData(); }

        private void AbsencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Absences AbsencesForm = new();
            if (dtDays != null && dtDays.Rows.Count > 0)
            {
                for (int i = 0; i < dtDays.Rows.Count; i++)
                {
                    AbsencesForm.dgv.Rows[i].SetValues(new object[] { dtDays.Rows[i][0], dtDays.Rows[i][1], dtDays.Rows[i][2] });
                }
            }
            if (AbsencesForm.ShowDialog() == DialogResult.OK)
            {
                dtDays = new DataTable("Days");
                for (int i = 0; i < AbsencesForm.dgv.ColumnCount; ++i)
                {
                    dtDays.Columns.Add(new DataColumn(AbsencesForm.dgv.Columns[i].Name));
                    AbsencesForm.dgv.Columns[i].DataPropertyName = AbsencesForm.dgv.Columns[i].Name;
                }
                for (int row = 0; row < AbsencesForm.dgv.RowCount; row++)
                {
                    DataRow dr = dtDays.NewRow();
                    for (int col = 0; col < AbsencesForm.dgv.Columns.Count; col++) { dr[col] = AbsencesForm.dgv.Rows[row].Cells[col].Value != null ? AbsencesForm.dgv.Rows[row].Cells[col].Value.ToString() : ""; }
                    dtDays.Rows.Add(dr);
                }
                ShowCellChangeInTitle();
            }
        }

        private void IntroductionMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string readmePdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TimeCalc.pdf");
                if (File.Exists(readmePdf))
                {
                    ProcessStartInfo psi = new(readmePdf) { UseShellExecute = true };
                    Process.Start(psi);
                }
                else { MessageBox.Show("'" + readmePdf + "'\nwurde nicht gefunden.", winTitle, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException) { MessageBox.Show(ex.Message, winTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

    }
}

