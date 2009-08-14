using System;
using System.Collections.Generic;
using System.Windows.Forms;

using NClass.AssemblyImport.Properties;

namespace NClass.AssemblyImport
{
  /// <summary>
  /// A form to set up the ImportSettings.
  /// </summary>
  public partial class ImportSettingsForm : Form
  {

    #region === Construction

    /// <summary>
    /// Initializes a new ImportSettingsForm2.
    /// </summary>
    /// <param name="theSettings">The ImportSettings which will be used for import. </param>
    public ImportSettingsForm(ImportSettings theSettings)
    {
      InitializeComponent();

      //Localization goes here...
      elementNameMap.Add("class", Elements.Class);
      elementNameMap.Add("constructor", Elements.Constructor);
      elementNameMap.Add("delegate", Elements.Delegate);
      elementNameMap.Add("elements", Elements.Elements);
      elementNameMap.Add("enum", Elements.Enum);
      elementNameMap.Add("event", Elements.Event);
      elementNameMap.Add("field", Elements.Field);
      elementNameMap.Add("interface", Elements.Interface);
      elementNameMap.Add("method", Elements.Method);
      elementNameMap.Add("property", Elements.Property);
      elementNameMap.Add("struct", Elements.Struct);

      modifierNameMap.Add("all", Modifiers.All);
      modifierNameMap.Add("instance", Modifiers.Instance);
      modifierNameMap.Add("internal", Modifiers.Internal);
      modifierNameMap.Add("private", Modifiers.Private);
      modifierNameMap.Add("protected", Modifiers.Protected);
      modifierNameMap.Add("protected internal", Modifiers.ProtectedInternal);
      modifierNameMap.Add("public", Modifiers.Public);
      modifierNameMap.Add("static", Modifiers.Static);

      //Build reverse maps and ComboBox-Items
      xExceptionColumnElement.Items.Clear();
      reverseElementNameMap.Clear();
      foreach(string stName in elementNameMap.Keys)
      {
        xExceptionColumnElement.Items.Add(stName);
        reverseElementNameMap.Add(elementNameMap[stName], stName);
      }
      xExceptionColumnModifier.Items.Clear();
      reverseModifierNameMap.Clear();
      foreach(string stName in modifierNameMap.Keys)
      {
        xExceptionColumnModifier.Items.Add(stName);
        reverseModifierNameMap.Add(modifierNameMap[stName], stName);
      }

      importSettings = theSettings;

      //Templates
      cboTemplate.Items.Clear();
      if(Settings.Default.ImportSettingsTemplates == null)
      {
        Settings.Default.ImportSettingsTemplates = new TemplateList();
        ImportSettings xNewSettings = new ImportSettings
                                        {
                                          Name = "<last used>"
                                        };
        Settings.Default.ImportSettingsTemplates.Add(xNewSettings);
      }
      foreach(object xTemplate in Settings.Default.ImportSettingsTemplates)
      {
        cboTemplate.Items.Add(xTemplate);
      }
      cboTemplate.SelectedItem = cboTemplate.Items[0];
      DisplaySettings((ImportSettings)cboTemplate.Items[0]);

    }

    #endregion

    #region === Fields

    /// <summary>
    /// The settings which are used for the import
    /// </summary>
    readonly ImportSettings importSettings;
    /// <summary>
    /// A map from element names to the element enum.
    /// </summary>
    readonly Dictionary<string, Elements> elementNameMap = new Dictionary<string, Elements>();
    /// <summary>
    /// A map from element enum to the element names.
    /// </summary>
    readonly Dictionary<Elements, string> reverseElementNameMap = new Dictionary<Elements, string>();
    /// <summary>
    /// A map from the modifier names to the modifier enum.
    /// </summary>
    readonly Dictionary<string, Modifiers> modifierNameMap = new Dictionary<string, Modifiers>();
    /// <summary>
    /// A map from the modifier enum to the modifier names.
    /// </summary>
    readonly Dictionary<Modifiers, string> reverseModifierNameMap = new Dictionary<Modifiers, string>();
    
    #endregion

    #region === Event-Methods

    /// <summary>
    /// Gets called when the OK-button is clicked. Closes the dialog.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void cmdOK_Click(object sender, EventArgs e)
    {
      Close();
    }

    /// <summary>
    /// Gets called when the dialog is closed. Stores all settings.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void ImportSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      StoreSettings(importSettings);
      StoreSettings((ImportSettings)Settings.Default.ImportSettingsTemplates[0]); //<last used>
      Settings.Default.Save();
    }

    /// <summary>
    /// Gets called when the LoadTemplate-button is clicked. Displays the
    /// settings belonging to the actual template.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void cmdLoadTemplate_Click(object sender, EventArgs e)
    {
      if(cboTemplate.SelectedItem == null)
      {
        MessageBox.Show("Please select a template first");
        return;
      }
      DisplaySettings((ImportSettings)cboTemplate.SelectedItem);
    }

    /// <summary>
    /// Gets called when the StoreTemplate-button is clicked. Stores the settings
    /// to a template.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void cmdStoreTemplate_Click(object sender, EventArgs e)
    {
      if(string.IsNullOrEmpty(cboTemplate.Text))
      {
        MessageBox.Show("Please enter a name");
        return;
      }
      if(cboTemplate.Text.Contains("<"))
      {
        MessageBox.Show("'<' is not allowed");
        return;
      }
      ImportSettings xSettings = (ImportSettings)cboTemplate.SelectedItem ?? new ImportSettings();
      StoreSettings(xSettings);
      xSettings.Name = cboTemplate.Text;
      if(cboTemplate.SelectedItem == null)
      {
        //New entry
        cboTemplate.Items.Add(xSettings);
        cboTemplate.SelectedItem = xSettings;
        Settings.Default.ImportSettingsTemplates.Add(xSettings);
      }
    }

    /// <summary>
    /// Gets called when the DeleteTemplate-button is clicked. Deletes the
    /// actual template.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void cmdDeleteTemplate_Click(object sender, EventArgs e)
    {
      if(cboTemplate.SelectedItem == null)
      {
        MessageBox.Show("Please select a template first");
        return;
      }
      if(cboTemplate.Text.Contains("<"))
      {
        MessageBox.Show("This template can't be deleted");
        return;
      }
      Settings.Default.ImportSettingsTemplates.Remove(cboTemplate.SelectedItem);
      cboTemplate.Items.Remove(cboTemplate.SelectedItem);
    }

    /// <summary>
    /// Gets called when the CreateAggregations-checkbox is (un)checked. 
    /// Updates the user interface.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">Information about the event.</param>
    private void chkCreateAggregations_CheckedChanged(object sender, EventArgs e)
    {
      chkLabelAggregations.Enabled = chkCreateAggregations.Checked;
      chkRemoveFields.Enabled = chkCreateAggregations.Checked;
    }

    #endregion

    #region === Methods

    /// <summary>
    /// Displays the given Settings.
    /// </summary>
    /// <param name="theSettings">The Settings which shall be displayed.</param>
    private void DisplaySettings(ImportSettings theSettings)
    {
      dgvExceptions.Rows.Clear();
      foreach(ModifierElement xRule in theSettings.Rules)
      {
        dgvExceptions.Rows.Add(reverseModifierNameMap[xRule.Modifier], reverseElementNameMap[xRule.Element]);
      }

      chkCreateAggregations.Checked = theSettings.CreateAggregations;
      chkLabelAggregations.Checked = theSettings.LabelAggregations;
      chkRemoveFields.Checked = theSettings.RemoveFields;
    }

    /// <summary>
    /// Stores the displayed settings to <paramref name="theSettings"/>
    /// </summary>
    /// <param name="theSettings">The destination of the store operation.</param>
    private void StoreSettings(ImportSettings theSettings)
    {
      theSettings.Rules.Clear();
      foreach(DataGridViewRow xRow in dgvExceptions.Rows)
      {
        if(xRow.Cells[0].Value != null && xRow.Cells[1].Value != null)
        {
          theSettings.Rules.Add(new ModifierElement(modifierNameMap[(string)xRow.Cells[0].Value], elementNameMap[(string)xRow.Cells[1].Value]));
        }
      }

      theSettings.CreateAggregations = chkCreateAggregations.Checked;
      theSettings.LabelAggregations = chkLabelAggregations.Checked;
      theSettings.RemoveFields = chkRemoveFields.Checked;
    }

    #endregion
  }
}