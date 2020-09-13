using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace SqlDbUnitTesting
{
  /// <summary>
  /// Read excel file to DataTable
  /// </summary>
  public class ExcelReader
  {
    const string COMMENT_COLUMN_HEADER = "**comment**";

    /// <summary>
    /// Reads Excel file and creates DataTable
    /// </summary>
    /// <param name="excellFilePath">The path of the target excel file.</param>
    /// <param name="sheetName">The name of the target sheet.</param>
    /// <returns>DataTable object</returns>
    /// <remarks>
    /// Read a specified sheet of Excel file into DataTable.
    /// The sheet name is set in the TableName property of the DataTable.
    /// </remarks>
    public static DataTable Read(string excellFilePath, string sheetName)
    {
      return Read(excellFilePath, sheetName, sheetName);
    }

    /// <summary>
    /// Reads Excel file and creates DataTable
    /// </summary>
    /// <param name="excellFilePath">The path of the target excel file.</param>
    /// <param name="sheetName">The name of the target sheet.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <returns>DataTable object</returns>
    /// <remarks>
    /// Read a specified sheet of Excel file into DataTable.
    /// The sheet name is set in the TableName property of the DataTable.
    /// </remarks>
    public static DataTable Read(string excellFilePath, string sheetName, string tableName)
    {
      var dic = new Dictionary<string, string>
      {
        { sheetName, tableName }
      };
      return Read(excellFilePath, dic)[0];
    }

    /// <summary>
    /// Reads Excel file and creates DataTable
    /// </summary>
    /// <param name="excellFilePath">The path of the target excel file.</param>
    /// <param name="sheetNames">The names of the target sheets.</param>
    /// <returns>DataTable objects</returns>
    /// <remarks>
    /// Read a specified sheet of Excel file into DataTable.
    /// The sheet name is set in the TableName property of the DataTable.
    /// </remarks>
    public static IList<DataTable> Read(string excellFilePath, IEnumerable<string> sheetNames)
    {
      return Read(excellFilePath, sheetNames.ToDictionary<string, string>(x => x));
    }

    /// <summary>
    /// Reads Excel file and creates DataTable
    /// </summary>
    /// <param name="excellFilePath">The path of the target excel file.</param>
    /// <param name="SheetAndTableNames">Dictopnay of The names of the target sheets and tables.</param>
    /// <returns>DataTable objects</returns>
    /// <remarks>
    /// Read a specified sheet of Excel file into DataTable.
    /// The sheet name is set in the TableName property of the DataTable.
    /// </remarks>
    public static IList<DataTable> Read(string excellFilePath, IDictionary<string, string> SheetAndTableNames)
    {

      using (var xl = new ExcelPackage())
      {
        var dts = new List<DataTable>();

        using (var st = File.OpenRead(excellFilePath))
        {
          xl.Load(st);
        }

        xl.Workbook.Calculate();

        foreach (var sheetName in SheetAndTableNames.Keys)
        {
          using (var ws = xl.Workbook.Worksheets[sheetName])
          {
            if (ws == null)
            {
              throw new ArgumentException($"'{sheetName}’ is not found.");
            }

            var dt = new DataTable() { TableName = SheetAndTableNames[sheetName] };

            var endColumn = ws.Dimension.End.Column;
            foreach (var cell in ws.Cells[1, 1, 1, endColumn])
            {
              if (cell.End.Column == ws.Dimension.End.Column && cell.Text == COMMENT_COLUMN_HEADER)
              {
                endColumn = cell.End.Column - 1;
                break;
              }
              dt.Columns.Add(cell.Text);
            }

            var startRow = 2;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
              var wsRow = ws.Cells[rowNum, 1, rowNum, endColumn];

              if (wsRow.Any(x => !string.IsNullOrEmpty(x.Text)))
              {
                DataRow row = dt.Rows.Add();
                foreach (var cell in wsRow)
                {
                  if (cell.Text == "NULL")
                  {
                    row[cell.Start.Column - 1] = DBNull.Value;
                  }
                  else
                  {
                    row[cell.Start.Column - 1] = cell.Text;
                  }
                }
              }
            }
            dts.Add(dt);
          }

        }
        return dts;
      }

    }
  }
}
