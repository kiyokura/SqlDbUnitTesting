using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SqlDbUnitTesting
{
  /// <summary>
  /// Compare the resultset with the excel data sheet.
  /// </summary>
  public static class ResultChecker
  {
    /// <summary>
    /// Compare the resultset with the excel data sheet.
    /// </summary>
    /// <param name="result">resultset</param>
    /// <param name="excelFile">The path of the target excel file</param>
    /// <param name="sheetName">The names of the target sheet</param>
    /// <param name="message">If A and B do not match, a message is set.</param>
    /// <returns>
    /// true : match, false: do not match.
    /// </returns>
    [Obsolete]
    public static object IsMatch(IEnumerable<dynamic> result, string excelFile, string sheetName, out string message)
    {
      var dt = ExcelReader.Read(excelFile, sheetName);
      return DataComparer.IsMatch(dt, result, out message);
    }

    /// <summary>
    /// Compare the resultset with the excel data sheet.
    /// </summary>
    /// <param name="result">resultset</param>
    /// <param name="excelFile">The path of the target excel file</param>
    /// <param name="sheetName">The names of the target sheet</param>
    /// <param name="message">If A and B do not match, a message is set.</param>
    /// <returns>
    /// true : match, false: do not match.
    /// </returns>
    public static object IsMatch(SqlDataReader result, string excelFile, string sheetName, out string message)
    {
      var dt = ExcelReader.Read(excelFile, sheetName);
      return DataComparer.IsMatch(dt, result, out message);
    }
  }
}
