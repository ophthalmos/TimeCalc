namespace TimeCalc
{
    partial class Absences
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            label = new Label();
            dataGridView = new DataGridView();
            Feiertag = new DataGridViewTextBoxColumn();
            Urlaub = new DataGridViewTextBoxColumn();
            Krankheit = new DataGridViewTextBoxColumn();
            buttonOK = new Button();
            buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // label
            // 
            label.BackColor = SystemColors.ControlLightLight;
            label.Dock = DockStyle.Top;
            label.Location = new Point(0, 0);
            label.Name = "label";
            label.Padding = new Padding(5);
            label.Size = new Size(403, 76);
            label.TabIndex = 0;
            label.Text = "Eine Abwesenheit ist nichts anderes, als ein Ersatz von Arbeitszeit";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { Feiertag, Urlaub, Krankheit });
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.Dock = DockStyle.Top;
            dataGridView.Location = new Point(0, 76);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 100;
            dataGridView.RowTemplate.Height = 25;
            dataGridView.ScrollBars = ScrollBars.None;
            dataGridView.Size = new Size(403, 333);
            dataGridView.TabIndex = 1;
            // 
            // Feiertag
            // 
            Feiertag.Frozen = true;
            Feiertag.HeaderText = "Feiertag";
            Feiertag.MinimumWidth = 100;
            Feiertag.Name = "Feiertag";
            // 
            // Urlaub
            // 
            Urlaub.Frozen = true;
            Urlaub.HeaderText = "Urlaub";
            Urlaub.MinimumWidth = 100;
            Urlaub.Name = "Urlaub";
            // 
            // Krankheit
            // 
            Krankheit.Frozen = true;
            Krankheit.HeaderText = "Krankheit";
            Krankheit.MinimumWidth = 100;
            Krankheit.Name = "Krankheit";
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(175, 415);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(105, 27);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "Speichern";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(286, 415);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(105, 27);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // Absences
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(403, 454);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(dataGridView);
            Controls.Add(label);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Absences";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Absences";
            KeyDown += Absences_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn Feiertag;
        private DataGridViewTextBoxColumn Urlaub;
        private DataGridViewTextBoxColumn Krankheit;
        private Button buttonOK;
        private Button buttonCancel;
    }
}