using MiniExcelLibs.Attributes;

namespace WpfSegment.Models
{
    public class IssueTypeGroupCountDto
    {
        [ExcelColumnName("问题类型", null)]
        public string TypeName { get; set; }

        [ExcelColumnName("类型分组数", null)]
        public long? Count { get; set; }

        [ExcelIgnore(true)]
        public decimal? Amount { get; set; }
    }

}
