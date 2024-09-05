using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertPrompt.Parser
{
    internal class Element
    {
        public int Start { get; set; }
        public int Lenght { get; set; }
        public string Text { get; set; }
        public Element() { }
    }
}
