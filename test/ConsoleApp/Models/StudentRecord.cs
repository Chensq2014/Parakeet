using NUglify;

namespace ConsoleApp.Models
{
    public class StudentRecord
    {
        public StudentRecord()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Shuxue { get; set; }
        public string YuWen { get; set; }

        //把科目和分数信息抽象到这个字典里面
        public Dictionary<string, List<string>> SubjectScores { get; set; } = new Dictionary<string, List<string>>();


        public List<SubjectRecord> Subjects { get; set; } = new List<SubjectRecord>();

        public void AddSubject(SubjectRecord subject)
        {
            var existSubject = Subjects.FirstOrDefault(x => x.Name == subject.Name);
            if (existSubject != null)
            {
                existSubject.Score = $"{existSubject.Score};{subject.Score}";
            }
            else
            {
                subject.StudentRecordId = Id;
                Subjects.Add(subject);
            }
        }

        public void AddSubjects(List<SubjectRecord> subjects)
        {
            foreach (var item in subjects)
            {
                AddSubject(item);
            }
        }


        public string GetSubjectsDisplay()
        {
            return $"Name:{Name}:Scores:{string.Join(" ", Subjects.Select(x => x.GetSubjectScoreDisplay()))}";
        }

        /// <summary>
        /// 添加学科成绩
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="score"></param>
        public void AddScore(string subject, string score)
        {
            if (SubjectScores.ContainsKey(subject))
            {
                SubjectScores[subject].Add(score);
            }
            else
            {
                SubjectScores.Add(subject, new List<string> { score });
            }
        }


        public string GetMergedScoresForSubject(string subject)
        {
            if (SubjectScores.ContainsKey(subject))
            {
                return $"{string.Join(";", SubjectScores[subject])}";
            }
            return null;
        }
        public override string ToString()
        {
            return $"Name: {Name}, Scores: {string.Join(", ", SubjectScores.Select(entry => $"{entry.Key}:{GetMergedScoresForSubject(entry.Key)}"))}";
        }
    }
}
