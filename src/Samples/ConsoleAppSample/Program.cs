using SqlDbUnitTesting.Dac;
using SqlDbUnitTesting.LocaDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleAppSample
{
  class Program
  {
    static void Main(string[] _)
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
      var vsinfo = GetVisualStudioInfo();
      var globalProperty = new Dictionary<string, string>()
      {
        { "FromUnitTest","True"},
        { "SQLDBExtensionsPath",$"{vsinfo.path}\\MSBuild\\Microsoft\\VisualStudio\\{vsinfo.version}\\SSDT"},
        { "VsInstallRoot",$"{vsinfo.path}"}
      };

      var builder = new Builder();
      return builder.Build(projectPath, dacpacDir, dacpacFileName, buildLogFile, globalProperty);
    }

    static void DeployDacpac(string connectionString, string targetDatabase, string dacpacFile)
    {
      var publisher = new Publisher();
      publisher.Publish(connectionString, targetDatabase, dacpacFile);
    }

    static (string version, string path) GetVisualStudioInfo()
    {
      foreach (var info in VisualStudioWhere.GetVisualStudioInstanceInfo().Where(x => !x.IsPreview).OrderByDescending(x => x.Version))
      {
        var majorVersion = $"v{info.Version.Split('.')[0]}.0";
        var path = info.InstallationPath;
        if (Directory.Exists($"{info.InstallationPath}\\MSBuild\\Microsoft\\VisualStudio\\{majorVersion}\\SSDT"))
        {
          return (majorVersion, path);
        }
      }
      return ("", "");
    }
  }
}
