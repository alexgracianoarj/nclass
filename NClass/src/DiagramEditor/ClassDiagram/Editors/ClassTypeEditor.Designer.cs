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
			this.txtStereotype.AcceptsTab = true;
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
			this.pnlAdvancedOptions.Controls.Add(this.lblStereotype);
			this.pnlAdvancedOptions.Controls.Add(this.txtStereotype);
			this.pnlAdvancedOptions.Location = new System.Drawing.Point(3, 52);
			this.pnlAdvancedOptions.Name = "pnlAdvancedOptions";
			this.pnlAdvancedOptions.Size = new System.Drawing.Size(321, 23);
			this.pnlAdvancedOptions.TabIndex = 12;
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
			this.Size = new System.Drawing.Size(330, 81);
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
	}
}
