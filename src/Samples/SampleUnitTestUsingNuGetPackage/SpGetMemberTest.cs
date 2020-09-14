using NUnit.Framework;
using SqlDbUnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleUnitTestUsingNuGetPackage
{
  [TestFixture]
  public class SpGetMemberTest
  {
    private GlobalSettings Settings = GlobalSettings.Instance;
    


    [Test]
    public void SpGetMember()
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(Settings.ConnectionString))
      {
        cn.Open();
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = "spGetMember";

          using (var dr = cmd.ExecuteReader())
          {
            var message = "";
            var ismatch = ResultChecker.IsMatch(dr, Path.Combine(Settings.BasePath, "spGetMemberTestData.xlsx"), "result", out message);
            Assert.AreEqual(true, ismatch, message);
          }
        }
      }
    }
  }
}
