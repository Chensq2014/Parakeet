using System;

namespace ConsoleApp.Cache
{
    public class DataModel
    {
        public object Value { get; set; }
        public ObsoluteType ObsoluteType { get; set; }
        public DateTime DeadLine { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
