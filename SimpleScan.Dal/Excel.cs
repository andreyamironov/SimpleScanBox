using OfficeOpenXml;
using OfficeOpenXml.Table;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleScan.Dal
{
    public static class Excel
    {
        public static byte[] ToExcel_Quick<T>(IEnumerable<T> source)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                package.Workbook.Properties.Author = "SimpleScan";
                package.Workbook.Properties.Title = "Scan";
                package.Workbook.Properties.Company = "M&M";

                bool printHeaders = true;
                var tableStyle = OfficeOpenXml.Table.TableStyles.Light9;

                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet1");
                ws.Cells[1, 1].LoadFromCollection<T>(source, printHeaders, tableStyle);
                ws.Cells.AutoFitColumns(5, 20);

                return package.GetAsByteArray();
            }
        }
    }
}
