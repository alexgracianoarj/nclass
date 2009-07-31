using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
      axElementNameMap.Add("class", Elements.Class);
      axElementNameMap.Add("constructor", Elements.Constructor);
      axElementNameMap.Add("delegate", Elements.Delegate);
      axElementNameMap.Add("elements", Elements.Elements);
      axElementNameMap.Add("enum", Elements.Enum);
      axElementNameMap.Add("event", Elements.Event);
      axElementNameMap.Add("field", Elements.Field);
      axElementNameMap.Add("interface", Elements.Interface);
      axElementNameMap.Add("method", Elements.Method);
      axElementNameMap.Add("property", Elements.Property);
      axElementNameMap.Add("struct", Elements.Struct);

      axModifierNameMap.Add("all", Modifiers.All);
      axModifierNameMap.Add("instance", Modifiers.Instance);
      axModifierNameMap.Add("internal", Modifiers.Internal);
      axModifierNameMap.Add("private", Modifiers.Private);
      axModifierNameMap.Add("protected", Modifiers.Protected);
      axModifierNameMap.Add("protected internal", Modifiers.ProtectedInternal);
      axModifierNameMap.Add("public", Modifiers.Public);
      axModifierNameMap.Add("static", Modifiers.Static);

      //Build reverse maps and ComboBox-Items
      xExceptionColumnElement.Items.Clear();
      axReverseElementNameMap.Clear();
      foreach(string stName in axElementNameMap.Keys)
      {
        xExceptionColumnElement.Items.Add(stName);
        axReverseElementNameMap.Add(axElementNameMap[stName], stName);
      }
      xExceptionColumnModifier.Items.Clear();
      axReverseModifierNameMap.Clear();
      foreach(string stName in axModifierNameMap.Keys)
      {
        xExceptionColumnModifier.Items.Add(stName);
        axReverseModifierNameMap.Add(axModifierNameMap[stName], stName);
      }

      xSettings = theSettings;

      //Templates
      cboTemplate.Items.Clear();
      if(Settings.Default.ImportSettingsTemplates == null)
      {
        Settings.Default.ImportSettingsTemplates = new TemplateList();
        ImportSettings xNewSettings = new ImportSettings();
        xNewSettings.Name = "<last used>";
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
    ImportSettings xSettings;
    /// <summary>
    /// A map from element names to the element enum.
    /// </summary>
    Dictionary<string, Elements> axElementNameMap = new Dictionary<string, Elements>();
    /// <summary>
    /// A map from element enum to the element names.
    /// </summary>
    Dictionary<Elements, string> axReverseElementNameMap = new Dictionary<Elements, string>();
    /// <summary>
    /// A map from the modifier names to the modifier enum.
    /// </summary>
    Dictionary<string, Modifiers> axModifierNameMap = new Dictionary<string, Modifiers>();
    /// <summary>
    /// A map from the modifier enum to the modifier names.
    /// </summary>
    Dictionary<Modifiers, string> axReverseModifierNameMap = new Dictionary<Modifiers, string>();
    
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
      StoreSettings(xSettings);
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
      ImportSettings xSettings = (ImportSettings)cboTemplate.SelectedItem;
      if(xSettings == null)
      {
        xSettings = new ImportSettings();
      }
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

    #endregion

    #region === Methods

    /// <summary>
    /// Displays the given Settings.
    /// </summary>
    /// <param name="xSettings">The Settings which shall be displayed.</param>
    private void DisplaySettings(ImportSettings theSettings)
    {
      dgvExceptions.Rows.Clear();
      foreach(ModifierElement xRule in theSettings.Rules)
      {
        dgvExceptions.Rows.Add(axReverseModifierNameMap[xRule.Modifier], axReverseElementNameMap[xRule.Element]);
      }
    }

    /// <summary>
    /// Stores the displayed settings to <paramref name="xSettings"/>
    /// </summary>
    /// <param name="xSettings">The destination of the store operation.</param>
    private void StoreSettings(ImportSettings theSettings)
    {
      theSettings.Rules.Clear();
      foreach(DataGridViewRow xRow in dgvExceptions.Rows)
      {
        if(xRow.Cells[0].Value != null && xRow.Cells[1].Value != null)
        {
          theSettings.Rules.Add(new ModifierElement(axModifierNameMap[(string)xRow.Cells[0].Value], axElementNameMap[(string)xRow.Cells[1].Value]));
        }
      }
    }

    #endregion
  }
}