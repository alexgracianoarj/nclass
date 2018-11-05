namespace NClass.GUI
{
    public class StringConstants
    {
        public static string DB2_CONN_STR_TEMPLATE =
            "Server=myAddress:myPortNumber;Database=myDataBase;UID=myUsername;PWD=myPassword;";

        public static string ORACLE_CONN_STR_TEMPLATE =
            "User ID=userName;Password=pass;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=dedicated)(SERVICE_NAME=XE)))";

        public static string SQL_CONN_STR_TEMPLATE =
            "Data Source=.\\SQLExpress;Integrated Security=true;AttachDbFilename=MyDataFile.mdf;User Instance=true;";

        public static string SQL_CE_CONN_STR_TEMPLATE =
            "Data Source=MyData.sdf;Persist Security Info=False;";

        public static string POSTGRESQL_CONN_STR_TEMPLATE =
            "Host=localhost;Port=5432;Database=myDataBase;User ID=root;Password=myPassword;";

        public static string MYSQL_CONN_STR_TEMPLATE =
            "Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";

        public static string SQLITE_CONN_STR_TEMPLATE =
            "Data Source=local.db;Foreign Keys=True;Version=3;New=False;Compress=True;";

        public static string FIREBIRD_CONN_STR_TEMPLATE =
            "DataSource=localhost;Port=3050;Database=SampleDatabase.fdb;User=SYSDBA;Password=masterkey;";

        public static string SYBASE_CONN_STR_TEMPLATE =
            "Provider=ASAProv;UID=uidname;PWD=password;DatabaseName=databasename;EngineName=enginename;CommLinks=TCPIP{host=servername}";

        public static string INGRES_CONN_STR_TEMPLATE = 
            "Host=localhost;Port=II7;Database=myDb;User ID=myUser;Password=myPassword;";

        public static string CUBRID_CONN_STR_TEMPLATE =
            "server=localhost;port=33000;database=demodb;user=dba;password=";
    }
}