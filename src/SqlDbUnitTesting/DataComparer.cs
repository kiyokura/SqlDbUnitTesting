using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlDbUnitTesting
{
  /// <summary>
  ///  Compare the DataTable with the result set
  /// </summary>
  public static class DataComparer
  {
    /// <summary>
    /// Compare the DataTable with the dynamic result set returned by Dapper
    /// </summary>
    /// <param name="expected">DataTable</param>
    /// <param name="actual">the dynamic result set returned by Dapper</param>
    /// <param name="message">If A and B do not match, a message is set.</param>
    /// <returns>
    /// true : match, false: do not match.
    /// </returns>
    [Obsolete]
    public static bool IsMatch(DataTable expected, IEnumerable<dynamic> actual, out string message)
    {
      if (expected == null)
      {
        throw new ArgumentNullException("expected");
      }
      if (actual == null)
      {
        throw new ArgumentNullException("actual");
      }
      if (!(actual is IEnumerable<IDictionary<string, object>>))
      {
        throw new ArgumentException("actual must be IEnumerable<IDictionary<string, object>>", "actual");
      }

      var rows = (IEnumerable<IDictionary<string, object>>)actual;

      // numbers of rows
      var expectedRowCount = expected.Rows.Count;
      var actualRowCount = rows.Count();
      if (actualRowCount != expectedRowCount)
      {
        message = $"numbers of rows do not match: expected [{expectedRowCount}], actual [{actualRowCount}] ";
        return false;
      }

      // number of columns
      var expectedColCount = expected.Columns.Count;
      var actualColCount = rows.First().Keys.Count;
      if (actualColCount != expectedColCount)
      {
        message = $"number of columns do not match: expected [{expectedColCount}], actual [{actualColCount}] ";
        return false;
      }

      // names of columns
      var actualColmuns = rows.First().Keys.Select(x => x.ToUpper()).ToList();
      var colmunIndex = 0;
      foreach (var col in expected.Columns)
      {
        var actualColumnName = actualColmuns[colmunIndex];
        var expectedColumnName = ((DataColumn)col).ColumnName.ToUpper();
        if (actualColumnName != expectedColumnName)
        {
          message = $"column names do not match : expected [{expectedColumnName}], actual [{actualColumnName}], index [{colmunIndex}]";
          return false;
        }
        colmunIndex += 1;
      }

      // content of data
      var rowindex = 0;
      foreach (var row in rows)
      {
        foreach (var col in row.Keys)
        {
          var actualValue = row[col];
          var expectedValuevalDt = expected.Rows[rowindex][col];

          if (Convert.ToString(actualValue) != Convert.ToString(expectedValuevalDt))
          {
            // ToDo: ouptput colmun Name and RowNumber, and values;
            message = $"contents do not match: rowindex [{rowindex}] , column name [{col}] , actual [{actualValue}], expected [{expectedValuevalDt}]";
            return false;
          }
        }
        rowindex += 1;
      }

      message = "";
      return true;
    }

    /// <summary>
    /// Compare the DataTable with the dynamic result set returned by Dapper
    /// </summary>
    /// <param name="expected">DataTable</param>
    /// <param name="actual">SqlDataReader</param>
    /// <param name="message">If A and B do not match, a message is set.</param>
    /// <returns>
    /// true : match, false: do not match.
    /// </returns>
    public static bool IsMatch(DataTable expected, System.Data.SqlClient.SqlDataReader actual, out string message)
    {
      if (expected == null)
      {
        throw new ArgumentNullException("expected");
      }
      if (actual == null)
      {
        throw new ArgumentNullException("actual");
      }

      // number of columns
      var expectedColCount = expected.Columns.Count;
      var actualColCount = actual.FieldCount;
      if (actualColCount != expectedColCount)
      {
        message = $"number of columns do not match: expected [{expectedColCount}], actual [{actualColCount}] ";
        return false;
      }

      // number of expected rows
      var expectedRowCount = expected.Rows.Count;

      // content of data
      var rowindex = 0;
      while (actual.Read())
      {
        if (rowindex < expectedRowCount)
        {
          foreach (var col in expected.Columns)
          {
            var colName = ((DataColumn)col).ColumnName.ToUpper();
            var actualValue = GetActualValue(actual, colName);
            var expectedValue = expected.Rows[rowindex][colName];

            if (actualValue.ToString() != expectedValue.ToString())
            {
              message = $"contents do not match: rowindex [{rowindex}] , column name [{col}] , expected [{expectedValue}], actual [{actualValue}]";
              return false;
            }
          }
        }
        rowindex += 1;
      }

      // numbers of actual rows
      var actualRowCount = rowindex;
      if (actualRowCount != expectedRowCount)
      {
        message = $"numbers of rows do not match: expected [{expectedRowCount}], actual [{actualRowCount}] ";
        return false;
      }

      message = "";
      return true;
    }

    private static object GetActualValue(System.Data.SqlClient.SqlDataReader actual, string colName)
    {
      var providerType = actual.GetSchemaTable().AsEnumerable()
                               .Where(x => string.Compare(x["ColumnName"].ToString(), colName, true) == 0)
                               .Select(x => (SqlDbType)(int)x["ProviderType"]).FirstOrDefault();

      if (providerType == SqlDbType.Date && actual[colName] != DBNull.Value)
      {
        return ((DateTime)actual[colName]).ToShortDateString();
      }
      return actual[colName];
    }
  }
}
