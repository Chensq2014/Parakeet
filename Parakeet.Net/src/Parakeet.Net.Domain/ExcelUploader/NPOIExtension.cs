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

        /// <summary>
        /// 判断是否为空返回double值
        /// </summary>
        /// <param name="cell">cell</param>
        /// <returns></returns>
        public static double GetCellValueOrZero(ICell cell)
        {
            if (cell.CellType == CellType.Numeric || cell.CellType == CellType.Formula)
            {
                return cell.NumericCellValue;
            }
            return string.IsNullOrWhiteSpace(cell.StringCellValue) ? 0 : cell.NumericCellValue;
        }
    }
}
