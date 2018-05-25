using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;

namespace NClass.GUI
{
    public partial class DatabaseObjects : Form
    {
        private IList<DatabaseTable> tables;
        private IList<DatabaseView> views;

        public DatabaseObjects(IList<DatabaseTable> tables, IList<DatabaseView> views)
        {
            this.tables = tables;
            this.views = views;

            InitializeComponent();
        }

        public bool ConvertToPascalCase { get; set; }

        public IList<DatabaseTable> Tables
        {
            get { return tables; }
        }

        public IList<DatabaseView> Views
        {
            get { return views; }
        }

        private void DatabaseObjects_Load(object sender, EventArgs e)
        {
            ConvertToPascalCase = true;

            TreeNode rootTables = triStateTreeView1.Nodes.Add("Tables");

            foreach(DatabaseTable table in tables)
            {
                rootTables.Nodes.Add(table.Name);
            }

            TreeNode rootViews = triStateTreeView1.Nodes.Add("Views");

            foreach (DatabaseView view in views)
            {
                rootViews.Nodes.Add(view.Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (TreeNode tableNode in triStateTreeView1.Nodes[0].Nodes)
            {
                if (!tableNode.Checked)
                    tables.Remove(tables.Single(t => t.Name == tableNode.Text));
            }

            foreach (TreeNode viewNode in triStateTreeView1.Nodes[1].Nodes)
            {
                if (!viewNode.Checked)
                    views.Remove(views.Single(v => v.Name == viewNode.Text));
            }

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void chkConvertObjNamesPascalCase_CheckedChanged(object sender, EventArgs e)
        {
            ConvertToPascalCase = chkConvertObjNamesPascalCase.Checked;
        }
    }
}
