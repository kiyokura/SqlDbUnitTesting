# SqlDbUnitTesting

Libraries that supports unit testing of the SQL Server Database Project.

For .NET Framework 4.7.2.


## Libraries
### SqlDbUnitTesting
- DataLoader
  - Load Excel files into a test database.
- ResultChecker
  - Check if the data in a record and an Excel file match.

### SqlDbUnitTesting.DAC
- Builder
  - Build dacpac from *.sqlproj.
- Publisher
  - Publish dacpac to test database.
### SqlDbUnitTesting.LocalDb
- LocalDbUtility
  - Utility to manipulate SQL Server LocalDb.
    - Wrapper libraly of SqlLocalDB.exe.
