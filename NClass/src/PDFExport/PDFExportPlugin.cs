using System.Threading;
using System.Windows.Forms;
using NClass.GUI;
using PdfSharp.Drawing;

namespace PDFExport
{
  public class PDFExportPlugin : SimplePlugin
  {
    /// <summary>
    /// Constructs a new instance of PDFExportPlugin.
    /// </summary>
    /// <param name="environment">An instance of NClassEnvironment.</param>
    public PDFExportPlugin(NClassEnvironment environment)
      : base(environment)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the plugin can be executed at the moment.
    /// </summary>
    public override bool IsAvailable
    {
      get
      {
        return Workspace.HasActiveProject;
      }
    }

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name
    {
      get { return "PDF Export"; }
    }

    /// <summary>
    /// Gets the author of the plugin.
    /// </summary>
    public override string Author
    {
      get { return "Malte Ried"; }
    }

    /// <summary>
    /// Gets the menu text for the plugin.
    /// </summary>
    public override string MenuText
    {
      get { return "&Export as PDF..."; }
    }

    /// <summary>
    /// Starts the functionality of the plugin.
    /// </summary>
    protected override void Launch()
    {
      if(!Workspace.HasActiveProject)
      {
        return;
      }

      string fileName;
      using(SaveFileDialog dialog = new SaveFileDialog())
      {
        dialog.Filter = "PDF-Files (*.pdf)|*.pdf";
        if(dialog.ShowDialog() == DialogResult.Cancel)
        {
          return;
        }
        fileName = dialog.FileName;
      }

      PDFExportOptions optionsForm = new PDFExportOptions
                                       {
                                         PDFPadding = new Padding(10),
                                         SelectedOnly = false,
                                         Unit = XGraphicsUnit.Point
                                       };
      if(optionsForm.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }
      Padding padding = new Padding((int)new XUnit(optionsForm.PDFPadding.Left, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Top, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Right, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Bottom, optionsForm.Unit).Point);

      PDFExportProgress.ShowAsync(null);

      PDFExporter exporter = new PDFExporter(fileName, DocumentManager.ActiveDocument, optionsForm.SelectedOnly, padding);
      Thread exportThread = new Thread(exporter.Export)
                              {
                                Name = "PDFExporterThread", 
                                IsBackground = true
                              };
      exportThread.Start();
      while(exportThread.IsAlive)
      {
        Application.DoEvents();
        exportThread.Join(5);
      }
      exportThread.Join();

      PDFExportProgress.CloseAsync();
    }
  }
}