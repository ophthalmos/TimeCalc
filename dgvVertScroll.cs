namespace TimeCalc
{
    public class DgvVertScroll : DataGridView
    {

        //bool isFirst = true;

        //        public static bool IsUnix
        //        {
        ///*
        //  The execution platform can be detected by using the System.Environment.OSVersion.Platform value. However
        //  correctly detecting Unix platforms, in every cases, requires a little more work. The first versions of
        //  the framework (1.0 and 1.1) didn't include any PlatformID value for Unix, so Mono used the value 128.
        //  The newer framework 2.0 added Unix to the PlatformID enum but, sadly, with a different value: 4 and
        //  newer versions of .NET distinguished between Unix and MacOS X, introducing yet another value 6 for MacOS X.
        //  This means that in order to detect properly code running on Unix platforms you must check the three values (4, 6 and 128).
        //  This ensure that the detection code will work as expected when executed on Mono CLR 1.x runtime and with both Mono and Microsoft CLR 2.x runtimes.
        //*/
        //            get
        //            {
        //                int p = (int)Environment.OSVersion.Platform;
        //                return (p == 4) || (p == 6) || (p == 128);
        //            }
        //        }    



        public DgvVertScroll()
        {
            //VerticalScrollBar.Visible = true;
            //VerticalScrollBar.VisibleChanged += new EventHandler(ShowScrollBars);
            DefaultCellStyle.SelectionBackColor = Color.FromArgb(109, 158, 218); //(214, 223, 242);
        }

        private void ShowScrollBars(object sender, EventArgs e)
        {
            if (!VerticalScrollBar.Visible)
            {
                int width = VerticalScrollBar.Width;
                //if (isFirst)
                //{
                //    Width -= width;
                //    isFirst = false;
                //}
                VerticalScrollBar.Location = new Point(ClientRectangle.Width - width - 1, 1);
                VerticalScrollBar.Size = new Size(width, ClientRectangle.Height - 1);
                VerticalScrollBar.Show();
             //   AutoResizeColumns();

                //MessageBox.Show(width.ToString());
                //SuspendLayout();
                //foreach (DataGridViewColumn c in Columns)
                //{
                //    c.Width -= 4;
                //}
                //ResumeLayout();
            }
        }

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);
            string rowNum = (e.RowIndex + 1).ToString() + ".";
            SizeF Sz = e.Graphics.MeasureString(rowNum, e.InheritedRowStyle.Font);
            // adjust the width of the column that contains the row header cells 
            if (RowHeadersWidth < (int)(Sz.Width + 20)) { RowHeadersWidth = (int)(Sz.Width + 20); }
            var centerFormat = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, RowHeadersWidth, e.RowBounds.Height);
            using SolidBrush sBrush = new(RowHeadersDefaultCellStyle.ForeColor);
            // The using statement automatically disposes the brush.
            e.Graphics.DrawString(rowNum, e.InheritedRowStyle.Font, sBrush, headerBounds, centerFormat);
        }


        //protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        this.ProcessTabKey(e.KeyData);
        //        return true;
        //    }
        //    return base.ProcessDataGridViewKey(e);
        //}

        //protected override bool ProcessDialogKey(Keys keyData)
        //{
        //    if (keyData == Keys.Enter)
        //    {
        //        this.ProcessTabKey(keyData);
        //        return true;
        //    }
        //    return base.ProcessDialogKey(keyData);
        //}
    }
}