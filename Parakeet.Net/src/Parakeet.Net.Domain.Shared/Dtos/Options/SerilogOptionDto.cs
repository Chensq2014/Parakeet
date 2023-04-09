namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// SerilogOption 配置类
    /// </summary>
    public class SerilogOptionDto : BaseAppOptionDto
    {
        public const string ConfigKey = "Serilog";

        public const string ConfigExceptionlessKey = "Serilog:Exceptionless";

        /// <summary>
        /// Exceptionless 配置类
        /// </summary>
        public ExceptionlessOptionDto Exceptionless { get; set; }
    }
}
