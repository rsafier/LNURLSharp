using System.Collections.Generic;
using System.Text.Json;

namespace LNURLSharp.Logic
{
    public class LNURLStatusResponse
    {
        public LNURLStatusResponse() { }
        public LNURLStatusResponse(JsonDocument doc)
        {
            try
            {
                Status = doc.RootElement.GetProperty("status").GetString();
            }
            catch (KeyNotFoundException e)
            {
            }
            try
            {
                Reason = doc.RootElement.GetProperty("reason").GetString();
            }
            catch (KeyNotFoundException e)
            {
            }
        }

        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
