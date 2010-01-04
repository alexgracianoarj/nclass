using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NClass.AssemblyImport.Lang;
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
      elementNameMap.Add(Strings.Element_Class, Elements.Class);
      elementNameMap.Add(Strings.Element_Constructor, Elements.Constructor);
      elementNameMap.Add(Strings.Element_Delegate, Elements.Delegate);
      elementNameMap.Add(Strings.Element_Elements, Elements.Elements);
      elementNameMap.Add(Strings.Element_Enum, Elements.Enum);
      elementNameMap.Add(Strings.Element_Event, Elements.Event);
      elementNameMap.Add(Strings.Element_Field, Elements.Field);
      elementNameMap.Add(Strings.Element_Interface, Elements.Interface);
      elementNameMap.Add(Strings.Element_Method, Elements.Method);
      elementNameMap.Add(Strings.Element_Property, Elements.Property);
      elementNameMap.Add(Strings.Element_Struct, Elements.Struct);

      modifierNameMap.Add(Strings.Modifier_All, Modifiers.All);
      modifierNameMap.Add(Strings.Modifier_Instance, Modifiers.Instance);
      modifierNameMap.Add(Strings.Modifier_Internal, Modifiers.Internal);
      modifierNameMap.Add(Strings.Modifier_Private, Modifiers.Private);
      modifierNameMap.Add(Strings.Modifier_Protected, Modifiers.Protected);
      modifierNameMap.Add(Strings.Modifier_ProtectedInternal, Modifiers.ProtectedInternal);
      modifierNameMap.Add(Strings.Modifier_Public, Modifiers.Public);
      modifierNameMap.Add(Strings.Modifier_Static, Modifiers.Static);

      //Build reverse maps and ComboBox-Items
      colExceptElement.Items.Clear();
      reverseElementNameMap.Clear();
      foreach(string stName in elementNameMap.Keys)
      {
        colExceptElement.Items.Add(stName);
        reverseElementNameMap.Add(elementNameMap[stName], stName);
      }
      colExceptModifier.Items.Clear();
      reverseModifierNameMap.Clear();
      foreach(string stName in modifierNameMap.Keys)
      {
        colExceptModifier.Items.Add(stName);
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
                                          Name = Strings.Settings_Template_LastUsed
                                        };
        Settings.Default.ImportSettingsTemplates.Add(xNewSettings);
      }
      foreach(object xTemplate in Settings.Default.ImportSettingsTemplates)
      {
        cboTemplate.Items.Add(xTemplate);
      }
      cboTemplate.SelectedItem = cboTemplate.Items[0];
      DisplaySettings((ImportSettings)cboTemplate.Items[0]);

      LocalizeComponents();
    }

    /// <summary>
    /// Displays the text for the current culture.
    /// </summary>
    private void LocalizeComponents()
    {
      Text = Strings.Settings_Title;
      grpTemplate.Text = Strings.Settings_Template;
      cmdLoadTemplate.Text = Strings.Settings_Template_LoadButton;
      cmdStoreTemplate.Text = Strings.Settings_Template_StoreButton;
      cmdDeleteTemplate.Text = Strings.Settings_Template_DeleteButton;
      lblDescription.Text = Strings.Settings_ImportExcept;
      colExceptElement.HeaderText = Strings.Settings_ImportExcept_Element;
      colExceptModifier.HeaderText = Strings.Settings_ImportExcept_Modifier;
      chkCreateAggregations.Text = Strings.Settings_CreateAggregations;
      chkLabelAggregations.Text = Strings.Settings_CreateLabel;
      chkRemoveFields.Text = Strings.Settings_RemoveFields;
      cmdOK.Text = Strings.Settings_OKButton;
      cmdCancel.Text = Strings.Settings_CancelButton;
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
        MessageBox.Show(Strings.Settings_Error_NoTemplateSelected, Strings.Error_MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show(Strings.Settings_Error_NoTemplateName, Strings.Error_MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      if(cboTemplate.Text.Contains("<"))
      {
        MessageBox.Show(Strings.Settings_Error_AngleBracketNotAllowed, Strings.Error_MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show(Strings.Settings_Error_NoTemplateSelected, Strings.Error_MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      if(cboTemplate.Text.Contains("<"))
      {
        MessageBox.Show(Strings.Settings_Error_TemplateCantBeDeleted, Strings.Error_MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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