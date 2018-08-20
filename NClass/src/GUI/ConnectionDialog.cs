using System;
using System.Windows.Forms;
using Microsoft.Data.ConnectionUI;

using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

using System.Threading;

using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;

namespace NClass.GUI
{
    public partial class ConnectionDialog : Form
    {
        ConnectionsSettings connectionsSettings;

        private ConnectionSettings _connection;

        public ConnectionSettings Connection
        {
            get { return _connection; }
            set 
            { 
                _connection = value; 
            }
        }

        public ConnectionDialog()
        {
            InitializeComponent();

            connectionsSettings = ConnectionsSettings.Load();

            PopulateConnections();

            PopulateServerTypes();

            serverTypeComboBox.SelectedIndexChanged += OnServerTypeSelectedIndexChanged;

            chkNHibernateMapping.Checked = CodeGenerator.Settings.Default.GenerateNHibernateMapping;
            cboDefaultIdGenerator.DataSource = Enum.GetValues(typeof(CodeGenerator.IdentityGeneratorType));
            cboDefaultIdGenerator.SelectedItem = CodeGenerator.Settings.Default.DefaultIdentityGenerator;
            chkDefaultFetching.Checked = CodeGenerator.Settings.Default.DefaultLazyFetching;
            chkUseUnderscoreAndLowercaseInDB.Checked = CodeGenerator.Settings.Default.UseUnderscoreAndLowercaseInDB;

            Load += OnConnectionDialogLoad;
        }

        private void OnConnectionDialogLoad(object sender, EventArgs e)
        {
            // If no connection has been passed in create a new one
            if (connectionsSettings.Connections.Count == 0)
            {
                Connection = CreateNewConnection();
            }
            else
            {
                var Connection = connectionsSettings.Connections.SingleOrDefault(x => x.Id.Equals(connectionsSettings.LastUsedConnection));

                if (Connection != null)
                {
                    cboConnection.SelectedItem = Connection;
                }
                else
                {
                    Connection = (ConnectionSettings)cboConnection.SelectedItem;
                }
            }

            BindData();
        }

        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            connectionsSettings.DeleteConnection(Connection);

            PopulateConnections();

            if (connectionsSettings.Connections.Count == 0)
            {
                Connection = CreateNewConnection();
                BindData();
            }
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            Connection = CreateNewConnection();

            int index = connectionsSettings.SaveConnection(Connection);

            PopulateConnections();

            cboConnection.SelectedIndex = index;
        }

        private void OnServerTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverTypeComboBox.SelectedIndex == -1)
            {
                // Nothing selected
                return;
            }

            // Set a default connection string if user changes server type.
            var serverType = (SqlType)serverTypeComboBox.SelectedItem;
            connectionStringTextBox.Text = GetDefaultConnectionStringForServerType(serverType);
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            CaptureConnection();

            CodeGenerator.Settings.Default.GenerateNHibernateMapping = chkNHibernateMapping.Checked;
            CodeGenerator.Settings.Default.DefaultIdentityGenerator = (CodeGenerator.IdentityGeneratorType)cboDefaultIdGenerator.SelectedItem;
            CodeGenerator.Settings.Default.DefaultLazyFetching = chkDefaultFetching.Checked;
            CodeGenerator.Settings.Default.UseUnderscoreAndLowercaseInDB = chkUseUnderscoreAndLowercaseInDB.Checked;
            CodeGenerator.Settings.Default.Save();
        }

        private ConnectionSettings CreateNewConnection(SqlType serverType = SqlType.SqlServer)
        {
            // Default connection settings.
            var connectionString = GetDefaultConnectionStringForServerType(serverType);

            return new ConnectionSettings
                       {
                           Id = Guid.NewGuid(),
                           Name = Translations.Strings.Untitled,
                           ConnectionString = connectionString,
                           ServerType = serverType,
                           Schema = null,
                       };
        }

        private string GetDefaultConnectionStringForServerType(SqlType serverType)
        {
            switch (serverType)
            {
                case SqlType.Db2:
                    return StringConstants.DB2_CONN_STR_TEMPLATE;
                case SqlType.Oracle:
                    return StringConstants.ORACLE_CONN_STR_TEMPLATE;
                case SqlType.SqlServer:
                    return StringConstants.SQL_CONN_STR_TEMPLATE;
                case SqlType.SqlServerCe:
                    return StringConstants.SQL_CE_CONN_STR_TEMPLATE;
                case SqlType.MySql:
                    return StringConstants.MYSQL_CONN_STR_TEMPLATE;
                case SqlType.PostgreSql:
                    return StringConstants.POSTGRESQL_CONN_STR_TEMPLATE;
                case SqlType.SQLite:
                    return StringConstants.SQLITE_CONN_STR_TEMPLATE;
                default:
                    return StringConstants.FIREBIRD_CONN_STR_TEMPLATE;
            }
        }

        private void BindData()
        {
            serverTypeComboBox.SelectedIndexChanged -= OnServerTypeSelectedIndexChanged;

            txtName.Text = Connection.Name;
            serverTypeComboBox.SelectedItem = Connection.ServerType;
            connectionStringTextBox.Text = Connection.ConnectionString;
            cboSchema.DataSource = new string[] { Connection.Schema };
            txtTextPrefix.Text = Connection.PrefixRemoval;

            serverTypeComboBox.SelectedIndexChanged += OnServerTypeSelectedIndexChanged;
        }

        private void CaptureConnection()
        {
            Connection.Name = txtName.Text;
            Connection.ServerType = (SqlType)serverTypeComboBox.SelectedItem;
            Connection.ConnectionString = connectionStringTextBox.Text.Trim();
            Connection.Schema = (string)cboSchema.SelectedItem;
            Connection.PrefixRemoval = txtTextPrefix.Text.Trim();

            connectionsSettings.LastUsedConnection = Connection.Id;

            connectionsSettings.SaveConnection(Connection);
        }

        private void PopulateConnections()
        {
            connectionsSettings = ConnectionsSettings.Load();

            cboConnection.DataSource = connectionsSettings.Connections;
            cboConnection.DisplayMember = "Name";
        }

        private void PopulateServerTypes()
        {
            serverTypeComboBox.DataSource = Enum.GetValues(typeof(SqlType));
            serverTypeComboBox.SelectedIndex = 0;
        }

        private void OnConnectionStringButtonClick(object sender, EventArgs e)
        {
            // Using the microsoft connection dialog as used in visual studio
            // http://archive.msdn.microsoft.com/Connection/Release/ProjectReleases.aspx?ReleaseId=3863
            var dialogResult = DialogResult.Cancel;
            var connectionString = string.Empty;

            var dcd = new DataConnectionDialog();

            try
            {
                CaptureConnection();

                var dcs = new DataConnectionConfiguration(null);
                dcs.LoadConfiguration(dcd, Connection.ServerType);

                if (Connection.ConnectionString != GetDefaultConnectionStringForServerType(Connection.ServerType))
                {
                    dcd.ConnectionString = Connection.ConnectionString;
                }

                dialogResult = DataConnectionDialog.Show(dcd);
                connectionString = dcd.ConnectionString;
            }
            catch (ArgumentException)
            {
                dcd.ConnectionString = string.Empty;
                dialogResult = DataConnectionDialog.Show(dcd);
            }
            finally
            {
                if (dialogResult == DialogResult.OK)
                {
                    Connection.ConnectionString = connectionString;
                    BindData();
                }
            }

        }

        bool isProcessRunning = false;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isProcessRunning)
                return;

            IList<DatabaseDbSchema> schemas = new List<DatabaseDbSchema>();

            string connectionStr = connectionStringTextBox.Text.Trim();
            SqlType serverType = (SqlType)serverTypeComboBox.SelectedItem;

            // Initialize the dialog that will contain the progress bar
            ProgressDialog progressDialog = new ProgressDialog();

            // Set the dialog to operate in indeterminate mode
            progressDialog.SetIndeterminate(true);

            // Initialize the thread that will handle the background process
            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    try
                    {
                        isProcessRunning = true;

                        //Create the database reader object.
                        var metadataReader = new DatabaseReader(connectionStr, serverType);
                        schemas = metadataReader.AllSchemas();

                        if (cboSchema.InvokeRequired)
                            cboSchema.BeginInvoke(new Action(() =>
                                {
                                    cboSchema.DataSource = schemas.Select(x => x.Name).ToList();
                                }));

                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Invoke(new MethodInvoker(() =>
                            {
                                MessageBox.Show(
                                    this,
                                    ex.Message,
                                    Translations.Strings.Error,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }));
                    }
                    finally
                    {
                        isProcessRunning = false;

                        if (progressDialog.InvokeRequired)
                            progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    }
                }));

            // Sets to single thread apartment (STA) mode before OLE calls
            backgroundThread.SetApartmentState(ApartmentState.STA);

            // Start the background process thread
            backgroundThread.Start();

            // Open the dialog
            progressDialog.ShowDialog();
        }

        private void cboConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            Connection = (ConnectionSettings)cboConnection.SelectedItem;
            BindData();
        }

    }

    [Serializable]
    public class ConnectionSettings
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public SqlType ServerType { get; set; }
        public string Schema { get; set; }
        public string PrefixRemoval { get; set; }
    }

    [Serializable]
    public class ConnectionsSettings
    {
        [NonSerialized]
        private static string pathFile = Application.StartupPath + @"\Connections\connections.bin";
        public Guid? LastUsedConnection { get; set; }
        public List<ConnectionSettings> Connections { get; set; }

        public ConnectionsSettings()
        {
            Connections = new List<ConnectionSettings>();
        }

        public void Save()
        {
            using (var fileStream = new FileStream(pathFile, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fileStream, this);
            }
        }

        public static ConnectionsSettings Load()
        {
            ConnectionsSettings connectionsSettings = new ConnectionsSettings();

            if (File.Exists(pathFile))
            {
                using (var fileStream = new FileStream(pathFile, FileMode.Open))
                {
                    connectionsSettings = (ConnectionsSettings)new BinaryFormatter().Deserialize(fileStream);
                }
            }

            return connectionsSettings;
        }

        public int SaveConnection(ConnectionSettings connection)
        {
            var connItem = Connections.SingleOrDefault(x => x.Id.Equals(connection.Id));

            if (connItem == null)
            {
                Connections.Add(connection);
            }
            else
            {
                connItem.Name = connection.Name;
                connItem.ConnectionString = connection.ConnectionString;
                connItem.ServerType = connection.ServerType;
                connItem.Schema = connection.Schema;
                connItem.PrefixRemoval = connection.PrefixRemoval;
            }

            Save();

            return Connections.FindIndex(x => x.Id.Equals(connection.Id));
        }

        public void DeleteConnection(ConnectionSettings connection)
        {
            var connItem = Connections.SingleOrDefault(x => x.Id.Equals(connection.Id));

            if (connItem != null)
            {
                Connections.Remove(connItem);
            }

            Save();
        }
    }
}
