using System.Globalization;

namespace TimeCalc;

public partial class Absences : Form
{
    internal DataGridView Dgv
    {
        get => dataGridView;
        set => dataGridView = value;
    } // Control als Eigenschaft offenlegen (DataGridView in diesem Formular!):

    public Absences()
    {
        InitializeComponent();
        dataGridView.Rows.Add(12);
        for (var i = 0; i < dataGridView.Rows.Count; i++)
        {
            dataGridView.Rows[i].HeaderCell.Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i + 1);
        }
    }

    private void Absences_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; }
    }
}
