using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;
using System.Text.Json;
using System.Net;

namespace LNURLSharp.Logic
{
    public static class LNURLExtentions
    {
        private static readonly Dictionary<string, string> SchemeTagMapping =
           new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
           {
                { "lnurlc", "channelRequest" },
                { "lnurlw", "withdrawRequest" },
                { "lnurlp", "payRequest" },
                { "keyauth", "login" }
           };
        public static (Uri uri, string tag) ParseLNURL(this string lnurl)
        {
            string tag = string.Empty;
            lnurl = lnurl.Replace("lightning:", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            if (lnurl.StartsWith("lnurl1", StringComparison.InvariantCultureIgnoreCase))
            {
                Bech32Engine.Decode(lnurl, out _, out var data);
                var result = new Uri(Encoding.UTF8.GetString(data));

                var query = result.ParseQueryString();
                tag = query.Get("tag");
                return (result, tag);
            }

            if (Uri.TryCreate(lnurl, UriKind.Absolute, out var lud17Uri) &&
                SchemeTagMapping.TryGetValue(lud17Uri.Scheme.ToLowerInvariant(), out tag))
                return (new Uri(lud17Uri.ToString()
                    .Replace(lud17Uri.Scheme + ":", lud17Uri.IsOnion() ? "http:" : "https:")), tag);

            throw new FormatException("LNURL uses bech32 and 'lnurl' as the hrp (LUD1) or an lnurl LUD17 scheme. ");
        }

        public static string ToLNURL(this string urlToEncode)
        {
            return Bech32Engine.Encode("lnurl", Encoding.UTF8.GetBytes(urlToEncode));
        }

        internal static void AppendPayloadToQuery(this UriBuilder uri, string key, string value)
        {
            if (uri.Query.Length > 1)
                uri.Query += "&";

            uri.Query = uri.Query + WebUtility.UrlEncode(key) + "=" +
                        WebUtility.UrlEncode(value);
        }

        internal static NameValueCollection ParseQueryString(this Uri uri)
        {
            NameValueCollection queryParameters = new NameValueCollection();
            string[] querySegments = uri.Query.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }
            return queryParameters;
        }

        public static string EncodeLNURL(this Uri serviceUrl)
        {
            return Bech32Engine.Encode("lnurl", Encoding.UTF8.GetBytes(serviceUrl.ToString()));
        }

        /// <summary>
        /// Is this an Onion Address?
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsOnion(this Uri uri)
        {
            if (uri == null || !uri.IsAbsoluteUri)
                return false;
            return uri.DnsSafeHost.EndsWith(".onion", StringComparison.OrdinalIgnoreCase);
        }

        public static LNURLPayResponse ToLNURLPayResponse(this JsonDocument d)
        {
            var payRequest = new LNURLPayResponse
            {
                Callback = d.RootElement.GetProperty("callback").GetString(),
                MinSendable = d.RootElement.GetProperty("minSendable").GetInt64(),
                MaxSendable = d.RootElement.GetProperty("maxSendable").GetInt64(),
                Metadata = d.RootElement.GetProperty("metadata").GetString(),
            };
            if (d.RootElement.TryGetProperty("CommentAllowed", out var comment))
            {
                payRequest.CommentAllowed = comment.GetInt32();
            }
            return payRequest;
        }

        public static LNURLWithdrawRequest ToLNURLWithdrawRequest(this JsonDocument d)
        {
            var withdrawRequest = new LNURLWithdrawRequest
            {
                Callback = d.RootElement.GetProperty("callback").GetString(),
                Tag = d.RootElement.GetProperty("tag").GetString(),
                MaxWithdrawable = d.RootElement.GetProperty("maxWithdrawable").GetInt64(),
                MinWithdrawable = d.RootElement.GetProperty("minWithdrawable").GetInt64(),
                K1 = d.RootElement.GetProperty("k1").GetString(),
            };
            withdrawRequest.CurrentBalance = withdrawRequest.MaxWithdrawable;
            if (d.RootElement.TryGetProperty("payLink", out var payLink))
            {
                withdrawRequest.PayLink = payLink.GetString();
            }
            if (d.RootElement.TryGetProperty("defaultDescription", out var defaultDescription))
            {
                withdrawRequest.DefaultDescription = defaultDescription.GetString();
            }
            if (d.RootElement.TryGetProperty("balanceCheck", out var balanceCheck))
            {
                withdrawRequest.BalanceCheck = balanceCheck.GetString();
            }
            return withdrawRequest;
        }

        public static LNURLPayInvoiceResponse ToLNURLPayInvoiceResponse(this JsonDocument d)
        {
            var payInvoice = new LNURLPayInvoiceResponse
            {
                pr = d.RootElement.GetProperty("pr").GetString(),
            };
            return payInvoice;
        }


    }
}
