using NPOI.SS.UserModel;

namespace Parakeet.Net.ExcelUploader
{
    public static class NPOIExtension
    {
        public static string StringValue(this IRow row, int index)
        {
            var cell = row.GetCell(index);
            return row.GetCell(index).StringCellValue;
        }

        public static double DoubleValue(this IRow row, int index)
        {
            var cell = row.GetCell(index);
            return cell.NumericCellValue;
        }
    }
}
