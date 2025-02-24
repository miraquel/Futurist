using ClosedXML.Excel;

namespace Futurist.Common.Helpers
{
    public static class ExcelHelper
    {
        public static IEnumerable<T> ParseExcel<T>(Stream stream, Func<IXLRow, T> mapRow, bool skipHeader = true)
        {
            var results = new List<T>();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var isFirstRow = true;
            foreach (var row in worksheet.RowsUsed())
            {
                if (skipHeader && isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                results.Add(mapRow(row));
            }

            return results;
        }
    }
}
