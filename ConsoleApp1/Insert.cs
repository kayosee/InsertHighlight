using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Insert
    {
        public string TableName { get;set; }
        public List<Mapping> Columns{ get; set; }
        public Insert()
        {
            TableName = "";
            Columns = new List<Mapping>();
        }
    }
}
