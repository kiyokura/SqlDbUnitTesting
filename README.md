# SqlDbUnitTesting

Libraries that supports unit testing of the SQL Server Database Project.

For .NET Framework 4.7.2.

## NuGet Packages
- https://www.nuget.org/packages/SqlDbUnitTesting/
- https://www.nuget.org/packages/SqlDbUnitTesting.Dac/
- https://www.nuget.org/packages/SqlDbUnitTesting.LocalDb/

## Libraries
### SqlDbUnitTesting
- DataLoader
  - Load Excel files into a test database.
- ResultChecker
  - Check if the data in a record and an Excel file match.
- ExcelReader
  - Read Excel file to DataTable using EPPlus.

### SqlDbUnitTesting.LocalDb
- LocalDbUtility
  - Utility to manipulate SQL Server LocalDb.
    - Tiny wrapper libraly of SqlLocalDB.exe.

### SqlDbUnitTesting.DAC
- Builder
  - Build dacpac from *.sqlproj using Microsoft.Build.
- Publisher
  - Publish dacpac to test database using DacFX.

## Install
Install the package from nuget.org.

```ps
PM> Install-Package SqlDbUnitTesting
PM> Install-Package SqlDbUnitTesting.LocalDb
PM> Install-Package SqlDbUnitTesting.DAC
```

## Usage
See Sample Project : `src\Samples\SampleUnitTestUsingNuGetPackage\SampleUnitTestUsingNuGetPackage.csproj`

### ExcelReader
```cs
var excelfile = @"c:\data.xlsx";
var excelSheetName = "Member";
var dt = ExcelReader.Read(excelfile, excelSheetName);
```
### DataLoader
```cs
var conStr = @"Data Source=(localdb)\TestDB;Initial Catalog=MyDataBase;Integrated Security=True;Persist Security Info=False;Pooling=False;";
var excelfile = @"c:\data.xlsx";
var excelSheetName = "Member"; // use as table name.
var dt = ExcelReader.Read(excelfile, excelSheetName);

var loader = new DataLoader(conStr);
loader.Load(dt, true); 
```

### ResultChecker
```cs
[Test]
public void SpGetMember()
{
  var conStr = @"Data Source=(localdb)\TestDB;Initial Catalog=MyDataBase;Integrated Security=True;Persist Security Info=False;Pooling=False;";
  var expectedDataFile = @"ExpectedResult.xlsx";
  var expectedSheetName  = "expected";
  using (var cn = new System.Data.SqlClient.SqlConnection(conStr))
  {
    cn.Open();
    using (var cmd = cn.CreateCommand())
    {
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = "SomeStoredProcedure"; // SomeStoredProcedure that  that returns records.

      using (var dr = cmd.ExecuteReader())
      {
        var message = "";
        var ismatch = ResultChecker.IsMatch(dr, PexpectedDataFile, expectedSheetName, out message);
        Assert.AreEqual(true, ismatch, message);
      }
    }
  }
}
```


### LocalDbUtility
```cs
var instanceName = @"TestDB"; // instance name
var databaseName = @"MyDataBase"; // databasename
var localDbVer = "13.0"; // LocalDb database version
// Create Instance
LocalDbUtility.CreateInstance(instanceName, localDbVer, true);
// Create Database
LocalDbUtility.CreateDatabase(instanceName, databaseName);
var conStr = LocalDbUtility.LocalDbUtility.GetConnectionString(instanceName, dataaseName);
```

### Builder
```cs
var project = @"C:\Sample\SampleDb.sqlproj";
var outoutdir = @"C:\dacpac";
var dacpacfilename = @"FOO.dacpac";

var builder = new SqlDbUnitTesting.Dac.Builder();
builder.Build(project, outoutdir, dacpacfilename);
```

### Publisher
```cs
var conStr = @"Data Source=(localdb)\TestDB;Initial Catalog=MyDataBase;Integrated Security=True;Persist Security Info=False;Pooling=False;";
var databaseName = @"MyDataBase";
var dacpacfile = @"C:\dacpac\FOO.dacpac";

var publisher = new Publisher();
publisher.Publish(conStr, databaseName, dacpacfile);
```


