using Google.Protobuf;
using LNDroneController.LND;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LNURLSharp.Logic
{
    /// <summary>
    /// All the logic to handle LUD 06, 18, 20 (stub for 09/10)
    /// </summary>
    public static class LNURLPayLogic
    {

        /// <summary>
        /// Generates a LNURLp Response object
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callbackUri"></param>
        /// <param name="minSendable"></param>
        /// <param name="maxSendable"></param>
        /// <param name="commentAllowedLenght"></param>
        /// <param name="image"></param>
        /// <param name="imageMimeType"></param>
        /// <param name="longText"></param>
        /// <param name="payerData"></param>
        /// <returns></returns>
        public static LNURLPayResponse BuildLNURLPayResponse(string username, string callbackUri, ulong minSendable = 1,
            ulong maxSendable = 100000000000, int? commentAllowedLenght = 160, byte[] image = null, string imageMimeType = null, 
            string longText = null, PayerDataType payerData = null)
        {
            var lightningAddressResponse = new LNURLPayResponse()
            {
                MaxSendable = 100000000000,
                MinSendable = 1000,
                CommentAllowed = commentAllowedLenght,
                PayerData = payerData,
            };
            var elements = 2;
            var index = 2;
            if (image != null) elements++;
            if (!longText.IsNullOrEmpty()) elements++;

            var metadata = new string[elements, 2];
            metadata[0, 0] = @"text/plain";
            metadata[0, 1] = $"Send sats to {username}";
            metadata[1, 0] = @"text/identifier";
            metadata[1, 1] = $"{username}";

            if (!longText.IsNullOrEmpty())
            {
                metadata[index, 0] = @"text/long-desc";
                metadata[index, 1] = longText;
                index++;
            }
            if (image != null)
            {
                metadata[index, 0] = imageMimeType + ";base64";
                metadata[index, 1] = Convert.ToBase64String(image);
                index++;
            }

            lightningAddressResponse.Metadata = metadata.ToJson();
            lightningAddressResponse.Callback = callbackUri;
            return lightningAddressResponse;
        }

        /// <summary>
        /// Generates an LNURLp response
        /// </summary>
        /// <param name="client"></param>
        /// <param name="amount"></param>
        /// <param name="originalMetadata"></param>
        /// <param name="payerJsonData"></param>
        /// <param name="expiryInSeconds"></param>
        /// <returns></returns>
        public static async Task<LNURLPayInvoiceResponse> BuildLNURLPayInvoiceResponse(this Lnrpc.Lightning.LightningClient client, long amount, string[,] originalMetadata, 
            string payerJsonData = null, int expiryInSeconds = 600)
        {
            var descriptionToBeHashed = originalMetadata.ToJson();
            if (!payerJsonData.IsNullOrEmpty())
            {
                descriptionToBeHashed += payerJsonData;
            }
            using var sha256 = SHA256.Create();
            var descriptionHash = sha256.ComputeHash(descriptionToBeHashed.ToUtf8Bytes());
            var invoice = await client.AddInvoiceAsync(new Lnrpc.Invoice
            {
                ValueMsat = amount,
                DescriptionHash = ByteString.CopyFrom(descriptionHash),
                Expiry = expiryInSeconds,
            });
            var response = new LNURLPayInvoiceResponse
            {
                pr = invoice.PaymentRequest,
            };
            return response;
        }

        public static async Task<bool> VerifyLNURLPayInvoice(this Lnrpc.Lightning.LightningClient client, LNURLPayResponse req, string payRequest, long amount) 
        {
            var decoded = await client.DecodePayReqAsync(new Lnrpc.PayReqString
            {
                PayReq = payRequest
            });
            var dh = decoded.DescriptionHash;
            using var sha256 = SHA256.Create();
            var descriptionHash = sha256.ComputeHash(req.Metadata.ToUtf8Bytes());
            return descriptionHash.ToHex() == decoded.DescriptionHash && decoded.NumMsat == amount;
        }

    }
}
