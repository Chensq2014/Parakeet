using NPOI.OpenXmlFormats.Dml;

namespace ConsoleApp.Models
{
    public class SubjectRecord
    {
        public SubjectRecord()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Score { get; set; }

        public Guid StudentRecordId { get; set; }

        public string GetSubjectScoreDisplay()
        {
            return $"{Name} {Score}";
        }
    }
}
