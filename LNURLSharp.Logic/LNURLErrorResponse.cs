using System.Text.Json;

namespace LNURLSharp.Logic
{
    public class LNURLErrorResponse
    {
        private JsonDocument doc;
        public LNURLErrorResponse(JsonDocument d)
        {
            doc = d;
        }

        public string Status
        {
            get
            {
                return doc.RootElement.GetProperty("status").GetString();
            }
        }
    }
}
