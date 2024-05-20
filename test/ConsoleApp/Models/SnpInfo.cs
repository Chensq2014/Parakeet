using MiniExcelLibs.Attributes;

namespace ConsoleApp.Models
{
    public class SnpInfo
    {
        /// <summary>
        /// ChromosomeStart
        /// </summary>
        [ExcelColumnName("chromosome:start")]
        public string ChromosomeStart { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [ExcelColumnName("name")]
        public string Name { get; set; }
    }
}
