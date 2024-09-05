using InsertPrompt.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertPrompt.Parser
{
    internal class Mapping
    {
        public Mapping() { }
        public Element Field { get; set; }
        public List<Element> Values { get; set; }
    }
}
