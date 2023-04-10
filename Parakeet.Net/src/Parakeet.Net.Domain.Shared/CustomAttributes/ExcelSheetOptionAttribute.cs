using System;

namespace Parakeet.Net.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelSheetOptionAttribute : Attribute
    {
        public ExcelSheetOptionAttribute(int x, int y)
        {
            StartX = x;
            StartY = y;
        }
        public int StartX { get; set; }
        public int StartY { get; set; }
    }
}
