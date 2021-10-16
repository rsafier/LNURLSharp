namespace LNURLSharp.Logic
{
    public class PayerDataType
    {
        public IsMandatoryType Name { get; set; }
        public IsMandatoryType Pubkey { get; set; }
        public IsMandatoryType Identifier { get; set; }
        public IsMandatoryType Email { get; set; }
        public AuthRequestType Auth { get; set; }

    }
}
