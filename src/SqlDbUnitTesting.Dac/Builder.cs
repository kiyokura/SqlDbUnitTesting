using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;
using System.IO;

namespace SqlDbUnitTesting.Dac
{
  /// <summary>
  /// Builder of dacpac from  .sqlproj
  /// </summary>
  public class Builder
  {
    /// <summary>
    /// Build dacpac from ".sqlproj" file
    /// </summary>
    /// <param name="sqlProjectFilePath">path of target sqlproj file</param>
    /// <param name="outputDir">Directory to output dacpac files</param>
    /// <param name="outputFilename">Name of the dacpac file to output</param>
    /// <param name="buildLogFile">build log file name</param>
    /// <returns></returns>
    public bool Build(string sqlProjectFilePath, string outputDir, string outputFilename, string buildLogFile = "SqlDbUnitTesting.Dac.Buildlog.txt")
    {
      if (Path.GetExtension(outputFilename).ToLower() == ".dacpac")
        outputFilename = Path.GetFileNameWithoutExtension(outputFilename);

      var project = ProjectCollection.GlobalProjectCollection.LoadProject(sqlProjectFilePath);
      project.SetProperty("OutputPath", outputDir);
      project.SetProperty("SqlTargetName", outputFilename);
      var fileLogger = new FileLogger { Parameters = @"logfile=" + buildLogFile };
      return project.Build(new[] { fileLogger });
    }

  }
}
