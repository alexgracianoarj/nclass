namespace NClass.DiagramEditor.ClassDiagram.Editors
{
	partial class ClassTypeEditor
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblStereotype = new System.Windows.Forms.Label();
            this.txtStereotype = new NClass.DiagramEditor.ClassDiagram.Editors.BorderedTextBox();
            this.pnlAdvancedOptions = new System.Windows.Forms.Panel();
            this.cboIdGenerator = new System.Windows.Forms.ComboBox();
            this.txtNHMTableName = new NClass.DiagramEditor.ClassDiagram.Editors.BorderedTextBox();
            this.lblNHMTableName = new System.Windows.Forms.Label();
            this.lblIdGenerator = new System.Windows.Forms.Label();
            this.toolStripAdvancedOptions = new System.Windows.Forms.ToolStrip();
            this.toolAdvancedOptions = new System.Windows.Forms.ToolStripButton();
            this.pnlAdvancedOptions.SuspendLayout();
            this.toolStripAdvancedOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Size = new System.Drawing.Size(298, 20);
            // 
            // lblStereotype
            // 
            this.lblStereotype.AutoSize = true;
            this.lblStereotype.Location = new System.Drawing.Point(-2, 4);
            this.lblStereotype.Name = "lblStereotype";
            this.lblStereotype.Size = new System.Drawing.Size(61, 13);
            this.lblStereotype.TabIndex = 10;
            this.lblStereotype.Text = "Stereotype:";
            // 
            // txtStereotype
            // 
            this.txtStereotype.Location = new System.Drawing.Point(59, 1);
            this.txtStereotype.Name = "txtStereotype";
            this.txtStereotype.Padding = new System.Windows.Forms.Padding(1);
            this.txtStereotype.ReadOnly = false;
            this.txtStereotype.SelectionStart = 0;
            this.txtStereotype.Size = new System.Drawing.Size(240, 20);
            this.txtStereotype.TabIndex = 9;
            this.txtStereotype.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStereotype_KeyDown);
            this.txtStereotype.Validating += new System.ComponentModel.CancelEventHandler(this.txtStereotype_Validating);
            // 
            // pnlAdvancedOptions
            // 
            this.pnlAdvancedOptions.Controls.Add(this.cboIdGenerator);
            this.pnlAdvancedOptions.Controls.Add(this.txtNHMTableName);
            this.pnlAdvancedOptions.Controls.Add(this.lblNHMTableName);
            this.pnlAdvancedOptions.Controls.Add(this.lblStereotype);
            this.pnlAdvancedOptions.Controls.Add(this.txtStereotype);
            this.pnlAdvancedOptions.Controls.Add(this.lblIdGenerator);
            this.pnlAdvancedOptions.Location = new System.Drawing.Point(3, 52);
            this.pnlAdvancedOptions.Name = "pnlAdvancedOptions";
            this.pnlAdvancedOptions.Size = new System.Drawing.Size(321, 77);
            this.pnlAdvancedOptions.TabIndex = 12;
            // 
            // cboIdGenerator
            // 
            this.cboIdGenerator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIdGenerator.FormattingEnabled = true;
            this.cboIdGenerator.Location = new System.Drawing.Point(124, 53);
            this.cboIdGenerator.Name = "cboIdGenerator";
            this.cboIdGenerator.Size = new System.Drawing.Size(100, 21);
            this.cboIdGenerator.TabIndex = 12;
            this.cboIdGenerator.SelectedIndexChanged += new System.EventHandler(this.cboIdGenerator_SelectedIndexChanged);
            // 
            // txtNHMTableName
            // 
            this.txtNHMTableName.Location = new System.Drawing.Point(59, 27);
            this.txtNHMTableName.Name = "txtNHMTableName";
            this.txtNHMTableName.Padding = new System.Windows.Forms.Padding(1);
            this.txtNHMTableName.ReadOnly = false;
            this.txtNHMTableName.SelectionStart = 0;
            this.txtNHMTableName.Size = new System.Drawing.Size(240, 20);
            this.txtNHMTableName.TabIndex = 10;
            this.txtNHMTableName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.borderedTextBox1_KeyDown);
            this.txtNHMTableName.Validating += new System.ComponentModel.CancelEventHandler(this.borderedTextBox1_Validating);
            // 
            // lblNHMTableName
            // 
            this.lblNHMTableName.AutoSize = true;
            this.lblNHMTableName.Location = new System.Drawing.Point(-3, 31);
            this.lblNHMTableName.Name = "lblNHMTableName";
            this.lblNHMTableName.Size = new System.Drawing.Size(65, 13);
            this.lblNHMTableName.TabIndex = 10;
            this.lblNHMTableName.Text = "NHM Table:";
            // 
            // lblIdGenerator
            // 
            this.lblIdGenerator.AutoSize = true;
            this.lblIdGenerator.Location = new System.Drawing.Point(56, 56);
            this.lblIdGenerator.Name = "lblIdGenerator";
            this.lblIdGenerator.Size = new System.Drawing.Size(69, 13);
            this.lblIdGenerator.TabIndex = 13;
            this.lblIdGenerator.Text = "Id Generator:";
            // 
            // toolStripAdvancedOptions
            // 
            this.toolStripAdvancedOptions.AutoSize = false;
            this.toolStripAdvancedOptions.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripAdvancedOptions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripAdvancedOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAdvancedOptions});
            this.toolStripAdvancedOptions.Location = new System.Drawing.Point(304, 26);
            this.toolStripAdvancedOptions.Name = "toolStripAdvancedOptions";
            this.toolStripAdvancedOptions.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripAdvancedOptions.Size = new System.Drawing.Size(25, 25);
            this.toolStripAdvancedOptions.TabIndex = 0;
            // 
            // toolAdvancedOptions
            // 
            this.toolAdvancedOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAdvancedOptions.Image = global::NClass.DiagramEditor.Properties.Resources.ExpandSingle;
            this.toolAdvancedOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAdvancedOptions.Name = "toolAdvancedOptions";
            this.toolAdvancedOptions.Size = new System.Drawing.Size(23, 22);
            this.toolAdvancedOptions.Click += new System.EventHandler(this.toolAdvancedOptions_Click);
            // 
            // ClassTypeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlAdvancedOptions);
            this.Controls.Add(this.toolStripAdvancedOptions);
            this.Name = "ClassTypeEditor";
            this.Size = new System.Drawing.Size(330, 133);
            this.Controls.SetChildIndex(this.toolStripAdvancedOptions, 0);
            this.Controls.SetChildIndex(this.txtName, 0);
            this.Controls.SetChildIndex(this.pnlAdvancedOptions, 0);
            this.pnlAdvancedOptions.ResumeLayout(false);
            this.pnlAdvancedOptions.PerformLayout();
            this.toolStripAdvancedOptions.ResumeLayout(false);
            this.toolStripAdvancedOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblStereotype;
		private BorderedTextBox txtStereotype;
		private System.Windows.Forms.Panel pnlAdvancedOptions;
		private System.Windows.Forms.ToolStrip toolStripAdvancedOptions;
		private System.Windows.Forms.ToolStripButton toolAdvancedOptions;
        private System.Windows.Forms.Label lblNHMTableName;
        private BorderedTextBox txtNHMTableName;
        private System.Windows.Forms.ComboBox cboIdGenerator;
        private System.Windows.Forms.Label lblIdGenerator;
	}
}
