using System.Collections.Generic;

namespace Parakeet.Net.ROClient.Models
{
    internal sealed class FPTerminalBasicCollection : ModelBase
    {
        public override string CommandName => "terminal_basics";

        public List<FPTerminalBasic> TerminalBasics { get; set; } = new List<FPTerminalBasic>();
    }
}