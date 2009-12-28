using System;
using System.Windows.Forms;
using PDFExport.Lang;
using PDFExport.Properties;
using PdfSharp.Drawing;

namespace PDFExport
{
  /// <summary>
  /// A form which lets the user choose some options for the PDF-Export.
  /// </summary>
  public partial class PDFExportOptions : Form
  {
    // ========================================================================
    // Attributes

    #region === Attributes

    /// <summary>
    /// Gets or sets if only the selected Elements should be exported.
    /// </summary>
    public bool SelectedOnly { get; set; }

    /// <summary>
    /// Gets or sets the unit of the padding.
    /// </summary>
    public XGraphicsUnit Unit { get; set; }

    /// <summary>
    /// Gets or sets the padding of the PDF.
    /// </summary>
    public Padding PDFPadding { get; set; }

    #endregion

    // ========================================================================
    // Con- / Destruction

    #region === Con- / Destruction

    /// <summary>
    /// Initializes a new instance of PDFExportOptions.
    /// </summary>
    public PDFExportOptions()
    {
      InitializeComponent();

      LocalizeComponents();
      LoadSettings();
    }

    #endregion

    // ========================================================================
    // Methods

    #region === Methods

    /// <summary>
    /// Displays the text for the current culture.
    /// </summary>
    private void LocalizeComponents()
    {
      Text = Strings.OptionsTitle;
      chkSelectedOnly.Text = Strings.OptionsOnlySelected;
      grpMargin.Text = Strings.OptionsMargin;
      lblUnit.Text = Strings.OptionsUnit;
      lblTop.Text = Strings.OptionsTop;
      lblRight.Text = Strings.OptionsRight;
      lblBottom.Text = Strings.OptionsBottom;
      lblLeft.Text = Strings.OptionsLeft;
      cmdOK.Text = Strings.OptionsOKButton;
      cmdCancel.Text = Strings.OptionsCancelButton;
    }

    /// <summary>
    /// Loads the last used settings into the properties.
    /// </summary>
    private void LoadSettings()
    {
      SelectedOnly = Settings.Default.OnlySelectedElements;
      Unit = Settings.Default.Unit;
      PDFPadding = Settings.Default.Padding;
    }

    /// <summary>
    /// Stores the current values of the properties into the settings.
    /// </summary>
    private void SaveSettings()
    {
      Settings.Default.OnlySelectedElements = SelectedOnly;
      Settings.Default.Unit = Unit;
      Settings.Default.Padding = PDFPadding;

      Settings.Default.Save();
    }

    /// <summary>
    /// Converts a XGraphicsUnit to a readable String.
    /// </summary>
    /// <param name="unit">The XGraphicsUnit to convert.</param>
    /// <returns>A string representing the XGraphicsUnit.</returns>
    private static String GetUnitString(XGraphicsUnit unit)
    {
      switch(unit)
      {
        case XGraphicsUnit.Point:
          return Strings.UnitDots;
        case XGraphicsUnit.Inch:
          return Strings.UnitInch;
        case XGraphicsUnit.Millimeter:
          return Strings.UnitMM;
        case XGraphicsUnit.Centimeter:
          return Strings.UnitCM;
        case XGraphicsUnit.Presentation:
          return Strings.UnitPixel;
        default:
          return Strings.UnitDots;
      }
    }

    /// <summary>
    /// Converts a string to the corresponding XGraphicsUnit.
    /// </summary>
    /// <param name="unit">The string to convert.</param>
    /// <returns>The resulting XGraphicsUnit.</returns>
    private static XGraphicsUnit GetUnit(String unit)
    {
      foreach(XGraphicsUnit xGraphicsUnit in Enum.GetValues(typeof(XGraphicsUnit)))
      {
        if(GetUnitString(xGraphicsUnit) == unit)
        {
          return xGraphicsUnit;
        }
      }

      return XGraphicsUnit.Point;
    }

    #endregion

    // ========================================================================
    // Event Handler

    #region === Event Handler

    /// <summary>
    /// Updates the properties.
    /// </summary>
    /// <param name="sender">The caller.</param>
    /// <param name="e">Additional information.</param>
    private void cmdOK_Click(object sender, EventArgs e)
    {
      SelectedOnly = chkSelectedOnly.Checked;
      PDFPadding = new Padding((int)numLeft.Value, (int)numTop.Value, (int)numRight.Value, (int)numBottom.Value);
      Unit = GetUnit(cboUnit.Text);

      SaveSettings();
    }

    /// <summary>
    /// Initialises the window.
    /// </summary>
    /// <param name="sender">The caller.</param>
    /// <param name="e">Additional information.</param>
    private void PDFExportOptions_Shown(object sender, EventArgs e)
    {
      chkSelectedOnly.Checked = SelectedOnly;

      numTop.Value = PDFPadding.Top;
      numRight.Value = PDFPadding.Right;
      numBottom.Value = PDFPadding.Bottom;
      numLeft.Value = PDFPadding.Left;

      cboUnit.Items.Clear();
      foreach(XGraphicsUnit xGraphicsUnit in Enum.GetValues(typeof(XGraphicsUnit)))
      {
        cboUnit.Items.Add(GetUnitString(xGraphicsUnit));
      }
      cboUnit.Text = GetUnitString(Unit);
    }

    #endregion
  }
}
