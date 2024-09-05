using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameInvoice
{
    public class InvoiceInfo
    {
        private byte _set = 0;
        private string company;
        private string date;
        private string money;
        private string code;

        public string Company
        {
            get => company; set
            {
                _set |= 0x1;
                company = value;
            }
        }
        public string Date
        {
            get => date; set
            {
                _set |= 0x2;
                date = value;
            }
        }
        public string Money
        {
            get => money; set
            {
                _set |= 0x3;
                money = value;
            }
        }
        public string Code
        {
            get => code; set
            {
                _set |= 0x4;
                code = value;
            }
        }
        public bool IsCompleted
        {
            get
            {
                return (_set & 0x4) == 0x4;
            }
        }  
    }
}
