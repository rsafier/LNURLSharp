using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LNURLSharp.Logic
{
    public class LNURLPayInvoiceResponse : LNURLStatusResponse
    {
        public LNURLPayInvoiceResponse() { }

        public LNURLPayInvoiceResponse(JsonDocument doc) : base(doc)
        {

        }

        public string pr { get; set; }
        public string[] routes { get; set; } = new string[] { };
        public object successAction { get; set; } = null;
    }
}
