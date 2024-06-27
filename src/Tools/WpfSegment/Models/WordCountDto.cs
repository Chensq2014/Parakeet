using MiniExcelLibs.Attributes;

namespace WpfSegment.Models
{
    public class WordCountDto
    {
        [ExcelColumnName("统计维度", null)]
        public string GroupByType { get; set; }

        [ExcelColumnName("关键字", null)]
        public string Word { get; set; }

        [ExcelColumnName("出现次数", null)]
        public long? Count { get; set; }
    }
}
