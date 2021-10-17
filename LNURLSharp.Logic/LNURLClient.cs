using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace LNURLSharp.Logic
{
    public static partial class LNURLClient
    {

        public static async Task<object> FetchInformation(Uri lnUrl, HttpClient httpClient)
        {
            return await FetchInformation(lnUrl, null, httpClient);
        }

        public static async Task<object> FetchInformation(Uri lnUrl, string tag, HttpClient httpClient)
        {
            string k1;
            switch (tag)
            {
                case null:
                    var response = JsonDocument.Parse(await httpClient.GetStringAsync(lnUrl));
                    if (response.RootElement.TryGetProperty("tag", out var tagToken))
                    {
                        tag = tagToken.ToString();
                        return FetchInformation(response, tag);
                    }

                    throw new Exception("A tag identifying the LNURL endpoint was not received.");
                //case "withdrawRequest":
                //    //fast withdraw request supported:
                //    queryString = lnUrl.ParseQueryString();
                //    k1 = queryString.Get("k1");
                //    var minWithdrawable = queryString.Get("minWithdrawable");
                //    var maxWithdrawable = queryString.Get("maxWithdrawable");
                //    var defaultDescription = queryString.Get("defaultDescription");
                //    var callback = queryString.Get("callback");
                //    if (k1 is null || minWithdrawable is null || maxWithdrawable is null || callback is null)
                //    {
                //        response = JObject.Parse(await httpClient.GetStringAsync(lnUrl));
                //        return FetchInformation(response, tag);
                //    }

                //    return new LNURLWithdrawRequest
                //    {
                //        Callback = new Uri(callback),
                //        K1 = k1,
                //        Tag = tag,
                //        DefaultDescription = defaultDescription,
                //        MaxWithdrawable = maxWithdrawable,
                //        MinWithdrawable = minWithdrawable
                //    };
                //case "login":

                //    queryString = lnUrl.ParseQueryString();
                //    k1 = queryString.Get("k1");
                //    var action = queryString.Get("action");

                //    return new LNAuthRequest()
                //    {
                //        K1 = k1,
                //        LNUrl = lnUrl,
                //        Action = string.IsNullOrEmpty(action)
                //            ? (LNAuthRequest.LNAUthRequestAction?)null
                //            : Enum.Parse<LNAuthRequest.LNAUthRequestAction>(action, true)
                //    };

                default:
                    response = JsonDocument.Parse(await httpClient.GetStringAsync(lnUrl));
                    return FetchInformation(response, tag);
            }
        }

        private static object FetchInformation(JsonDocument response, string tag)
        {
            if (IsErrorResponse(response)) return response;

            switch (tag)
            {
                //case "channelRequest":
                //    return response.ToObject<LNURLChannelRequest>();
                //case "withdrawRequest":
                //    return response.ToObject<LNURLWithdrawRequest>();
                case "payRequest":
                    return response.ToLNURLPayResponse();
                default:
                    return response;
            }
        }

        private static bool IsErrorResponse(JsonDocument response)
        {
            if (response.RootElement.TryGetProperty("status", out var value))
            {
                if (value.GetString().Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
