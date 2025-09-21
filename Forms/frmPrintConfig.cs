using System.Drawing.Printing;

namespace TimeCalc;

public partial class FrmPrintConfig : Form
{
    private readonly FrmTimeCalc parentForm;

    public FrmPrintConfig(FrmTimeCalc parentForm)
    {
        this.parentForm = parentForm;
        InitializeComponent();
        foreach (string Printername in PrinterSettings.InstalledPrinters)        {            comboBoxDrucker.Items.Add(Printername);        }

        comboBoxDrucker.Text = parentForm.printDocument.PrinterSettings.PrinterName;
        //MessageBox.Show(parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind.ToString());
        Aktualisieren();
    }

    private void ComboBoxDrucker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Aktualisieren();
    }

    private void Aktualisieren()
    {
        parentForm.printDocument.DefaultPageSettings.PrinterSettings.PrinterName = comboBoxDrucker.Text;

        if (!parentForm.printDocument.PrinterSettings.CanDuplex)
        {
            comboBoxDuplex.SelectedIndex = 0;
            comboBoxDuplex.Enabled = false;
        }
        else
        {
            comboBoxDuplex.SelectedIndex = 1;
            comboBoxDuplex.Enabled = true;
        }

        if (parentForm.printDocument.DefaultPageSettings.Landscape)        {            comboBoxFormat.SelectedIndex = 0;        }
        else        {            comboBoxFormat.SelectedIndex = 1;        }

        comboBoxResolution.Items.Clear();
        foreach (PrinterResolution res in parentForm.printDocument.PrinterSettings.PrinterResolutions)
        {
            if (res.Kind != PrinterResolutionKind.Custom)            {                comboBoxResolution.Items.Add(res.Kind);            }
        }

        comboBoxResolution.SelectedIndex 
            = parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind == PrinterResolutionKind.Custom ? 0 
            : parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind == PrinterResolutionKind.High ? 0 
            : parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind == PrinterResolutionKind.Medium ? 1 
            : parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind == PrinterResolutionKind.Low ? 2 : 3; 
    }

    private void ButtonOK_Click(object sender, EventArgs e)
    {
        parentForm.printDocument.DefaultPageSettings.PrinterSettings.PrinterName = comboBoxDrucker.Text;

        parentForm.printDocument.PrinterSettings.Duplex 
            = comboBoxResolution.SelectedIndex == 2 ? Duplex.Vertical 
            : comboBoxResolution.SelectedIndex == 1 ? Duplex.Horizontal 
            : Duplex.Simplex;

        parentForm.printDocument.DefaultPageSettings.Landscape = comboBoxFormat.SelectedIndex == 0;

        parentForm.printDocument.DefaultPageSettings.PrinterResolution.Kind 
            = comboBoxResolution.SelectedIndex == 0 ? PrinterResolutionKind.High
            : comboBoxResolution.SelectedIndex == 1 ?  PrinterResolutionKind.Medium 
            : comboBoxResolution.SelectedIndex == 2 ? PrinterResolutionKind.Low : PrinterResolutionKind.Draft;

        parentForm.printDocument.PrinterSettings.Copies = Convert.ToInt16(numericUpDownCopies.Value);
        
        parentForm.PrintNowFromChild();
        Close();
    }
        
    private void ButtonCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

}
