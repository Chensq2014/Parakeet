namespace ConsoleApp.Models
{
    public class StudentRecord
    {
        public string Name { get; set; }
        public string Shuxue { get; set; }
        public string YuWen { get; set; }

        //把科目和分数信息抽象到这个字典里面
        public Dictionary<string, List<decimal>> SubjectScores { get; set; }=new Dictionary<string, List<decimal>>();


        /// <summary>
        /// 添加学科成绩
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="score"></param>
        public void AddScore(string subject,decimal score)
        {
            if (SubjectScores.ContainsKey(subject))
            {
                SubjectScores[subject].Add(score);
            }
            else
            {
                SubjectScores.Add(subject,new List<decimal> { score});
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
