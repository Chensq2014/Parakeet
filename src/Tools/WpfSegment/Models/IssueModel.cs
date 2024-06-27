using MiniExcelLibs.Attributes;

namespace WpfSegment.Models
{
    public class IssueModel
    {
        [ExcelColumnName("提出时间", null)]
        public DateTime ReportedDate { get; set; } = DateTime.Now;


        [ExcelColumnName("事业部/中心", null)]
        public string Division { get; set; }

        [ExcelColumnName("末级部门", null)]
        public string Department { get; set; }

        [ExcelColumnName("工号", null)]
        public string EmployeeID { get; set; }

        [ExcelColumnName("提出人", null)]
        public string Reporter { get; set; }

        [ExcelColumnName("紧急程度", null)]
        public string UrgencyLevel { get; set; }

        [ExcelColumnName("问题类型", null)]
        public string IssueType { get; set; }

        [ExcelColumnName("问题描述", null)]
        public string ProblemDescription { get; set; }

        [ExcelIgnore(true)]
        public List<WordCountDto> WordCounts { get; set; } = new List<WordCountDto>();
    }
}
