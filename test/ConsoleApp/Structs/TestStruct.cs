using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Structs
{
    /// <summary>
    /// 测试一下结构体 可空值类型
    /// </summary>
    public struct TestStruct
    {
        public int? Count;
        public int? Hours { get; set; }
        public string Name { get; set; }
    }
}
