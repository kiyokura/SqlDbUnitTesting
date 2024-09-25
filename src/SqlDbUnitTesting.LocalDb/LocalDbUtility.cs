using MartinCostello.SqlLocalDb;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SqlDbUnitTesting.LocaDb
{
  public static class LocalDbUtility
  {
    /// <summary>
    /// Create an instance of SQL Server LocalDB
    /// </summary>
    /// <param name="instanceName">instance name</param>
    /// <param name="version">version of SQL Serer LocalDB</param>
    /// <param name="isRecreate">If an instance of the same name exists, destroy it before creating it</param>
    public static void CreateInstance(string instanceName, string version, bool isRecreate)
    {
      var localDb = new SqlLocalDbApi();
      if (localDb.GetInstanceNames().Contains(instanceName))
      {
        if (isRecreate)
        {
          // For details of options, see API Reference : https://docs.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis
          localDb.StopInstance(instanceName, StopInstanceOptions.KillProcess, new TimeSpan(0, 0, 30));
          localDb.DeleteInstance(instanceName, true);
        }
        else
        {
          return;
        }
      }
      localDb.CreateInstance(instanceName, version);
    }

    /// <summary>
    /// Create a database under the instance folde
    /// </summary>
    /// <param name="instanceName">instance name</param>
    /// <param name="databaseName">database name</param>
    /// <param name="collation">Collation(default:SQL_Latin1_General_CP1_CI_AS)</param>
    /// <param name="isPartialContainedDatabase">true : Make database by Partially Contained Database</param>
    /// <remarks>
    /// if deploying dacpac, Partially Contained Database settings must also be done in dacpac.
    /// </remarks>
    public static void CreateDatabase(string instanceName, string databaseName, string collation = "SQL_Latin1_General_CP1_CI_AS", bool isPartialContainedDatabase = false)
    {
      var connectionString = GetConnectionString(instanceName, "master");
      var mdfPath = System.IO.Path.Combine(GetInstancePath(instanceName), databaseName + ".mdf");
      var sql = new StringBuilder();
      if (isPartialContainedDatabase)
      {
        sql.AppendLine("EXEC sp_configure 'contained database authentication', 1;");
        sql.AppendLine("RECONFIGURE;");
      }
      sql.AppendLine($"CREATE DATABASE {databaseName} ON (NAME = '{databaseName}', FILENAME='{mdfPath}') COLLATE {collation};");
      if (isPartialContainedDatabase)
      {
        sql.AppendLine($"ALTER DATABASE [{databaseName}] SET CONTAINMENT = PARTIAL WITH NO_WAIT;");
      }

      using (var cn = new System.Data.SqlClient.SqlConnection(connectionString))
      {
        cn.Open();
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandText = sql.ToString();
          cmd.CommandType = CommandType.Text;
          cmd.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Get the connection string for SQL Serer LocalDB
    /// </summary>
    /// <param name="instanceName">instance name</param>
    /// <param name="databaseName">database name</param>
    /// <returns>connection string for the database of the SQL Serer LocalDB</returns>
    public static string GetConnectionString(string instanceName, string databaseName)
    {
      const string connectionStringBase = "Data Source=(localdb)\\{0};Initial Catalog={1};Integrated Security=True;Persist Security Info=True;Pooling=False;";
      return GetConnectionString(connectionStringBase, instanceName, databaseName);
    }

    /// <summary>
    /// Get the connection string for SQL Serer LocalDB
    /// </summary>
    /// <param name="connectionStringBase">connection string that instancename and databasename are place folder</param>
    /// <param name="instanceName">instance name</param>
    /// <param name="databaseName">database name</param>
    /// <returns>connection string for the database of the SQL Serer LocalDB</returns>
    public static string GetConnectionString(string connectionStringBase, string instanceName, string databaseName)
    {
      return string.Format(connectionStringBase, instanceName, databaseName);
    }

    /// <summary>
    /// Get the full path of the directory containing the SQL server LocalDB instance files for the current user.
    /// </summary>
    /// <param name="instanceName">instance name</param>
    /// <returns>full path of the directory containing the SQL server LocalDB instance</returns>
    public static string GetInstancePath(string instanceName)
    {
      return System.IO.Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instanceName);
    }
  }
}
