using System;
using System.Windows.Forms;
using PdfSharp.Drawing;

namespace PDFExport
{
  public partial class PDFExportOptions : Form
  {
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

    /// <summary>
    /// Initializes a new instance of PDFExportOptions.
    /// </summary>
    public PDFExportOptions()
    {
      InitializeComponent();
    }

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
          return "dot";
        case XGraphicsUnit.Inch:
          return "inch";
        case XGraphicsUnit.Millimeter:
          return "mm";
        case XGraphicsUnit.Centimeter:
          return "cm";
        case XGraphicsUnit.Presentation:
          return "pixel";
        default:
          return "dot";
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
  }
}
