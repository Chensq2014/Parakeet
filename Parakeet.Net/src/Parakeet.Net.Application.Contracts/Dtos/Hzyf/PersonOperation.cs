namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 宇泛人脸识别设备操作类
    /// </summary>
    public class PersonOperation
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo;

        /// <summary>
        /// 接口名称 新增:"person/create"   /删除:"person/delete"
        /// </summary>
        public string InterfaceName;

        /// <summary>
        /// 操作
        /// </summary>
        public bool Result;

        /// <summary>
        /// 操作对象
        /// </summary>
        public string Person;//添加/删除人员 需要的参数(json字符串)

        /// <summary>
        /// 操作Id字符串
        /// </summary>
        public string Id;//删除人员 需要的参数(json字符串)

    }
}
