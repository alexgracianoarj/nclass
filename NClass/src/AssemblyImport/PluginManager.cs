using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NClass.Core;
using NClass.CSharp;
using NClass.DiagramEditor.ClassDiagram;
using NClass.GUI;

namespace NClass.AssemblyImport
{
	public class PluginManager : SimplePlugin
	{
		public PluginManager(NClassEnvironment environment) : base(environment)
		{
		}

		public override string Name
		{
			get { return "Assembly Importer"; }
		}

		public override string Author
		{
			get { return "Malte Ried"; }
		}

		public override string MenuText
		{
			get { return "&Import assembly..."; }
		}

		protected override void Launch()
		{
			if (Workspace.HasActiveProject)
			{
				string fileName;
				using (OpenFileDialog dialog = new OpenFileDialog())
				{
					dialog.Filter = "Assemblies (*.exe, *.dll)|*.exe;*.dll";
					if (dialog.ShowDialog() == DialogResult.Cancel)
						return;
					fileName = dialog.FileName;
				}

				ImportSettings settings = new ImportSettings();
				using (ImportSettingsForm settingsForm = new ImportSettingsForm(settings))
				{
					if (settingsForm.ShowDialog() == DialogResult.OK)
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
