using NUnit.Framework;
using SqlDbUnitTesting;
using SqlDbUnitTesting.Dac;
using SqlDbUnitTesting.LocaDb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleUnitTestUsingNuGetPackage
{
  [SetUpFixture]
  public class Startup
  {

    [OneTimeSetUp]
    public void InitializeTest()
    {
      var settings = GlobalSettings.Instance;

      // create dacpac from sqlproj
      var builder = new SqlDbUnitTesting.Dac.Builder();

      var vsinfo = GetVisualStudioInfo();
      var globalProperty = new Dictionary<string, string>()
      {
        { "FromUnitTest","True"},
        { "SQLDBExtensionsPath",$"{vsinfo.path}\\MSBuild\\Microsoft\\VisualStudio\\{vsinfo.version}\\SSDT"},
        { "VsInstallRoot",$"{vsinfo.path}"}
      };

      if (!builder.Build(settings.SqlProjectFile, settings.DacpacOutputDir, settings.DacpacFileName, "logs/build.log", globalProperty))
      {
        throw new Exception("create dacpac failed.");
      }

      // initialize localdb
      LocalDbUtility.CreateInstance(settings.LocalDbInstanceName, settings.LocalDbVersion, true);
      LocalDbUtility.CreateDatabase(settings.LocalDbInstanceName, settings.DataBaseName);

      // publish dacpac to localdb
      var publisher = new Publisher();
      publisher.Publish(settings.ConnectionString, settings.DataBaseName, settings.DacpacFilePath);

      // Load initilize data
      var dt = ExcelReader.Read(Path.Combine(settings.BasePath, "Initialdata.xlsx"), "Member");
      var loader = new DataLoader(settings.ConnectionString);
      loader.Load(dt, true);
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
