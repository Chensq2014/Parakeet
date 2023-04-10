using System;

namespace Parakeet.Net.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExcelCellOptionAttribute : Attribute
    {
        public ExcelCellOptionAttribute(int index)
        {
            ColumnIndex = index;
        }
        public int ColumnIndex { get; set; }
    }
}
