using Common.Interfaces;

namespace ConsoleApp
{
    /// <summary>
    /// 自定义模块
    /// </summary>
    public class CustomModule: ICustomModule
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name => "";
        /// <summary>
        /// 区域
        /// </summary>
        public string Area => "";

        /// <summary>
        /// 排序
        /// </summary>
        public int Order => 0;
    }
}
