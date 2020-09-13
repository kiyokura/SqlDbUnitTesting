using SqlDbUnitTesting.Dac;
using SqlDbUnitTesting.LocaDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSample
{
  class Program
  {
    static void Main(string[] args)
    {
      var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      var projectPath = Path.Combine(basePath, @"..\..\..\MyDataBase\MyDataBase.sqlproj");
      var dacpacDir = Path.Combine(basePath, @"dacpac");
      var dacpacFileName = "output.dacpac";

      // Build dacpac
      Console.WriteLine("Buidling dacpac ...");
      if (!BuildDacpac(projectPath, dacpacDir, dacpacFileName, @"logs/build.log"))
      {
        Console.WriteLine(@"Generating dacpac file failed. See :logs/build.log");
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
        return;
      }
      Console.WriteLine("...done. ");

      // Create SQL Server LocalDB instance
      //  Need : installing SQL Server LocalDB.
      Console.WriteLine("Creating LocalDb instance ... ");
      LocalDbUtility.CreateInstance("TestDbInstance", "13.0", true);
      LocalDbUtility.CreateDatabase("TestDbInstance", "MyTestDB");
      var connectionString = LocalDbUtility.GetConnectionString("TestDbInstance", "MyTestDB");

      Console.WriteLine("...done. ");

      // Deploy dacpac to LocalDb
      Console.WriteLine("Deploying dacpac ...");
      DeployDacpac(connectionString, "MyTestDB", Path.Combine(basePath, @"dacpac", dacpacFileName));
      Console.WriteLine("...done. ");

      Console.WriteLine("\nPress any key to exit.");
      Console.ReadKey();
    }

    static bool BuildDacpac(string projectPath, string dacpacDir, string dacpacFileName, string buildLogFile)
    {
      var builder = new Builder();
      return builder.Build(projectPath, dacpacDir, dacpacFileName, buildLogFile);
    }

    static void DeployDacpac(string connectionString, string targetDatabase, string dacpacFile)
    {
      var publisher = new Publisher();
      publisher.Publish(connectionString, targetDatabase, dacpacFile);
    }
  }
}
