using LNDroneController.LND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNURLSharp
{
    public class LNURLSettings
    {
        public string Domain { get; set; }
        public bool EnableTorEndpoint { get; set; }
        public PaySettings Pay { get; set; }

        public List<LNDSettings> LNDNodes { get; set; }
    }

    public class PaySettings
    {
        public int InvoiceExpiryInSeconds { get; set; }
        public long MaxSendable { get; set; }
        public long MinSendable { get; set; }
        public int? CommentMaxLength { get; set; }
    }
}
