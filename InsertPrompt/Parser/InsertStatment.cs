using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertPrompt.Parser
{
    internal class InsertStatment
    {
        public string TableName { get;set; }
        public List<Mapping> ColumnMappings{ get; set; }
        public InsertStatment()
        {
            TableName = "";
            ColumnMappings = new List<Mapping>();
        }
    }
}
