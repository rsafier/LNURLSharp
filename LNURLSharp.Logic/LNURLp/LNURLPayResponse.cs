using System.Runtime.Serialization;

namespace LNURLSharp.Logic
{
    public class LNURLPayResponse
    {
        public string Callback { get; set; }  
        public long MaxSendable { get; set; } 
        public long MinSendable { get; set; }
        public string Metadata { get; set; }
        public string Tag { get; } = "payRequest";
        public int? CommentAllowed { get; set; }
        public PayerDataType PayerData { get; set; }
    }
}
