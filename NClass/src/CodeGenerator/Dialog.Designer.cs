namespace NClass.CodeGenerator
{
	partial class Dialog
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
			if (disposing && (components != null)) {
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
            this.chkUseTabs = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.updIndentSize = new System.Windows.Forms.NumericUpDown();
            this.lblIndentSize = new System.Windows.Forms.Label();
            this.lstImportList = new System.Windows.Forms.ListBox();
            this.importToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolDelete = new System.Windows.Forms.ToolStripButton();
            this.toolMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolMoveUp = new System.Windows.Forms.ToolStripButton();
            this.toolImportList = new System.Windows.Forms.ToolStripLabel();
            this.txtNewImport = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.grpCodeStyle = new System.Windows.Forms.GroupBox();
            this.chkAutomaticProperties = new System.Windows.Forms.CheckBox();
            this.chkNotImplemented = new System.Windows.Forms.CheckBox();
            this.chkUseLowercaseUnderscoredWordsInDb = new System.Windows.Forms.CheckBox();
            this.chkUseLazyLoading = new System.Windows.Forms.CheckBox();
            this.cboMappingType = new System.Windows.Forms.ComboBox();
            this.lblTypeOfMapping = new System.Windows.Forms.Label();
            this.chkGenerateNHibernateMapping = new System.Windows.Forms.CheckBox();
            this.lblSolutionType = new System.Windows.Forms.Label();
            this.cboSolutionType = new System.Windows.Forms.ComboBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.gpbNHibernate = new System.Windows.Forms.GroupBox();
            this.lblIdGeneratorType = new System.Windows.Forms.Label();
            this.cboIdGeneratorType = new System.Windows.Forms.ComboBox();
            this.lblUsePrefixTables = new System.Windows.Forms.Label();
            this.txtTextPrefix = new System.Windows.Forms.TextBox();
            this.chkGenerateCodeFromTemplates = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkGenerateSqlCode = new System.Windows.Forms.CheckBox();
            this.cboSqlToServerType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.updIndentSize)).BeginInit();
            this.importToolStrip.SuspendLayout();
            this.grpCodeStyle.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gpbNHibernate.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkUseTabs
            // 
            this.chkUseTabs.AutoSize = true;
            this.chkUseTabs.Location = new System.Drawing.Point(13, 20);
            this.chkUseTabs.Name = "chkUseTabs";
            this.chkUseTabs.Size = new System.Drawing.Size(120, 17);
            this.chkUseTabs.TabIndex = 0;
            this.chkUseTabs.Text = "Use tabs for indents";
            this.chkUseTabs.UseVisualStyleBackColor = true;
            this.chkUseTabs.CheckedChanged += new System.EventHandler(this.chkUseTabs_CheckedChanged);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGenerate.Location = new System.Drawing.Point(426, 413);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(507, 413);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // updIndentSize
            // 
            this.updIndentSize.Location = new System.Drawing.Point(13, 56);
            this.updIndentSize.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.updIndentSize.Name = "updIndentSize";
            this.updIndentSize.Size = new System.Drawing.Size(82, 20);
            this.updIndentSize.TabIndex = 2;
            this.updIndentSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lblIndentSize
            // 
            this.lblIndentSize.AutoSize = true;
            this.lblIndentSize.Enabled = false;
            this.lblIndentSize.Location = new System.Drawing.Point(10, 40);
            this.lblIndentSize.Name = "lblIndentSize";
            this.lblIndentSize.Size = new System.Drawing.Size(61, 13);
            this.lblIndentSize.TabIndex = 1;
            this.lblIndentSize.Text = "Indent size:";
            // 
            // lstImportList
            // 
            this.lstImportList.FormattingEnabled = true;
            this.lstImportList.Location = new System.Drawing.Point(12, 90);
            this.lstImportList.Name = "lstImportList";
            this.lstImportList.Size = new System.Drawing.Size(285, 186);
            this.lstImportList.TabIndex = 4;
            this.lstImportList.SelectedValueChanged += new System.EventHandler(this.lstImportList_SelectedValueChanged);
            this.lstImportList.Leave += new System.EventHandler(this.lstImportList_Leave);
            // 
            // importToolStrip
            // 
            this.importToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importToolStrip.AutoSize = false;
            this.importToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.importToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.importToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolDelete,
            this.toolMoveDown,
            this.toolMoveUp,
            this.toolImportList});
            this.importToolStrip.Location = new System.Drawing.Point(75, 62);
            this.importToolStrip.Name = "importToolStrip";
            this.importToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.importToolStrip.Size = new System.Drawing.Size(222, 25);
            this.importToolStrip.TabIndex = 3;
            this.importToolStrip.Text = "toolStrip1";
            // 
            // toolDelete
            // 
            this.toolDelete.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolDelete.Enabled = false;
            this.toolDelete.Image = global::NClass.CodeGenerator.Properties.Resources.Delete;
            this.toolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelete.Name = "toolDelete";
            this.toolDelete.Size = new System.Drawing.Size(23, 22);
            this.toolDelete.Text = "Delete";
            this.toolDelete.Click += new System.EventHandler(this.toolDelete_Click);
            // 
            // toolMoveDown
            // 
            this.toolMoveDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolMoveDown.Enabled = false;
            this.toolMoveDown.Image = global::NClass.CodeGenerator.Properties.Resources.MoveDown;
            this.toolMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolMoveDown.Name = "toolMoveDown";
            this.toolMoveDown.Size = new System.Drawing.Size(23, 22);
            this.toolMoveDown.Text = "Move Down";
            this.toolMoveDown.Click += new System.EventHandler(this.toolMoveDown_Click);
            // 
            // toolMoveUp
            // 
            this.toolMoveUp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolMoveUp.Enabled = false;
            this.toolMoveUp.Image = global::NClass.CodeGenerator.Properties.Resources.MoveUp;
            this.toolMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolMoveUp.Name = "toolMoveUp";
            this.toolMoveUp.Size = new System.Drawing.Size(23, 22);
            this.toolMoveUp.Text = "Move Up";
            this.toolMoveUp.Click += new System.EventHandler(this.toolMoveUp_Click);
            // 
            // toolImportList
            // 
            this.toolImportList.Name = "toolImportList";
            this.toolImportList.Size = new System.Drawing.Size(61, 22);
            this.toolImportList.Text = "Import list";
            // 
            // txtNewImport
            // 
            this.txtNewImport.Location = new System.Drawing.Point(12, 284);
            this.txtNewImport.Name = "txtNewImport";
            this.txtNewImport.Size = new System.Drawing.Size(204, 20);
            this.txtNewImport.TabIndex = 5;
            this.txtNewImport.TextChanged += new System.EventHandler(this.txtNewImport_TextChanged);
            this.txtNewImport.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewImport_KeyDown);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Enabled = false;
            this.btnAddItem.Location = new System.Drawing.Point(222, 282);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(75, 23);
            this.btnAddItem.TabIndex = 6;
            this.btnAddItem.Text = "Add item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // grpCodeStyle
            // 
            this.grpCodeStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCodeStyle.Controls.Add(this.chkAutomaticProperties);
            this.grpCodeStyle.Controls.Add(this.chkNotImplemented);
            this.grpCodeStyle.Controls.Add(this.lblIndentSize);
            this.grpCodeStyle.Controls.Add(this.updIndentSize);
            this.grpCodeStyle.Controls.Add(this.chkUseTabs);
            this.grpCodeStyle.Location = new System.Drawing.Point(311, 110);
            this.grpCodeStyle.Name = "grpCodeStyle";
            this.grpCodeStyle.Size = new System.Drawing.Size(271, 127);
            this.grpCodeStyle.TabIndex = 9;
            this.grpCodeStyle.TabStop = false;
            this.grpCodeStyle.Text = "Code style";
            // 
            // chkAutomaticProperties
            // 
            this.chkAutomaticProperties.AutoSize = true;
            this.chkAutomaticProperties.Location = new System.Drawing.Point(13, 105);
            this.chkAutomaticProperties.Name = "chkAutomaticProperties";
            this.chkAutomaticProperties.Size = new System.Drawing.Size(122, 17);
            this.chkAutomaticProperties.TabIndex = 5;
            this.chkAutomaticProperties.Text = "Automatic properties";
            this.chkAutomaticProperties.UseVisualStyleBackColor = true;
            // 
            // chkNotImplemented
            // 
            this.chkNotImplemented.AutoSize = true;
            this.chkNotImplemented.Location = new System.Drawing.Point(13, 82);
            this.chkNotImplemented.Name = "chkNotImplemented";
            this.chkNotImplemented.Size = new System.Drawing.Size(243, 17);
            this.chkNotImplemented.TabIndex = 4;
            this.chkNotImplemented.Text = "Fill methods with \'Not implemented\' exceptions";
            this.chkNotImplemented.UseVisualStyleBackColor = true;
            // 
            // chkUseLowercaseUnderscoredWordsInDb
            // 
            this.chkUseLowercaseUnderscoredWordsInDb.AutoSize = true;
            this.chkUseLowercaseUnderscoredWordsInDb.Location = new System.Drawing.Point(13, 105);
            this.chkUseLowercaseUnderscoredWordsInDb.Name = "chkUseLowercaseUnderscoredWordsInDb";
            this.chkUseLowercaseUnderscoredWordsInDb.Size = new System.Drawing.Size(213, 30);
            this.chkUseLowercaseUnderscoredWordsInDb.TabIndex = 10;
            this.chkUseLowercaseUnderscoredWordsInDb.Text = "Use lowercase and underscored words \r\nin the database";
            this.chkUseLowercaseUnderscoredWordsInDb.UseVisualStyleBackColor = true;
            // 
            // chkUseLazyLoading
            // 
            this.chkUseLazyLoading.AutoSize = true;
            this.chkUseLazyLoading.Location = new System.Drawing.Point(13, 82);
            this.chkUseLazyLoading.Name = "chkUseLazyLoading";
            this.chkUseLazyLoading.Size = new System.Drawing.Size(103, 17);
            this.chkUseLazyLoading.TabIndex = 9;
            this.chkUseLazyLoading.Text = "Use lazy loading";
            this.chkUseLazyLoading.UseVisualStyleBackColor = true;
            // 
            // cboMappingType
            // 
            this.cboMappingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMappingType.FormattingEnabled = true;
            this.cboMappingType.Location = new System.Drawing.Point(13, 55);
            this.cboMappingType.Name = "cboMappingType";
            this.cboMappingType.Size = new System.Drawing.Size(120, 21);
            this.cboMappingType.TabIndex = 8;
            // 
            // lblTypeOfMapping
            // 
            this.lblTypeOfMapping.AutoSize = true;
            this.lblTypeOfMapping.Location = new System.Drawing.Point(10, 39);
            this.lblTypeOfMapping.Name = "lblTypeOfMapping";
            this.lblTypeOfMapping.Size = new System.Drawing.Size(89, 13);
            this.lblTypeOfMapping.TabIndex = 7;
            this.lblTypeOfMapping.Text = "Type of mapping:";
            // 
            // chkGenerateNHibernateMapping
            // 
            this.chkGenerateNHibernateMapping.AutoSize = true;
            this.chkGenerateNHibernateMapping.Location = new System.Drawing.Point(13, 19);
            this.chkGenerateNHibernateMapping.Name = "chkGenerateNHibernateMapping";
            this.chkGenerateNHibernateMapping.Size = new System.Drawing.Size(170, 17);
            this.chkGenerateNHibernateMapping.TabIndex = 6;
            this.chkGenerateNHibernateMapping.Text = "Generate NHibernate mapping";
            this.chkGenerateNHibernateMapping.UseVisualStyleBackColor = true;
            // 
            // lblSolutionType
            // 
            this.lblSolutionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSolutionType.AutoSize = true;
            this.lblSolutionType.Location = new System.Drawing.Point(308, 67);
            this.lblSolutionType.Name = "lblSolutionType";
            this.lblSolutionType.Size = new System.Drawing.Size(101, 13);
            this.lblSolutionType.TabIndex = 12;
            this.lblSolutionType.Text = "Type of solution file:";
            // 
            // cboSolutionType
            // 
            this.cboSolutionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSolutionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSolutionType.FormattingEnabled = true;
            this.cboSolutionType.Items.AddRange(new object[] {
            "Visual Studio 2005",
            "Visual Studio 2008"});
            this.cboSolutionType.Location = new System.Drawing.Point(311, 83);
            this.cboSolutionType.Name = "cboSolutionType";
            this.cboSolutionType.Size = new System.Drawing.Size(271, 21);
            this.cboSolutionType.TabIndex = 13;
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "C#",
            "Java"});
            this.cboLanguage.Location = new System.Drawing.Point(12, 64);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(60, 21);
            this.cboLanguage.TabIndex = 14;
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.cboLanguage_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnBrowse, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtDestination, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDestination, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 37);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBrowse.Location = new System.Drawing.Point(495, 7);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(66, 8);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(423, 20);
            this.txtDestination.TabIndex = 5;
            // 
            // lblDestination
            // 
            this.lblDestination.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(0, 11);
            this.lblDestination.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(63, 13);
            this.lblDestination.TabIndex = 4;
            this.lblDestination.Text = "Destination:";
            // 
            // gpbNHibernate
            // 
            this.gpbNHibernate.Controls.Add(this.lblIdGeneratorType);
            this.gpbNHibernate.Controls.Add(this.cboIdGeneratorType);
            this.gpbNHibernate.Controls.Add(this.lblUsePrefixTables);
            this.gpbNHibernate.Controls.Add(this.txtTextPrefix);
            this.gpbNHibernate.Controls.Add(this.chkGenerateNHibernateMapping);
            this.gpbNHibernate.Controls.Add(this.lblTypeOfMapping);
            this.gpbNHibernate.Controls.Add(this.cboMappingType);
            this.gpbNHibernate.Controls.Add(this.chkUseLazyLoading);
            this.gpbNHibernate.Controls.Add(this.chkUseLowercaseUnderscoredWordsInDb);
            this.gpbNHibernate.Location = new System.Drawing.Point(311, 242);
            this.gpbNHibernate.Name = "gpbNHibernate";
            this.gpbNHibernate.Size = new System.Drawing.Size(271, 164);
            this.gpbNHibernate.TabIndex = 11;
            this.gpbNHibernate.TabStop = false;
            this.gpbNHibernate.Text = "NHibernate";
            // 
            // lblIdGeneratorType
            // 
            this.lblIdGeneratorType.AutoSize = true;
            this.lblIdGeneratorType.Location = new System.Drawing.Point(136, 39);
            this.lblIdGeneratorType.Name = "lblIdGeneratorType";
            this.lblIdGeneratorType.Size = new System.Drawing.Size(106, 13);
            this.lblIdGeneratorType.TabIndex = 16;
            this.lblIdGeneratorType.Text = "Type of Id generator:";
            // 
            // cboIdGeneratorType
            // 
            this.cboIdGeneratorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIdGeneratorType.FormattingEnabled = true;
            this.cboIdGeneratorType.Location = new System.Drawing.Point(139, 55);
            this.cboIdGeneratorType.Name = "cboIdGeneratorType";
            this.cboIdGeneratorType.Size = new System.Drawing.Size(121, 21);
            this.cboIdGeneratorType.TabIndex = 15;
            // 
            // lblUsePrefixTables
            // 
            this.lblUsePrefixTables.AutoSize = true;
            this.lblUsePrefixTables.Location = new System.Drawing.Point(6, 142);
            this.lblUsePrefixTables.Name = "lblUsePrefixTables";
            this.lblUsePrefixTables.Size = new System.Drawing.Size(193, 13);
            this.lblUsePrefixTables.TabIndex = 14;
            this.lblUsePrefixTables.Text = "Use prefix in the tables (empty to none):";
            // 
            // txtTextPrefix
            // 
            this.txtTextPrefix.Location = new System.Drawing.Point(200, 139);
            this.txtTextPrefix.Name = "txtTextPrefix";
            this.txtTextPrefix.Size = new System.Drawing.Size(65, 20);
            this.txtTextPrefix.TabIndex = 13;
            this.txtTextPrefix.Text = "pfx_";
            // 
            // chkGenerateCodeFromTemplates
            // 
            this.chkGenerateCodeFromTemplates.AutoSize = true;
            this.chkGenerateCodeFromTemplates.Location = new System.Drawing.Point(13, 19);
            this.chkGenerateCodeFromTemplates.Name = "chkGenerateCodeFromTemplates";
            this.chkGenerateCodeFromTemplates.Size = new System.Drawing.Size(168, 17);
            this.chkGenerateCodeFromTemplates.TabIndex = 16;
            this.chkGenerateCodeFromTemplates.Text = "Generate code from templates";
            this.chkGenerateCodeFromTemplates.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkGenerateCodeFromTemplates);
            this.groupBox1.Location = new System.Drawing.Point(12, 361);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 45);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Templates";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkGenerateSqlCode);
            this.groupBox2.Controls.Add(this.cboSqlToServerType);
            this.groupBox2.Location = new System.Drawing.Point(12, 310);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(285, 45);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SQL";
            // 
            // chkGenerateSqlCode
            // 
            this.chkGenerateSqlCode.AutoSize = true;
            this.chkGenerateSqlCode.Location = new System.Drawing.Point(13, 19);
            this.chkGenerateSqlCode.Name = "chkGenerateSqlCode";
            this.chkGenerateSqlCode.Size = new System.Drawing.Size(136, 17);
            this.chkGenerateSqlCode.TabIndex = 2;
            this.chkGenerateSqlCode.Text = "Generate SQL code to:";
            this.chkGenerateSqlCode.UseVisualStyleBackColor = true;
            // 
            // cboSqlToServerType
            // 
            this.cboSqlToServerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSqlToServerType.FormattingEnabled = true;
            this.cboSqlToServerType.Location = new System.Drawing.Point(158, 17);
            this.cboSqlToServerType.Name = "cboSqlToServerType";
            this.cboSqlToServerType.Size = new System.Drawing.Size(121, 21);
            this.cboSqlToServerType.TabIndex = 1;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 451);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.cboSolutionType);
            this.Controls.Add(this.lblSolutionType);
            this.Controls.Add(this.gpbNHibernate);
            this.Controls.Add(this.grpCodeStyle);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.txtNewImport);
            this.Controls.Add(this.importToolStrip);
            this.Controls.Add(this.lstImportList);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGenerate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 309);
            this.Name = "Dialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Code Generation";
            ((System.ComponentModel.ISupportInitialize)(this.updIndentSize)).EndInit();
            this.importToolStrip.ResumeLayout(false);
            this.importToolStrip.PerformLayout();
            this.grpCodeStyle.ResumeLayout(false);
            this.grpCodeStyle.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gpbNHibernate.ResumeLayout(false);
            this.gpbNHibernate.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkUseTabs;
		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.NumericUpDown updIndentSize;
		private System.Windows.Forms.Label lblIndentSize;
		private System.Windows.Forms.ListBox lstImportList;
		private System.Windows.Forms.ToolStrip importToolStrip;
		private System.Windows.Forms.ToolStripButton toolDelete;
		private System.Windows.Forms.ToolStripButton toolMoveDown;
		private System.Windows.Forms.ToolStripButton toolMoveUp;
		private System.Windows.Forms.ToolStripLabel toolImportList;
		private System.Windows.Forms.TextBox txtNewImport;
		private System.Windows.Forms.Button btnAddItem;
		private System.Windows.Forms.GroupBox grpCodeStyle;
		private System.Windows.Forms.Label lblSolutionType;
		private System.Windows.Forms.ComboBox cboSolutionType;
		private System.Windows.Forms.CheckBox chkNotImplemented;
		private System.Windows.Forms.ComboBox cboLanguage;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtDestination;
		private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.CheckBox chkAutomaticProperties;
        private System.Windows.Forms.CheckBox chkGenerateNHibernateMapping;
        private System.Windows.Forms.ComboBox cboMappingType;
        private System.Windows.Forms.Label lblTypeOfMapping;
        private System.Windows.Forms.CheckBox chkUseLazyLoading;
        private System.Windows.Forms.CheckBox chkUseLowercaseUnderscoredWordsInDb;
        private System.Windows.Forms.GroupBox gpbNHibernate;
        private System.Windows.Forms.TextBox txtTextPrefix;
        private System.Windows.Forms.Label lblUsePrefixTables;
        private System.Windows.Forms.CheckBox chkGenerateCodeFromTemplates;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblIdGeneratorType;
        private System.Windows.Forms.ComboBox cboIdGeneratorType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkGenerateSqlCode;
        private System.Windows.Forms.ComboBox cboSqlToServerType;
	}
}