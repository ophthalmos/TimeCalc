namespace TimeCalc
{
    partial class FrmImportIntro
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmImportIntro));
            lblImportInfo = new Label();
            btnFileImport = new Button();
            btnClipboardImport = new Button();
            importFileDialog = new OpenFileDialog();
            lblFirstStep = new Label();
            label1 = new Label();
            panelImport = new Panel();
            panelFormat = new Panel();
            comboBox = new ComboBox();
            panelImport.SuspendLayout();
            panelFormat.SuspendLayout();
            SuspendLayout();
            // 
            // lblImportInfo
            // 
            lblImportInfo.BackColor = SystemColors.ControlLightLight;
            lblImportInfo.Dock = DockStyle.Top;
            lblImportInfo.Location = new Point(0, 0);
            lblImportInfo.Margin = new Padding(4, 0, 4, 0);
            lblImportInfo.Name = "lblImportInfo";
            lblImportInfo.Padding = new Padding(4, 3, 2, 2);
            lblImportInfo.Size = new Size(330, 98);
            lblImportInfo.TabIndex = 0;
            lblImportInfo.Text = resources.GetString("lblImportInfo.Text");
            // 
            // btnFileImport
            // 
            btnFileImport.Location = new Point(8, 12);
            btnFileImport.Margin = new Padding(4, 3, 4, 3);
            btnFileImport.Name = "btnFileImport";
            btnFileImport.Size = new Size(153, 32);
            btnFileImport.TabIndex = 1;
            btnFileImport.Text = "Aus Datei ...";
            btnFileImport.UseVisualStyleBackColor = true;
            btnFileImport.Click += BtnFileImport_Click;
            // 
            // btnClipboardImport
            // 
            btnClipboardImport.Location = new Point(169, 12);
            btnClipboardImport.Margin = new Padding(4, 3, 4, 3);
            btnClipboardImport.Name = "btnClipboardImport";
            btnClipboardImport.Size = new Size(153, 32);
            btnClipboardImport.TabIndex = 2;
            btnClipboardImport.Text = "Text aus Zwischenablage";
            btnClipboardImport.UseVisualStyleBackColor = true;
            btnClipboardImport.Click += BtnClipboardImport_Click;
            // 
            // importFileDialog
            // 
            importFileDialog.DefaultExt = "*.csv";
            importFileDialog.Filter = "CSV-Datei (*.csv)|*.csv|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            importFileDialog.InitialDirectory = "Environment.SpecialFolder.Downloads";
            importFileDialog.RestoreDirectory = true;
            // 
            // lblFirstStep
            // 
            lblFirstStep.AutoSize = true;
            lblFirstStep.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblFirstStep.Location = new Point(8, 109);
            lblFirstStep.Name = "lblFirstStep";
            lblFirstStep.Size = new Size(272, 19);
            lblFirstStep.TabIndex = 13;
            lblFirstStep.Text = "1. Schritt: Wählen Sie das Importformat";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(8, 200);
            label1.Name = "label1";
            label1.Size = new Size(284, 19);
            label1.TabIndex = 14;
            label1.Text = "2. Schritt: Wählen Sie die Importmethode";
            // 
            // panelImport
            // 
            panelImport.BackColor = SystemColors.ControlLightLight;
            panelImport.Controls.Add(btnClipboardImport);
            panelImport.Controls.Add(btnFileImport);
            panelImport.Dock = DockStyle.Bottom;
            panelImport.Location = new Point(0, 222);
            panelImport.Name = "panelImport";
            panelImport.Size = new Size(330, 57);
            panelImport.TabIndex = 15;
            // 
            // panelFormat
            // 
            panelFormat.BackColor = SystemColors.ControlLightLight;
            panelFormat.Controls.Add(comboBox);
            panelFormat.Location = new Point(0, 131);
            panelFormat.Name = "panelFormat";
            panelFormat.Size = new Size(330, 55);
            panelFormat.TabIndex = 16;
            // 
            // comboBox
            // 
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            comboBox.FormattingEnabled = true;
            comboBox.Items.AddRange(new object[] { "BMAS-App \"einfach erfasst\"", "FlexLog - Arbeitszeiterfassung", "StempelUhr (iOS-App)", "TimeCalc Version 1.x (csv)" });
            comboBox.Location = new Point(8, 15);
            comboBox.Name = "comboBox";
            comboBox.Size = new Size(314, 25);
            comboBox.TabIndex = 0;
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            // 
            // FrmImportIntro
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(330, 279);
            Controls.Add(panelFormat);
            Controls.Add(panelImport);
            Controls.Add(label1);
            Controls.Add(lblFirstStep);
            Controls.Add(lblImportInfo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmImportIntro";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Datenimport";
            KeyDown += FrmImportIntro_KeyDown;
            panelImport.ResumeLayout(false);
            panelFormat.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblImportInfo;
        private System.Windows.Forms.Button btnFileImport;
        private System.Windows.Forms.Button btnClipboardImport;
        private System.Windows.Forms.OpenFileDialog importFileDialog;
        private Label lblFirstStep;
        private Label label1;
        private Panel panelImport;
        private Panel panelFormat;
        private ComboBox comboBox;
    }
}