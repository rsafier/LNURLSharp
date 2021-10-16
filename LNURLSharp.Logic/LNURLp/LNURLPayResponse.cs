using System.Runtime.Serialization;

namespace LNURLSharp.Logic
{
    public class LNURLPayResponse
    {
        public string Callback { get; set; }  
        public ulong MaxSendable { get; set; } 
        public ulong MinSendable { get; set; }
        public string Metadata { get; set; }
        public string Tag { get; } = "payRequest";
        public int? CommentAllowed { get; set; }
        public PayerDataType PayerData { get; set; }
    }
}
