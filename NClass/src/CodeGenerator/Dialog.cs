// NClass - Free class diagram editor
// Copyright (C) 2006-2009 Balazs Tihanyi
// 
// This program is free software; you can redistribute it and/or modify it under 
// the terms of the GNU General Public License as published by the Free Software 
// Foundation; either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// this program; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;
using System.Collections.Specialized;
using System.Windows.Forms;
using NClass.Core;
using NClass.CSharp;
using NClass.Java;
using NClass.Translations;

using System.Threading;

using DatabaseSchemaReader.DataSchema;

namespace NClass.CodeGenerator
{
	public partial class Dialog : Form
	{
		Project project = null;

		public Dialog()
		{
            InitializeComponent();
            importToolStrip.Renderer = ToolStripSimplifiedRenderer.Default;
		}

		private void UpdateTexts()
		{
			this.Text = Strings.CodeGeneration;
			lblDestination.Text = Strings.Destination;
			btnAddItem.Text = Strings.ButtonAddItem;
			btnBrowse.Text = Strings.ButtonBrowse;
			btnGenerate.Text = Strings.ButtonGenerate;
			btnCancel.Text = Strings.ButtonCancel;
			toolImportList.Text = Strings.ImportList;
			toolMoveUp.Text = Strings.MoveUp;
			toolMoveDown.Text = Strings.MoveDown;
			toolDelete.Text = Strings.Delete;
			lblSolutionType.Text = Strings.SolutionType;
			grpCodeStyle.Text = Strings.CodeStyle;
			chkUseTabs.Text = Strings.UseTabs;
			lblIndentSize.Text = Strings.IndentSize;
			rbNotImplemented.Text = Strings.UseNotImplementedExceptions;
		}

		private void UpdateValues()
		{
			lstImportList.Items.Clear();
			UpdateImportList();

			txtDestination.Text = Settings.Default.DestinationPath;
			chkUseTabs.Checked = Settings.Default.UseTabsForIndents;
			updIndentSize.Value = Settings.Default.IndentSize;
			cboSolutionType.SelectedItem = Settings.Default.SolutionType;
			rbNotImplemented.Checked = Settings.Default.UseNotImplementedExceptions;
            rbAutomaticProperties.Checked = Settings.Default.UseAutomaticProperties;
            chkGenerateNHibernateMapping.Checked = Settings.Default.GenerateNHibernateMapping;
            cboDefaultMapping.SelectedItem = Settings.Default.DefaultMapping;
            cboDefaultIdGenerator.SelectedItem = Settings.Default.DefaultIdGenerator;
            chkDefaultLazyFetching.Checked = Settings.Default.DefaultLazyFetching;
            chkUseLowercaseUnderscoredWordsInDb.Checked = Settings.Default.UseLowercaseAndUnderscoredWordsInDb;
            txtTextPrefix.Text = Settings.Default.PrefixTable;
            chkGenerateCodeFromTemplates.Checked = Settings.Default.GenerateCodeFromTemplates;
            chkGenerateSqlCode.Checked = Settings.Default.GenerateSQLCode;
            cboSqlToServerType.SelectedItem = Settings.Default.SQLToServerType;
            cboLanguage.SelectedIndex = 0;
		}

		private void UpdateImportList()
		{
			Language language = null;

			//TODO: ezt le kellene kérdezni egy LanguageManager-től
			if (object.Equals(cboLanguage.SelectedItem, "C#"))
				language = CSharpLanguage.Instance;
			else if (object.Equals(cboLanguage.SelectedItem, "Java"))
				language = JavaLanguage.Instance;

			if (language != null)
			{
				Settings.Default.Upgrade();
				lstImportList.Items.Clear();
				foreach (string importString in Settings.Default.ImportList[language])
					lstImportList.Items.Add(importString);
			}
		}

		private void SaveImportList()
		{
			StringCollection importList = new StringCollection();
			foreach (object import in lstImportList.Items)
				importList.Add(import.ToString());

			//TODO: ezt is másképp kéne
			if (object.Equals(cboLanguage.SelectedItem, "C#"))
				Settings.Default.CSharpImportList = importList;
			else if (object.Equals(cboLanguage.SelectedItem, "Java"))
				Settings.Default.JavaImportList = importList;
		}

		public void ShowDialog(Project project)
		{
			this.project = project;

			UpdateTexts();
            PopulateSolutionType();
            PopulateIdGeneratorType();
            PopulateMappingType();
            PopulateSqlToServerType();
			UpdateValues();
			ShowDialog();
			
			if (DialogResult == DialogResult.OK)
				Settings.Default.Save();
			else
				Settings.Default.Reload();
		}

        private void PopulateSolutionType()
        {
            cboSolutionType.DataSource = Enum.GetValues(typeof(SolutionType));
        }

        private void PopulateMappingType()
        {
            cboDefaultMapping.DataSource = Enum.GetValues(typeof(MappingType));
        }

        private void PopulateIdGeneratorType()
        {
            cboDefaultIdGenerator.DataSource = Enum.GetValues(typeof(IdGeneratorType));
        }

        private void PopulateSqlToServerType()
        {
            cboSqlToServerType.DataSource = Enum.GetValues(typeof(SqlType));
        }

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				dialog.Description = Strings.GeneratorTargetDir;
				dialog.SelectedPath = txtDestination.Text;
				if (dialog.ShowDialog() == DialogResult.OK)
					txtDestination.Text = dialog.SelectedPath;
			}
		}

		private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateImportList();
		}

		private void toolMoveUp_Click(object sender, EventArgs e)
		{
			int index = lstImportList.SelectedIndex;
			if (index > 0)
			{
				object temp = lstImportList.Items[index];
				lstImportList.Items[index] = lstImportList.Items[index - 1];
				lstImportList.Items[index - 1] = temp;
				lstImportList.SelectedIndex--;
				SaveImportList();
			}
		}

		private void toolMoveDown_Click(object sender, EventArgs e)
		{
			int index = lstImportList.SelectedIndex;
			if (index < lstImportList.Items.Count - 1)
			{
				object temp = lstImportList.Items[index];
				lstImportList.Items[index] = lstImportList.Items[index + 1];
				lstImportList.Items[index + 1] = temp;
				lstImportList.SelectedIndex++;
				SaveImportList();
			}
		}

		private void toolDelete_Click(object sender, EventArgs e)
		{
			if (lstImportList.SelectedItem != null)
			{
				int selectedIndex = lstImportList.SelectedIndex;
				lstImportList.Items.RemoveAt(selectedIndex);
				if (lstImportList.Items.Count > selectedIndex)
					lstImportList.SelectedIndex = selectedIndex;
				else
					lstImportList.SelectedIndex = lstImportList.Items.Count - 1;
				SaveImportList();
			}
		}

		private void lstImportList_SelectedValueChanged(object sender, EventArgs e)
		{
			bool isSelected = (lstImportList.SelectedItem != null);

			toolMoveUp.Enabled = isSelected && (lstImportList.SelectedIndex > 0);
			toolMoveDown.Enabled = isSelected &&
				(lstImportList.SelectedIndex < lstImportList.Items.Count - 1);
			toolDelete.Enabled = isSelected;
		}

		private void lstImportList_Leave(object sender, EventArgs e)
		{
			lstImportList.SelectedItem = null;
		}

		private void txtNewImport_TextChanged(object sender, EventArgs e)
		{
			btnAddItem.Enabled = (txtNewImport.Text.Length > 0);
		}

		private void txtNewImport_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && txtNewImport.Text.Length > 0)
			{
				lstImportList.Items.Add(txtNewImport.Text);
				txtNewImport.Text = string.Empty;
				SaveImportList();
			}
		}

		private void btnAddItem_Click(object sender, EventArgs e)
		{
			lstImportList.Items.Add(txtNewImport.Text);
			txtNewImport.Text = string.Empty;
			txtNewImport.Focus();
			SaveImportList();
		}

		private void chkUseTabs_CheckedChanged(object sender, EventArgs e)
		{
			bool useTabs = chkUseTabs.Checked;

			lblIndentSize.Enabled = !useTabs;
			updIndentSize.Enabled = !useTabs;
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			if (project != null)
			{
				ValidateSettings();

                // Initialize the dialog that will contain the progress bar
                ProgressDialog progressDialog = new ProgressDialog();

                // Set the dialog to operate in indeterminate mode
                progressDialog.SetIndeterminate(true);

                SolutionType solutionType = (SolutionType)cboSolutionType.SelectedIndex;
                string destination = txtDestination.Text;

                try
                {
                    Generator generator = new Generator(project, solutionType);
                    GenerationResult result = new GenerationResult();

                    Thread backgroundThread = new Thread(
                        new ThreadStart(() =>
                        {
                            result = generator.Generate(destination);
                            
                            // Close the dialog if it hasn't been already
                            if (progressDialog.InvokeRequired)
                                progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                        }));

                    result = CheckDestination(destination);

                    if (result == GenerationResult.Success)
                    {
                        // Start the background process thread
                        backgroundThread.Start();

                        // Open the dialog
                        progressDialog.ShowDialog();
                    }

                    if (result == GenerationResult.Success)
                    {
                        MessageBox.Show(Strings.CodeGenerationCompleted,
                            Strings.CodeGeneration, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else if (result == GenerationResult.Error)
                    {
                        MessageBox.Show(Strings.CodeGenerationFailed,
                            Strings.Error, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else // Cancelled
                    {
                        this.DialogResult = DialogResult.None;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Strings.UnknownError,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
			}
		}

        private GenerationResult CheckDestination(string location)
        {
            try
            {
                location = Path.Combine(location, project.Name);
                if (Directory.Exists(location))
                {
                    DialogResult result = MessageBox.Show(
                                this,
                                Strings.CodeGenerationOverwriteConfirmation,
                                Strings.Confirmation,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                        return GenerationResult.Success;
                    else
                        return GenerationResult.Cancelled;
                }
                else
                {
                    Directory.CreateDirectory(location);
                    return GenerationResult.Success;
                }
            }
            catch
            {
                return GenerationResult.Error;
            }
        }

		private void ValidateSettings()
		{
			Settings.Default.DestinationPath = txtDestination.Text;
			Settings.Default.UseTabsForIndents = chkUseTabs.Checked;
			Settings.Default.IndentSize = (int) updIndentSize.Value;
            Settings.Default.SolutionType = (SolutionType)cboSolutionType.SelectedItem;
			Settings.Default.UseNotImplementedExceptions = rbNotImplemented.Checked;
            Settings.Default.UseAutomaticProperties = rbAutomaticProperties.Checked;
            Settings.Default.GenerateNHibernateMapping = chkGenerateNHibernateMapping.Checked;
            Settings.Default.DefaultMapping = (MappingType) cboDefaultMapping.SelectedItem;
            Settings.Default.DefaultIdGenerator = (IdGeneratorType)cboDefaultIdGenerator.SelectedItem;
            Settings.Default.DefaultLazyFetching = chkDefaultLazyFetching.Checked;
            Settings.Default.UseLowercaseAndUnderscoredWordsInDb = chkUseLowercaseUnderscoredWordsInDb.Checked;
            Settings.Default.PrefixTable = txtTextPrefix.Text.Trim();
            Settings.Default.GenerateCodeFromTemplates = chkGenerateCodeFromTemplates.Checked;
            Settings.Default.GenerateSQLCode = chkGenerateSqlCode.Checked;
            Settings.Default.SQLToServerType = (SqlType)cboSqlToServerType.SelectedItem;
        }
	}
}