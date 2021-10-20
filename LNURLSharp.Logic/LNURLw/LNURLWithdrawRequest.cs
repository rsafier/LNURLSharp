using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks; 

namespace LNURLSharp.Logic
{
    /// <summary>
    ///     https://github.com/fiatjaf/lnurl-rfc/blob/luds/02.md
    /// </summary>
    public class LNURLWithdrawRequest
    {
        public LNURLWithdrawRequest() { }

        public string Callback { get; set; }

        public string K1 { get; set; }

        public string Tag { get; set; }

        public string DefaultDescription { get; set; }

        public long MinWithdrawable { get; set; }

        public long MaxWithdrawable { get; set; }

        public long CurrentBalance { get; set; }

        public string BalanceCheck { get; set; }

        public string PayLink { get; set; }
    }
}