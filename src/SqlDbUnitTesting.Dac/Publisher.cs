using Microsoft.SqlServer.Dac;

namespace SqlDbUnitTesting.Dac
{
  public class Publisher
  {
    /// <summary>
    /// Publish dacpac to database
    /// </summary>
    /// <param name="connectionString">connection string</param>
    /// <param name="targetDatbase">taget database name</param>
    /// <param name="dacpacfile">dacpac file</param>
    public void Publish(string connectionString, string targetDatbase, string dacpacfile)
    {
      var dac = new DacServices(connectionString);
      var dacpac = DacPackage.Load(dacpacfile);
      dac.Deploy(dacpac, targetDatbase, true);
    }
  }
}
