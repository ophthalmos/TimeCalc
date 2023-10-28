namespace TimeCalc
{
    partial class QuerySummary
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
            panel = new Panel();
            label = new Label();
            textBox = new TextBox();
            button = new Button();
            panel.SuspendLayout();
            SuspendLayout();
            // 
            // panel
            // 
            panel.BackColor = SystemColors.ControlLightLight;
            panel.Controls.Add(label);
            panel.Controls.Add(textBox);
            panel.Dock = DockStyle.Top;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.Padding = new Padding(10);
            panel.Size = new Size(384, 306);
            panel.TabIndex = 0;
            // 
            // label
            // 
            label.AutoSize = true;
            label.Location = new Point(13, 279);
            label.Name = "label";
            label.Size = new Size(67, 19);
            label.TabIndex = 1;
            label.Text = "Summary";
            // 
            // textBox
            // 
            textBox.BorderStyle = BorderStyle.None;
            textBox.Dock = DockStyle.Top;
            textBox.Location = new Point(10, 10);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size(364, 266);
            textBox.TabIndex = 2;
            textBox.TextChanged += TextBox_TextChanged;
            // 
            // button
            // 
            button.DialogResult = DialogResult.OK;
            button.Location = new Point(269, 312);
            button.Name = "button";
            button.Size = new Size(105, 25);
            button.TabIndex = 0;
            button.Text = "OK";
            button.UseVisualStyleBackColor = true;
            // 
            // QuerySummary
            // 
            AcceptButton = button;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button;
            ClientSize = new Size(384, 348);
            Controls.Add(button);
            Controls.Add(panel);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuerySummary";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "QuerySummary";
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel;
        private Label label;
        private TextBox textBox;
        private Button button;
    }
}