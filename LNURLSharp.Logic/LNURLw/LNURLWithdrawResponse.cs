namespace LNURLSharp.Logic
{
    public class LNURLWithdrawResponse
    {
        public string Callback { get; set; }
        public long MaxWithdrawable { get; set; }
        public long MinWithdrawable { get; set; }
        public string Tag { get; } = "withdrawRequest";
        public string DefaultDescription { get; set; }
        public string K1 { get; set; }
    }

}