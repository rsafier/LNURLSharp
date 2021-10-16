using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNURLSharp
{
    public class LNURLSettings
    {
        public string Domain { get; set; }
        public int InvoiceExpiryInSeconds { get; set; }
        public ulong MaxSendable { get; set; }
        public ulong MinSendable { get; set; }
        public int? CommentAllowed { get; set; }
    }
}
