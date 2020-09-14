using SqlDbUnitTesting.LocaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleUnitTestUsingNuGetPackage
{
  public sealed class GlobalSettings
  {
    public static GlobalSettings Instance { get; } = new GlobalSettings();


    public string BasePath { get; }

    public string SqlProjectFile { get; }

    public string LocalDbInstanceName { get; }

    public string DataBaseName { get; }

    public string LocalDbVersion { get; }

    public string DacpacFileName { get; }

    public string DacpacOutputDir { get; }

    public string DacpacFilePath
    {
      get
      {
        return Path.Combine(DacpacOutputDir, DacpacFileName);
      }
    }

    public string ConnectionString { get; }




    private GlobalSettings()
    {
      BasePath = Path.GetDirectoryName((Assembly.GetExecutingAssembly()).Location);
      SqlProjectFile = GetSqlProjectFile();
      LocalDbInstanceName = ConfigurationManager.AppSettings["LocalDbInstanceName"];
      DataBaseName = ConfigurationManager.AppSettings["DataBaseName"];
      LocalDbVersion = ConfigurationManager.AppSettings["LocalDbVersion"];
      DacpacFileName = ConfigurationManager.AppSettings["DacpacFileName"];
      DacpacOutputDir = GetDacpacOutputDir();
      ConnectionString = LocalDbUtility.GetConnectionString(LocalDbInstanceName, DataBaseName);
    }

    private string GetSqlProjectFile()
    {
      var s = ConfigurationManager.AppSettings["SqlProjectFile"];
      if (IsAbsolatePath(s))
      {
        return s;
      }
      else
      {
        return Path.Combine(BasePath, s);
      }
    }

    private string GetDacpacOutputDir()
    {
      var s = ConfigurationManager.AppSettings["DacpacOutputDir"];
      if (IsAbsolatePath(s))
      {
        return s;
      }
      else
      {
        return Path.Combine(BasePath, s);
      }
    }

    public bool IsAbsolatePath(string path)
    {
      var len = path.Length;
      if (
          (
            2 <= len &&
            (path[0] == Path.DirectorySeparatorChar || path[0] == Path.AltDirectorySeparatorChar) &&
            (path[1] == Path.DirectorySeparatorChar || path[1] == Path.AltDirectorySeparatorChar)
          ) || (
            3 <= len &&
            path[1] == Path.VolumeSeparatorChar &&
            (path[2] == Path.DirectorySeparatorChar || path[2] == Path.AltDirectorySeparatorChar)
          )
         )
      {
        return true;
      }
      return false;
    }
  }
}
