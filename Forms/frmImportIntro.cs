namespace TimeCalc
{
    public partial class FrmImportIntro : Form
    {

        internal OpenFileDialog ImportForm_fileDialog { get { return importFileDialog; } } // Control als Eigenschaft offenlegen (OpenFileDialog in diesem Formular!):

        internal bool impFromFile = false;
        internal int impIndex = 0;

        public FrmImportIntro()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 0; // 0: BMAS, 1: Flexlog, 2: StempelUhr
        }

        private void BtnFileImport_Click(object sender, EventArgs e)
        {
            impFromFile = true;
            if (impIndex > 0) // 0: BMAS
            {
                importFileDialog.DefaultExt = "*.csv";
                importFileDialog.Filter = "CSV-Dateien (*.csv)|*.csv|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            }
            else
            {
                importFileDialog.DefaultExt = "*.txt";
                importFileDialog.Filter = "Textdateien (*.txt)|*.txt|CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*";
            }
            if (importFileDialog.ShowDialog() == DialogResult.OK) { DialogResult = DialogResult.OK; }
        }

        private void BtnClipboardImport_Click(object sender, EventArgs e)
        {
            impFromFile = false;
            DialogResult = DialogResult.OK;
        }

        private void FrmImportIntro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            impIndex = (sender as ComboBox).SelectedIndex; // impFlexLog = ((sender as ComboBox).SelectedItem as string).StartsWith("FlexLog");
        }

    }
}
