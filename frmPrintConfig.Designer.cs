namespace TimeCalc
{
    partial class FrmPrintConfig
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
            this.groupBoxPrinter = new System.Windows.Forms.GroupBox();
            this.numericUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.labelDuplex = new System.Windows.Forms.Label();
            this.comboBoxDuplex = new System.Windows.Forms.ComboBox();
            this.labelResolution = new System.Windows.Forms.Label();
            this.comboBoxResolution = new System.Windows.Forms.ComboBox();
            this.labeExemplare = new System.Windows.Forms.Label();
            this.labelFormat = new System.Windows.Forms.Label();
            this.labelDrucker = new System.Windows.Forms.Label();
            this.comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.comboBoxDrucker = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxPrinter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxPrinter
            // 
            this.groupBoxPrinter.Controls.Add(this.numericUpDownCopies);
            this.groupBoxPrinter.Controls.Add(this.labelDuplex);
            this.groupBoxPrinter.Controls.Add(this.comboBoxDuplex);
            this.groupBoxPrinter.Controls.Add(this.labelResolution);
            this.groupBoxPrinter.Controls.Add(this.comboBoxResolution);
            this.groupBoxPrinter.Controls.Add(this.labeExemplare);
            this.groupBoxPrinter.Controls.Add(this.labelFormat);
            this.groupBoxPrinter.Controls.Add(this.labelDrucker);
            this.groupBoxPrinter.Controls.Add(this.comboBoxFormat);
            this.groupBoxPrinter.Controls.Add(this.comboBoxDrucker);
            this.groupBoxPrinter.Location = new System.Drawing.Point(5, 6);
            this.groupBoxPrinter.Name = "groupBoxPrinter";
            this.groupBoxPrinter.Size = new System.Drawing.Size(252, 159);
            this.groupBoxPrinter.TabIndex = 0;
            this.groupBoxPrinter.TabStop = false;
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.Location = new System.Drawing.Point(149, 128);
            this.numericUpDownCopies.Name = "numericUpDownCopies";
            this.numericUpDownCopies.Size = new System.Drawing.Size(95, 20);
            this.numericUpDownCopies.TabIndex = 10;
            this.numericUpDownCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelDuplex
            // 
            this.labelDuplex.AutoSize = true;
            this.labelDuplex.Location = new System.Drawing.Point(4, 61);
            this.labelDuplex.Name = "labelDuplex";
            this.labelDuplex.Size = new System.Drawing.Size(40, 13);
            this.labelDuplex.TabIndex = 9;
            this.labelDuplex.Text = "Duplex";
            // 
            // comboBoxDuplex
            // 
            this.comboBoxDuplex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDuplex.FormattingEnabled = true;
            this.comboBoxDuplex.Items.AddRange(new object[] {
            "(Ohne)",
            "Lange Seite",
            "Kurze Seite"});
            this.comboBoxDuplex.Location = new System.Drawing.Point(6, 77);
            this.comboBoxDuplex.Name = "comboBoxDuplex";
            this.comboBoxDuplex.Size = new System.Drawing.Size(130, 21);
            this.comboBoxDuplex.TabIndex = 8;
            // 
            // labelResolution
            // 
            this.labelResolution.AutoSize = true;
            this.labelResolution.Location = new System.Drawing.Point(4, 111);
            this.labelResolution.Name = "labelResolution";
            this.labelResolution.Size = new System.Drawing.Size(54, 13);
            this.labelResolution.TabIndex = 7;
            this.labelResolution.Text = "Auflösung";
            // 
            // comboBoxResolution
            // 
            this.comboBoxResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResolution.FormattingEnabled = true;
            this.comboBoxResolution.Location = new System.Drawing.Point(6, 127);
            this.comboBoxResolution.Name = "comboBoxResolution";
            this.comboBoxResolution.Size = new System.Drawing.Size(130, 21);
            this.comboBoxResolution.TabIndex = 6;
            // 
            // labeExemplare
            // 
            this.labeExemplare.AutoSize = true;
            this.labeExemplare.Location = new System.Drawing.Point(147, 111);
            this.labeExemplare.Name = "labeExemplare";
            this.labeExemplare.Size = new System.Drawing.Size(56, 13);
            this.labeExemplare.TabIndex = 5;
            this.labeExemplare.Text = "Exemplare";
            // 
            // labelFormat
            // 
            this.labelFormat.AutoSize = true;
            this.labelFormat.Location = new System.Drawing.Point(147, 61);
            this.labelFormat.Name = "labelFormat";
            this.labelFormat.Size = new System.Drawing.Size(39, 13);
            this.labelFormat.TabIndex = 4;
            this.labelFormat.Text = "Format";
            // 
            // labelDrucker
            // 
            this.labelDrucker.AutoSize = true;
            this.labelDrucker.Location = new System.Drawing.Point(4, 11);
            this.labelDrucker.Name = "labelDrucker";
            this.labelDrucker.Size = new System.Drawing.Size(45, 13);
            this.labelDrucker.TabIndex = 3;
            this.labelDrucker.Text = "Drucker";
            // 
            // comboBoxFormat
            // 
            this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFormat.FormattingEnabled = true;
            this.comboBoxFormat.Items.AddRange(new object[] {
            "Querformat",
            "Hochformat"});
            this.comboBoxFormat.Location = new System.Drawing.Point(149, 77);
            this.comboBoxFormat.Name = "comboBoxFormat";
            this.comboBoxFormat.Size = new System.Drawing.Size(95, 21);
            this.comboBoxFormat.TabIndex = 1;
            // 
            // comboBoxDrucker
            // 
            this.comboBoxDrucker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrucker.FormattingEnabled = true;
            this.comboBoxDrucker.Location = new System.Drawing.Point(6, 27);
            this.comboBoxDrucker.Name = "comboBoxDrucker";
            this.comboBoxDrucker.Size = new System.Drawing.Size(238, 21);
            this.comboBoxDrucker.TabIndex = 0;
            this.comboBoxDrucker.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDrucker_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(111, 171);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(70, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(187, 171);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(70, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // frmPrintConfig
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(263, 200);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxPrinter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrintConfig";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Drucken";
            this.groupBoxPrinter.ResumeLayout(false);
            this.groupBoxPrinter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPrinter;
        private System.Windows.Forms.Label labelResolution;
        private System.Windows.Forms.Label labeExemplare;
        private System.Windows.Forms.Label labelFormat;
        private System.Windows.Forms.Label labelDrucker;
        internal System.Windows.Forms.ComboBox comboBoxDrucker;
        internal System.Windows.Forms.ComboBox comboBoxResolution;
        internal System.Windows.Forms.ComboBox comboBoxFormat;
        internal System.Windows.Forms.Button buttonOK;
        internal System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelDuplex;
        internal System.Windows.Forms.ComboBox comboBoxDuplex;
        private System.Windows.Forms.NumericUpDown numericUpDownCopies;
    }
}