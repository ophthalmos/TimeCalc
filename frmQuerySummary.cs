using System.Runtime.InteropServices;

namespace TimeCalc
{
    public partial class QuerySummary : Form
    {
        internal TextBox WSTextBox { get { return textBox; } set { textBox = value; } }
        internal Label WSLabel { get { return label; } set { label = value; } }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        public QuerySummary() { InitializeComponent(); }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            Size textBoxRect = TextRenderer.MeasureText(textBox.Text, textBox.Font, new Size(textBox.Width, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            ShowScrollBar(textBox.Handle, 1, textBoxRect.Height > textBox.Height);
            textBox.BorderStyle = textBoxRect.Height > textBox.Height ? BorderStyle.FixedSingle : BorderStyle.None;
        }
    }
}
