using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using NClass.GUI;
using PDFExport.Properties;
using PdfSharp.Drawing;
using Strings=PDFExport.Lang.Strings;

namespace PDFExport
{
  /// <summary>
  /// A plugin to export a diagram to a pdf.
  /// </summary>
  public class PDFExportPlugin : Plugin
  {
    // ========================================================================
    // Attributes

    #region === Attributes

    /// <summary>
    /// The menu item used to start the export.
    /// </summary>
    private readonly ToolStripMenuItem menuItem;

    #endregion

    // ========================================================================
    // Con- / Destruction

    #region === Con- / Destruction

    /// <summary>
    /// Set up the current culture for the strings.
    /// </summary>
    static PDFExportPlugin()
    {
      Strings.Culture = CultureInfo.GetCultureInfo(NClass.GUI.Settings.Default.UILanguage);
    }

    /// <summary>
    /// Constructs a new instance of PDFExportPlugin.
    /// </summary>
    /// <param name="environment">An instance of NClassEnvironment.</param>
    public PDFExportPlugin(NClassEnvironment environment)
      : base(environment)
    {
      menuItem = new ToolStripMenuItem
                   {
                     Text = Strings.MenuTitle,
                     Image = Resources.Document_pdf_16,
                     ToolTipText = Strings.MenuToolTip
                   };
      menuItem.Click += menuItem_Click;
    }

    #endregion

    // ========================================================================
    // Event handling

    #region === Event handling

    /// <summary>
    /// Starts the export.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">Additional information.</param>
    void menuItem_Click(object sender, System.EventArgs e)
    {
      Launch();
    }

    #endregion

    // ========================================================================
    // Properties

    #region === Properties

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
    /// Gets the menu item used to start the plugin.
    /// </summary>
    public override ToolStripItem MenuItem
    {
      get
      {
        return menuItem;
      }
    }

    #endregion

    // ========================================================================
    // Methods

    #region === Methods

    /// <summary>
    /// Starts the functionality of the plugin.
    /// </summary>
    protected void Launch()
    {
      if(!Workspace.HasActiveProject)
      {
        return;
      }

      string fileName;
      using(SaveFileDialog dialog = new SaveFileDialog())
      {
        dialog.Filter = Strings.SaveDialogFilter;
        dialog.RestoreDirectory = true;
        if(dialog.ShowDialog() == DialogResult.Cancel)
        {
          return;
        }
        fileName = dialog.FileName;
      }

      PDFExportOptions optionsForm = new PDFExportOptions();
      if(optionsForm.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }
      Padding padding = new Padding((int)new XUnit(optionsForm.PDFPadding.Left, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Top, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Right, optionsForm.Unit).Point,
                                    (int)new XUnit(optionsForm.PDFPadding.Bottom, optionsForm.Unit).Point);

      MainForm mainForm = null;
      foreach(Form form in Application.OpenForms)
      {
        if(form is MainForm)
        {
          mainForm = (MainForm)form;
          break;
        }
      }
      PDFExportProgress.ShowAsync(mainForm);

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

      if(new PDFExportFinished().ShowDialog() == DialogResult.OK)
      {
        Process.Start(fileName);
      }
    }

    #endregion
  }
}