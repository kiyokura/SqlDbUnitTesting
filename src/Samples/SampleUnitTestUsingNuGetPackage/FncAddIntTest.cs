using NUnit.Framework;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleUnitTestUsingNuGetPackage
{
  [TestFixture]
  public class FncAddIntTest
  {
    private readonly GlobalSettings Settings = GlobalSettings.Instance;

    [Test]
    public void FncAddIntCanAdd()
    {
      var expected = 2;
      using (var cn = new System.Data.SqlClient.SqlConnection(Settings.ConnectionString))
      {
        cn.Open();
        int actual = cn.ExecuteScalar<int>("SELECT dbo.fncAddInt(1,1) ", cn);
        Assert.AreEqual(expected, actual);
      }
    }
  }
}
