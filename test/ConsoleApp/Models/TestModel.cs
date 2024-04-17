using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Models
{
    public interface IFoo { }
    public interface IBar { }
    public interface IBaz { }
     
    public class Foo : IFoo { }
    public class Bar : IBar { }
    public class Baz : IBaz { }
}
