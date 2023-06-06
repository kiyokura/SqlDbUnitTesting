using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleUnitTestUsingNuGetPackage
{
  internal class VisualStudioWhere
  {
    internal static VisualStudioInstanceInfo[] GetVisualStudioInstanceInfo()
    {
      var query = new SetupConfiguration();
      var query2 = (ISetupConfiguration2)query;
      var e = query2.EnumAllInstances();

      var helper = (ISetupHelper)query;
      int fetched;
      var instances = new ISetupInstance[1];
      var list = new List<VisualStudioInstanceInfo>();
      do
      {
        e.Next(1, instances, out fetched);
        if (fetched > 0)
        {
          var ret = GetInstanceInfo(instances[0], helper);
          if (ret != null)
          {
            list.Add(ret);
          }
        }
      }
      while (fetched > 0);

      return list.ToArray();
    }

    private static VisualStudioInstanceInfo GetInstanceInfo(ISetupInstance instance, ISetupHelper helper)
    {
      var instance2 = (ISetupInstance2)instance;
      var state = instance2.GetState();
      if ((state & InstanceState.Local) == InstanceState.Local)
      {
        return new VisualStudioInstanceInfo(
          instance.GetInstallationVersion(),
          instance.GetDisplayName(),
          instance2.GetInstallationPath(),
          instance.GetInstallationName().ToLower().Contains("preview")
          );
      }
      else
      {
        return null;
      }
    }

    internal class VisualStudioInstanceInfo
    {
      public string Version { get; }
      public string Name { get; }
      public string InstallationPath { get; }
      public bool IsPreview { get; set; }

      public VisualStudioInstanceInfo(string version, string name, string installationPath, bool isPreview)
      {
        Version = version;
        Name = name;
        InstallationPath = installationPath;
        IsPreview = isPreview;
      }
    }
  }
}
