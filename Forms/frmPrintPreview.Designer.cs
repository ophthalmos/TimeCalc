namespace TimeCalc
{
    partial class FrmPrintPreview
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._txtStartPage = new System.Windows.Forms.ToolStripTextBox();
            this._lblPageCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._printPreviewControl = new System.Windows.Forms.PrintPreviewControl();
            this._btnLast = new System.Windows.Forms.ToolStripButton();
            this._btnPrint = new System.Windows.Forms.ToolStripButton();
            this._btnZoom = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._itemAuto = new System.Windows.Forms.ToolStripMenuItem();
            this._item200 = new System.Windows.Forms.ToolStripMenuItem();
            this._item100 = new System.Windows.Forms.ToolStripMenuItem();
            this._item50 = new System.Windows.Forms.ToolStripMenuItem();
            this._btnFirst = new System.Windows.Forms.ToolStripButton();
            this._btnPrev = new System.Windows.Forms.ToolStripButton();
            this._btnNext = new System.Windows.Forms.ToolStripButton();
            this._btnCancel = new System.Windows.Forms.ToolStripButton();
            this._toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStrip
            // 
            this._toolStrip.CanOverflow = false;
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnPrint,
            this.toolStripSeparator2,
            this._btnZoom,
            this.toolStripSeparator1,
            this._btnFirst,
            this._btnPrev,
            this._txtStartPage,
            this._lblPageCount,
            this._btnNext,
            this._btnLast,
            this.toolStripSeparator3,
            this._btnCancel});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(426, 25);
            this._toolStrip.TabIndex = 0;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _txtStartPage
            // 
            this._txtStartPage.AutoSize = false;
            this._txtStartPage.Name = "_txtStartPage";
            this._txtStartPage.Size = new System.Drawing.Size(32, 23);
            this._txtStartPage.Text = "1";
            this._txtStartPage.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._txtStartPage.ToolTipText = "Seite (Strg+S)";
            this._txtStartPage.Enter += new System.EventHandler(this.TxtStartPage_Enter);
            this._txtStartPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtStartPage_KeyPress);
            this._txtStartPage.Validating += new System.ComponentModel.CancelEventHandler(this.TxtStartPage_Validating);
            // 
            // _lblPageCount
            // 
            this._lblPageCount.AutoSize = false;
            this._lblPageCount.Name = "_lblPageCount";
            this._lblPageCount.Size = new System.Drawing.Size(32, 22);
            this._lblPageCount.Text = " /1";
            this._lblPageCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // _printPreviewControl
            // 
            this._printPreviewControl.AutoZoom = false;
            this._printPreviewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._printPreviewControl.Location = new System.Drawing.Point(0, 25);
            this._printPreviewControl.Name = "_printPreviewControl";
            this._printPreviewControl.Size = new System.Drawing.Size(426, 586);
            this._printPreviewControl.TabIndex = 1;
            this._printPreviewControl.Zoom = 0.5D;
            this._printPreviewControl.StartPageChanged += new System.EventHandler(this.Preview_StartPageChanged);
            // 
            // _btnLast
            // 
            this._btnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnLast.Image = global::TimeCalc.Properties.Resources.Last;
            this._btnLast.Name = "_btnLast";
            this._btnLast.Size = new System.Drawing.Size(23, 22);
            this._btnLast.Text = "Letzte Seite";
            this._btnLast.ToolTipText = "Letzte Seite (Strg+Ende)";
            this._btnLast.Click += new System.EventHandler(this.BtnLast_Click);
            // 
            // _btnPrint
            // 
            this._btnPrint.Enabled = false;
            this._btnPrint.Image = global::TimeCalc.Properties.Resources.Print;
            this._btnPrint.Name = "_btnPrint";
            this._btnPrint.Size = new System.Drawing.Size(71, 22);
            this._btnPrint.Text = "&Drucken";
            this._btnPrint.ToolTipText = "Drucken (Strg+P)";
            this._btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // _btnZoom
            // 
            this._btnZoom.AutoToolTip = false;
            this._btnZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this._itemAuto,
            this._item200,
            this._item100,
            this._item50});
            this._btnZoom.Image = global::TimeCalc.Properties.Resources.Zoom;
            this._btnZoom.Name = "_btnZoom";
            this._btnZoom.Size = new System.Drawing.Size(84, 22);
            this._btnZoom.Text = "&Zoomen";
            this._btnZoom.ToolTipText = "Zoomen (Strg+Z)";
            this._btnZoom.ButtonClick += new System.EventHandler(this.BtnZoom_ButtonClick);
            this._btnZoom.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.BtnZoom_DropDownItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(99, 6);
            // 
            // _itemAuto
            // 
            this._itemAuto.CheckOnClick = true;
            this._itemAuto.Name = "_itemAuto";
            this._itemAuto.Size = new System.Drawing.Size(102, 22);
            this._itemAuto.Text = "Auto";
            // 
            // _item200
            // 
            this._item200.CheckOnClick = true;
            this._item200.Name = "_item200";
            this._item200.Size = new System.Drawing.Size(102, 22);
            this._item200.Text = "200%";
            // 
            // _item100
            // 
            this._item100.CheckOnClick = true;
            this._item100.Name = "_item100";
            this._item100.Size = new System.Drawing.Size(102, 22);
            this._item100.Text = "100%";
            // 
            // _item50
            // 
            this._item50.Checked = true;
            this._item50.CheckOnClick = true;
            this._item50.CheckState = System.Windows.Forms.CheckState.Checked;
            this._item50.Name = "_item50";
            this._item50.Size = new System.Drawing.Size(102, 22);
            this._item50.Text = "50%";
            // 
            // _btnFirst
            // 
            this._btnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnFirst.Image = global::TimeCalc.Properties.Resources.First;
            this._btnFirst.Name = "_btnFirst";
            this._btnFirst.Size = new System.Drawing.Size(23, 22);
            this._btnFirst.Text = "Erste Seite";
            this._btnFirst.ToolTipText = "Erste Seite (Strg+Pos1)";
            this._btnFirst.Click += new System.EventHandler(this.BtnFirst_Click);
            // 
            // _btnPrev
            // 
            this._btnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnPrev.Image = global::TimeCalc.Properties.Resources.Previous;
            this._btnPrev.Name = "_btnPrev";
            this._btnPrev.Size = new System.Drawing.Size(23, 22);
            this._btnPrev.Text = "Vorige Seite";
            this._btnPrev.ToolTipText = "Vorige Seite (Bild auf)";
            this._btnPrev.Click += new System.EventHandler(this.BtnPrev_Click);
            // 
            // _btnNext
            // 
            this._btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnNext.Image = global::TimeCalc.Properties.Resources.Next;
            this._btnNext.Name = "_btnNext";
            this._btnNext.Size = new System.Drawing.Size(23, 22);
            this._btnNext.Text = "Nächste Seite";
            this._btnNext.ToolTipText = "Nächste Seite (Bild ab)";
            this._btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.AutoToolTip = false;
            this._btnCancel.Image = global::TimeCalc.Properties.Resources.Close;
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(85, 22);
            this._btnCancel.Text = "A&bbrechen";
            this._btnCancel.ToolTipText = "Abbrechen (Esc)";
            this._btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // frmPrintPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(426, 611);
            this.Controls.Add(this._printPreviewControl);
            this.Controls.Add(this._toolStrip);
            this.Name = "frmPrintPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Seitenvorschau";
            this.Load += new System.EventHandler(this.FrmPrintPreview_Load);
            this.Shown += new System.EventHandler(this.FrmPrintPreview_Shown);
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _btnPrint;
        private System.Windows.Forms.ToolStripSplitButton _btnZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem _item200;
        private System.Windows.Forms.ToolStripMenuItem _item100;
        private System.Windows.Forms.ToolStripMenuItem _item50;
        private System.Windows.Forms.ToolStripMenuItem _itemAuto;
        private System.Windows.Forms.ToolStripButton _btnFirst;
        private System.Windows.Forms.ToolStripButton _btnPrev;
        private System.Windows.Forms.ToolStripTextBox _txtStartPage;
        private System.Windows.Forms.ToolStripLabel _lblPageCount;
        private System.Windows.Forms.ToolStripButton _btnNext;
        private System.Windows.Forms.ToolStripButton _btnLast;
        private System.Windows.Forms.ToolStripButton _btnCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.PrintPreviewControl _printPreviewControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}