namespace ConsoleApp.Models
{
    /// <summary>
    /// 实体类
    /// </summary>
    public class People
    {
        /// <summary>
        /// Age
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int Id;
    }

    /// <summary>
    /// 实体类Target
    /// </summary>
    public class PeopleCopy
    {
        /// <summary>
        /// Age
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }//ShortName
        /// <summary>
        /// Id
        /// </summary>
        public int Id;
    }
}
