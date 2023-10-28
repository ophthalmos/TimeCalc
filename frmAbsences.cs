using System.Globalization;

namespace TimeCalc
{
    public partial class Absences : Form
    {
        internal DataGridView dgv { get { return dataGridView; } set { dataGridView = value; } } // Control als Eigenschaft offenlegen (DataGridView in diesem Formular!):

        public Absences()
        {
            InitializeComponent();
            dataGridView.Rows.Add(12);
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i + 1);
            }
        }

        private void Absences_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; }
        }
    }
}
