using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection; // MethodInfo

namespace TimeCalc
{
    public partial class FrmPrintPreview : Form
    {
        int numPages;
        readonly FrmTimeCalc parentForm;

        public FrmPrintPreview(FrmTimeCalc parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    Close();
                    return true;
                case Keys.P | Keys.Control:
                    _toolStrip.Enabled = false;
                    parentForm.ShowPrintDialog();
                    Close();
                    return true;
                case Keys.Z | Keys.Control:
                    _btnZoom.PerformButtonClick();
                    return true;
                case Keys.S | Keys.Control:
                    _txtStartPage.Focus();
                    _txtStartPage.SelectAll();
                    return true;
                case Keys.Add | Keys.Control:
                case Keys.Oemplus | Keys.Control:
                    Magnify();
                    return true;
                case Keys.OemMinus | Keys.Control:
                case Keys.Subtract | Keys.Control:
                    Reduce();
                    return true;
                case Keys.PageUp:
                case Keys.PageUp | Keys.Control:
                    if (_printPreviewControl.StartPage > 0) { _printPreviewControl.StartPage--; } else { Console.Beep(); } //_btnPrev.PerformClick();
                    return true;
                case Keys.PageDown:
                case Keys.PageDown | Keys.Control:
                    if (_printPreviewControl.StartPage < (numPages - 1)) { _printPreviewControl.StartPage++; } else { Console.Beep(); } //_btnNext.PerformClick();
                    return true;
                case Keys.End | Keys.Control:
                    _btnLast.PerformClick();
                    return true;
                case Keys.Home | Keys.Control:
                    _btnFirst.PerformClick();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FrmPrintPreview_Load(object sender, EventArgs e)
        {
            Type ObjectType = typeof(PrintPreviewControl);
            MethodInfo CalculatePageInfo = ObjectType.GetMethod("CalculatePageInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            CalculatePageInfo.Invoke(this._printPreviewControl, null);
            FieldInfo PageInfo = ObjectType.GetField("pageInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            PreviewPageInfo[] infos = (PreviewPageInfo[])PageInfo.GetValue(this._printPreviewControl);
            numPages = infos.Length;
            _btnPrint.Enabled = (_printPreviewControl.Document != null);
            _lblPageCount.Text = "/ " + numPages;
        }

        private void FrmPrintPreview_Shown(object sender, EventArgs e)
        {
            BringToFront(); // des nicht abstellbaren kleinen Fensters mit dem Namen
            Activate(); // scheint die entscheidende Arbeit zu leisten!!!
        }

        public void BtnPrint_Click(object sender, EventArgs e)
        {
            _toolStrip.Enabled = false;
            parentForm.ShowPrintDialog();
            Close();
        }

        private void BtnZoom_ButtonClick(object sender, EventArgs e)
        {
            if (_printPreviewControl.Zoom < 0.5) { _item50.PerformClick(); }
            else if (_printPreviewControl.Zoom < 1) { _item100.PerformClick(); }
            else if (_printPreviewControl.Zoom < 2) { _item200.PerformClick(); }
            else { _printPreviewControl.AutoZoom = true; }
        }

        private void Magnify()
        {
            if (_printPreviewControl.Zoom < 0.5) { _item50.PerformClick(); }
            else if (_printPreviewControl.Zoom < 1) { _item100.PerformClick(); }
            else if (_printPreviewControl.Zoom < 2) { _item200.PerformClick(); }
            else { Console.Beep(); }
        }

        private void Reduce()
        {
            if (_printPreviewControl.Zoom > 2) { _item200.PerformClick(); }
            else if (_printPreviewControl.Zoom > 1) { _item100.PerformClick(); }
            else if (_printPreviewControl.Zoom > 0.5) { _item50.PerformClick(); }
            else { Console.Beep(); }
        }

        private void BtnZoom_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _itemAuto.Checked = _item200.Checked = _item100.Checked = _item50.Checked = false;
            if (e.ClickedItem == _itemAuto) { _printPreviewControl.AutoZoom = true; }
            else if (e.ClickedItem == _item100) { _printPreviewControl.Zoom = 1; }
            else if (e.ClickedItem == _item200) { _printPreviewControl.Zoom = 2; }
            else if (e.ClickedItem == _item50) { _printPreviewControl.Zoom = .5; }
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            _printPreviewControl.StartPage = 0;
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (_printPreviewControl.StartPage > 0) { _printPreviewControl.StartPage--; } else { Console.Beep(); }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_printPreviewControl.StartPage < (numPages - 1)) { _printPreviewControl.StartPage++; } else { Console.Beep(); }
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            _printPreviewControl.StartPage = numPages - 1; // GetPageCount(_preview.Document);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TxtStartPage_Enter(object sender, EventArgs e)
        {
            _txtStartPage.SelectAll();
        }

        private void TxtStartPage_Validating(object sender, CancelEventArgs e)
        {
            CommitPageNumber();
        }

        private void TxtStartPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            var c = e.KeyChar;
            if (c == (char)13)
            {
                CommitPageNumber();
                e.Handled = true;
            }
            else if (c > ' ' && !char.IsDigit(c))
            {
                e.Handled = true;
            }
        }

        private void CommitPageNumber()
        {
            if (int.TryParse(_txtStartPage.Text, out int page))
            {
                if (page > 0) { _printPreviewControl.StartPage = page - 1; }
                else { _txtStartPage.Text = "1"; _txtStartPage.SelectAll(); } // ist der Fall wenn 0 eingegeben wird
            }
            _txtStartPage.Select(_txtStartPage.Text.Length, 0); // Cursor ans Ende setzten (Markierungslänge = 0)
        }

        private void Preview_StartPageChanged(object sender, EventArgs e)
        {
            var page = _printPreviewControl.StartPage + 1;
            _txtStartPage.Text = page.ToString();
        }
   
    }
}
