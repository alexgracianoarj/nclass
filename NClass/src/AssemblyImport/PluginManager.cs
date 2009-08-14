using System.Windows.Forms;
using NClass.CSharp;
using NClass.DiagramEditor.ClassDiagram;
using NClass.GUI;

namespace NClass.AssemblyImport
{
  /// <summary>
  /// Implements the PlugIn-Interface of NClass.
  /// </summary>
  public class PluginManager : SimplePlugin
  {
    /// <summary>
    /// Constructs a new instance of PluginManager.
    /// </summary>
    /// <param name="environment">An instance of NClassEnvironment.</param>
    public PluginManager(NClassEnvironment environment)
      : base(environment)
    {
    }

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name
    {
      get { return "Assembly Importer"; }
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
      get { return "&Import assembly..."; }
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
    /// Starts the functionality of the plugin.
    /// </summary>
    protected override void Launch()
    {
      if(Workspace.HasActiveProject)
      {
        string fileName;
        using(OpenFileDialog dialog = new OpenFileDialog())
        {
          dialog.Filter = "Assemblies (*.exe, *.dll)|*.exe;*.dll";
          if(dialog.ShowDialog() == DialogResult.Cancel)
            return;
          fileName = dialog.FileName;
        }

        ImportSettings settings = new ImportSettings();
        using(ImportSettingsForm settingsForm = new ImportSettingsForm(settings))
        {
          if(settingsForm.ShowDialog() == DialogResult.OK)
          {
            Diagram diagram = new Diagram(CSharpLanguage.Instance);
            NETImport importer = new NETImport(diagram, settings);

            importer.ImportAssembly(fileName);
            Workspace.ActiveProject.Add(diagram);
          }
        }
      }
    }
  }
}
