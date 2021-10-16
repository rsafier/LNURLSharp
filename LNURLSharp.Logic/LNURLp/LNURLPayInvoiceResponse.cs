using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LNURLSharp.Logic
{
    public class LNURLPayInvoiceResponse
    {
        public string pr { get; set; }
        public string[] routes { get; set; } = new string[] { };
        public object successAction { get; set; } = null;
    }
}
