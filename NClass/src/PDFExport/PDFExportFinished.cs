using System.Windows.Forms;
using PDFExport.Lang;

namespace PDFExport
{
  /// <summary>
  /// A form which gets displayed after the PDF-Export has finished.
  /// </summary>
  public partial class PDFExportFinished : Form
  {
    /// <summary>
    /// Initializes a new instance of PDFExportFinished.
    /// </summary>
    public PDFExportFinished()
    {
      InitializeComponent();

      LocalizeComponents();
    }

    /// <summary>
    /// Displays the text for the current culture.
    /// </summary>
    private void LocalizeComponents()
    {
      Text = Strings.FinishedDialogTitle;
      lblFinished.Text = Strings.FinishedDialogText;
      cmdOpen.Text = Strings.FinishedDialogOpen;
      cmdClose.Text = Strings.FinishedDialogClose;
    }
  }
}
