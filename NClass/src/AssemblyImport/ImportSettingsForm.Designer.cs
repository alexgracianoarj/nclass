namespace NClass.AssemblyImport
{
  partial class ImportSettingsForm
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
      if(disposing && (components != null))
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
		this.lblDescription = new System.Windows.Forms.Label();
		this.cmdOK = new System.Windows.Forms.Button();
		this.dgvExceptions = new System.Windows.Forms.DataGridView();
		this.xExceptionColumnModifier = new System.Windows.Forms.DataGridViewComboBoxColumn();
		this.xExceptionColumnElement = new System.Windows.Forms.DataGridViewComboBoxColumn();
		this.grpTemplate = new System.Windows.Forms.GroupBox();
		this.cmdLoadTemplate = new System.Windows.Forms.Button();
		this.cmdDeleteTemplate = new System.Windows.Forms.Button();
		this.cmdStoreTemplate = new System.Windows.Forms.Button();
		this.cboTemplate = new System.Windows.Forms.ComboBox();
		this.cmdCancel = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize) (this.dgvExceptions)).BeginInit();
		this.grpTemplate.SuspendLayout();
		this.SuspendLayout();
		// 
		// lblDescription
		// 
		this.lblDescription.AutoSize = true;
		this.lblDescription.Location = new System.Drawing.Point(9, 65);
		this.lblDescription.Name = "lblDescription";
		this.lblDescription.Size = new System.Drawing.Size(123, 13);
		this.lblDescription.TabIndex = 0;
		this.lblDescription.Text = "Import everything except";
		// 
		// cmdOK
		// 
		this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
		this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.cmdOK.Location = new System.Drawing.Point(268, 305);
		this.cmdOK.Name = "cmdOK";
		this.cmdOK.Size = new System.Drawing.Size(75, 23);
		this.cmdOK.TabIndex = 3;
		this.cmdOK.Text = "OK";
		this.cmdOK.UseVisualStyleBackColor = true;
		this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
		// 
		// dgvExceptions
		// 
		this.dgvExceptions.AllowUserToResizeColumns = false;
		this.dgvExceptions.AllowUserToResizeRows = false;
		this.dgvExceptions.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					| System.Windows.Forms.AnchorStyles.Left)
					| System.Windows.Forms.AnchorStyles.Right)));
		this.dgvExceptions.BackgroundColor = System.Drawing.SystemColors.Control;
		this.dgvExceptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvExceptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.xExceptionColumnModifier,
            this.xExceptionColumnElement});
		this.dgvExceptions.Location = new System.Drawing.Point(12, 81);
		this.dgvExceptions.Name = "dgvExceptions";
		this.dgvExceptions.RowHeadersVisible = false;
		this.dgvExceptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvExceptions.Size = new System.Drawing.Size(412, 218);
		this.dgvExceptions.TabIndex = 4;
		// 
		// xExceptionColumnModifier
		// 
		this.xExceptionColumnModifier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.xExceptionColumnModifier.HeaderText = "Modifier";
		this.xExceptionColumnModifier.Items.AddRange(new object[] {
            "all",
            "public",
            "protected",
            "private",
            "internal",
            "protected internal"});
		this.xExceptionColumnModifier.Name = "xExceptionColumnModifier";
		// 
		// xExceptionColumnElement
		// 
		this.xExceptionColumnElement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.xExceptionColumnElement.HeaderText = "Element";
		this.xExceptionColumnElement.Items.AddRange(new object[] {
            "elements",
            "class",
            "struct",
            "interface",
            "enum",
            "delegate",
            "nested type",
            "method",
            "constructor",
            "field",
            "property",
            "event"});
		this.xExceptionColumnElement.MaxDropDownItems = 12;
		this.xExceptionColumnElement.Name = "xExceptionColumnElement";
		// 
		// grpTemplate
		// 
		this.grpTemplate.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					| System.Windows.Forms.AnchorStyles.Right)));
		this.grpTemplate.Controls.Add(this.cmdLoadTemplate);
		this.grpTemplate.Controls.Add(this.cmdDeleteTemplate);
		this.grpTemplate.Controls.Add(this.cmdStoreTemplate);
		this.grpTemplate.Controls.Add(this.cboTemplate);
		this.grpTemplate.Location = new System.Drawing.Point(12, 12);
		this.grpTemplate.Name = "grpTemplate";
		this.grpTemplate.Size = new System.Drawing.Size(412, 50);
		this.grpTemplate.TabIndex = 8;
		this.grpTemplate.TabStop = false;
		this.grpTemplate.Text = "Template";
		// 
		// cmdLoadTemplate
		// 
		this.cmdLoadTemplate.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.cmdLoadTemplate.Location = new System.Drawing.Point(157, 17);
		this.cmdLoadTemplate.Name = "cmdLoadTemplate";
		this.cmdLoadTemplate.Size = new System.Drawing.Size(75, 23);
		this.cmdLoadTemplate.TabIndex = 2;
		this.cmdLoadTemplate.Text = "Load";
		this.cmdLoadTemplate.UseVisualStyleBackColor = true;
		this.cmdLoadTemplate.Click += new System.EventHandler(this.cmdLoadTemplate_Click);
		// 
		// cmdDeleteTemplate
		// 
		this.cmdDeleteTemplate.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.cmdDeleteTemplate.Location = new System.Drawing.Point(319, 17);
		this.cmdDeleteTemplate.Name = "cmdDeleteTemplate";
		this.cmdDeleteTemplate.Size = new System.Drawing.Size(87, 23);
		this.cmdDeleteTemplate.TabIndex = 1;
		this.cmdDeleteTemplate.Text = "Delete";
		this.cmdDeleteTemplate.UseVisualStyleBackColor = true;
		this.cmdDeleteTemplate.Click += new System.EventHandler(this.cmdDeleteTemplate_Click);
		// 
		// cmdStoreTemplate
		// 
		this.cmdStoreTemplate.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.cmdStoreTemplate.Location = new System.Drawing.Point(238, 17);
		this.cmdStoreTemplate.Name = "cmdStoreTemplate";
		this.cmdStoreTemplate.Size = new System.Drawing.Size(75, 23);
		this.cmdStoreTemplate.TabIndex = 1;
		this.cmdStoreTemplate.Text = "Store";
		this.cmdStoreTemplate.UseVisualStyleBackColor = true;
		this.cmdStoreTemplate.Click += new System.EventHandler(this.cmdStoreTemplate_Click);
		// 
		// cboTemplate
		// 
		this.cboTemplate.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					| System.Windows.Forms.AnchorStyles.Right)));
		this.cboTemplate.FormattingEnabled = true;
		this.cboTemplate.Location = new System.Drawing.Point(6, 19);
		this.cboTemplate.Name = "cboTemplate";
		this.cboTemplate.Size = new System.Drawing.Size(145, 21);
		this.cboTemplate.TabIndex = 0;
		// 
		// cmdCancel
		// 
		this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
		this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cmdCancel.Location = new System.Drawing.Point(349, 305);
		this.cmdCancel.Name = "cmdCancel";
		this.cmdCancel.Size = new System.Drawing.Size(75, 23);
		this.cmdCancel.TabIndex = 9;
		this.cmdCancel.Text = "Cancel";
		this.cmdCancel.UseVisualStyleBackColor = true;
		// 
		// ImportSettingsForm
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(436, 340);
		this.Controls.Add(this.cmdCancel);
		this.Controls.Add(this.grpTemplate);
		this.Controls.Add(this.dgvExceptions);
		this.Controls.Add(this.cmdOK);
		this.Controls.Add(this.lblDescription);
		this.MinimumSize = new System.Drawing.Size(444, 374);
		this.Name = "ImportSettingsForm";
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Import Settings";
		this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportSettingsForm_FormClosed);
		((System.ComponentModel.ISupportInitialize) (this.dgvExceptions)).EndInit();
		this.grpTemplate.ResumeLayout(false);
		this.ResumeLayout(false);
		this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Button cmdOK;
    private System.Windows.Forms.DataGridView dgvExceptions;
    private System.Windows.Forms.GroupBox grpTemplate;
    private System.Windows.Forms.Button cmdLoadTemplate;
    private System.Windows.Forms.Button cmdDeleteTemplate;
    private System.Windows.Forms.Button cmdStoreTemplate;
    private System.Windows.Forms.ComboBox cboTemplate;
    private System.Windows.Forms.DataGridViewComboBoxColumn xExceptionColumnModifier;
    private System.Windows.Forms.DataGridViewComboBoxColumn xExceptionColumnElement;
    private System.Windows.Forms.Button cmdCancel;
  }
}