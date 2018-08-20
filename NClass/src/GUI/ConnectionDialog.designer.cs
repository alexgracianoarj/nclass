namespace NClass.GUI
{
    partial class ConnectionDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.connectionStringTextBox = new System.Windows.Forms.TextBox();
            this.serverTypeComboBox = new System.Windows.Forms.ComboBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.connectionStringButton = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cboSchema = new System.Windows.Forms.ComboBox();
            this.cboConnection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTextPrefix = new System.Windows.Forms.TextBox();
            this.gpNHibernate = new System.Windows.Forms.GroupBox();
            this.cboDefaultIdGenerator = new System.Windows.Forms.ComboBox();
            this.chkDefaultFetching = new System.Windows.Forms.CheckBox();
            this.chkNHibernateMapping = new System.Windows.Forms.CheckBox();
            this.chkUseUnderscoreAndLowercaseInDB = new System.Windows.Forms.CheckBox();
            this.lblDefaultIdGenerator = new System.Windows.Forms.Label();
            this.gpNHibernate.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(501, 144);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(293, 144);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(112, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save && Connect";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name && Database:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection String:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Database Schema:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(242, 10);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(125, 20);
            this.txtName.TabIndex = 3;
            // 
            // connectionStringTextBox
            // 
            this.connectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionStringTextBox.Location = new System.Drawing.Point(112, 36);
            this.connectionStringTextBox.Name = "connectionStringTextBox";
            this.connectionStringTextBox.Size = new System.Drawing.Size(431, 20);
            this.connectionStringTextBox.TabIndex = 3;
            // 
            // serverTypeComboBox
            // 
            this.serverTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverTypeComboBox.FormattingEnabled = true;
            this.serverTypeComboBox.Location = new System.Drawing.Point(373, 10);
            this.serverTypeComboBox.Name = "serverTypeComboBox";
            this.serverTypeComboBox.Size = new System.Drawing.Size(122, 21);
            this.serverTypeComboBox.TabIndex = 16;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.deleteButton.Location = new System.Drawing.Point(411, 144);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(80, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "Remove";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.OnDeleteButtonClick);
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.addButton.Location = new System.Drawing.Point(501, 8);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 17;
            this.addButton.Text = "New";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.OnAddButtonClick);
            // 
            // connectionStringButton
            // 
            this.connectionStringButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.connectionStringButton.Location = new System.Drawing.Point(549, 34);
            this.connectionStringButton.Name = "connectionStringButton";
            this.connectionStringButton.Size = new System.Drawing.Size(27, 23);
            this.connectionStringButton.TabIndex = 18;
            this.connectionStringButton.Text = "...";
            this.connectionStringButton.UseVisualStyleBackColor = true;
            this.connectionStringButton.Click += new System.EventHandler(this.OnConnectionStringButtonClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(112, 60);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(122, 23);
            this.btnConnect.TabIndex = 19;
            this.btnConnect.Text = "Connect for Schema";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cboSchema
            // 
            this.cboSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSchema.FormattingEnabled = true;
            this.cboSchema.Location = new System.Drawing.Point(242, 61);
            this.cboSchema.Name = "cboSchema";
            this.cboSchema.Size = new System.Drawing.Size(178, 21);
            this.cboSchema.TabIndex = 21;
            // 
            // cboConnection
            // 
            this.cboConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnection.FormattingEnabled = true;
            this.cboConnection.Location = new System.Drawing.Point(112, 10);
            this.cboConnection.Name = "cboConnection";
            this.cboConnection.Size = new System.Drawing.Size(122, 21);
            this.cboConnection.TabIndex = 22;
            this.cboConnection.SelectedIndexChanged += new System.EventHandler(this.cboConnection_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(426, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Prefix Removal:";
            // 
            // txtTextPrefix
            // 
            this.txtTextPrefix.Location = new System.Drawing.Point(513, 62);
            this.txtTextPrefix.Name = "txtTextPrefix";
            this.txtTextPrefix.Size = new System.Drawing.Size(63, 20);
            this.txtTextPrefix.TabIndex = 24;
            this.txtTextPrefix.Text = "pfx_";
            // 
            // gpNHibernate
            // 
            this.gpNHibernate.Controls.Add(this.cboDefaultIdGenerator);
            this.gpNHibernate.Controls.Add(this.chkDefaultFetching);
            this.gpNHibernate.Controls.Add(this.chkNHibernateMapping);
            this.gpNHibernate.Controls.Add(this.chkUseUnderscoreAndLowercaseInDB);
            this.gpNHibernate.Controls.Add(this.lblDefaultIdGenerator);
            this.gpNHibernate.Location = new System.Drawing.Point(12, 89);
            this.gpNHibernate.Name = "gpNHibernate";
            this.gpNHibernate.Size = new System.Drawing.Size(564, 49);
            this.gpNHibernate.TabIndex = 25;
            this.gpNHibernate.TabStop = false;
            this.gpNHibernate.Text = "NHibernate Defaults";
            // 
            // cboDefaultIdGenerator
            // 
            this.cboDefaultIdGenerator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultIdGenerator.FormattingEnabled = true;
            this.cboDefaultIdGenerator.Location = new System.Drawing.Point(144, 17);
            this.cboDefaultIdGenerator.Name = "cboDefaultIdGenerator";
            this.cboDefaultIdGenerator.Size = new System.Drawing.Size(100, 21);
            this.cboDefaultIdGenerator.TabIndex = 2;
            // 
            // chkDefaultFetching
            // 
            this.chkDefaultFetching.AutoSize = true;
            this.chkDefaultFetching.Location = new System.Drawing.Point(262, 19);
            this.chkDefaultFetching.Name = "chkDefaultFetching";
            this.chkDefaultFetching.Size = new System.Drawing.Size(48, 17);
            this.chkDefaultFetching.TabIndex = 3;
            this.chkDefaultFetching.Text = "Lazy";
            this.chkDefaultFetching.UseVisualStyleBackColor = true;
            // 
            // chkNHibernateMapping
            // 
            this.chkNHibernateMapping.AutoSize = true;
            this.chkNHibernateMapping.Location = new System.Drawing.Point(6, 19);
            this.chkNHibernateMapping.Name = "chkNHibernateMapping";
            this.chkNHibernateMapping.Size = new System.Drawing.Size(67, 17);
            this.chkNHibernateMapping.TabIndex = 0;
            this.chkNHibernateMapping.Text = "Mapping";
            this.chkNHibernateMapping.UseVisualStyleBackColor = true;
            // 
            // chkUseUnderscoreAndLowercaseInDB
            // 
            this.chkUseUnderscoreAndLowercaseInDB.AutoSize = true;
            this.chkUseUnderscoreAndLowercaseInDB.Location = new System.Drawing.Point(319, 19);
            this.chkUseUnderscoreAndLowercaseInDB.Name = "chkUseUnderscoreAndLowercaseInDB";
            this.chkUseUnderscoreAndLowercaseInDB.Size = new System.Drawing.Size(239, 17);
            this.chkUseUnderscoreAndLowercaseInDB.TabIndex = 4;
            this.chkUseUnderscoreAndLowercaseInDB.Text = "Use Underscore and Lowercase in Database";
            this.chkUseUnderscoreAndLowercaseInDB.UseVisualStyleBackColor = true;
            // 
            // lblDefaultIdGenerator
            // 
            this.lblDefaultIdGenerator.AutoSize = true;
            this.lblDefaultIdGenerator.Location = new System.Drawing.Point(77, 20);
            this.lblDefaultIdGenerator.Name = "lblDefaultIdGenerator";
            this.lblDefaultIdGenerator.Size = new System.Drawing.Size(69, 13);
            this.lblDefaultIdGenerator.TabIndex = 1;
            this.lblDefaultIdGenerator.Text = "Id Generator:";
            // 
            // ConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(584, 176);
            this.Controls.Add(this.gpNHibernate);
            this.Controls.Add(this.txtTextPrefix);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboConnection);
            this.Controls.Add(this.cboSchema);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.connectionStringButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.serverTypeComboBox);
            this.Controls.Add(this.connectionStringTextBox);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Dialog";
            this.gpNHibernate.ResumeLayout(false);
            this.gpNHibernate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox connectionStringTextBox;
        private System.Windows.Forms.ComboBox serverTypeComboBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button connectionStringButton;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cboSchema;
        private System.Windows.Forms.ComboBox cboConnection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTextPrefix;
        private System.Windows.Forms.GroupBox gpNHibernate;
        private System.Windows.Forms.Label lblDefaultIdGenerator;
        private System.Windows.Forms.CheckBox chkNHibernateMapping;
        private System.Windows.Forms.CheckBox chkDefaultFetching;
        private System.Windows.Forms.ComboBox cboDefaultIdGenerator;
        private System.Windows.Forms.CheckBox chkUseUnderscoreAndLowercaseInDB;
    }
}