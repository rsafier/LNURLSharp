using LNDroneController.LND;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LNURLSharp.Logic;
using System.Diagnostics;
using LNURLSharp.DB;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LNURLSharp.Controllers
{
    [ApiController]
    public class PayController : ControllerBase
    {
        private LNURLSettings settings;
        private ILogger<PayController> logger;
        private LNDNodeConnection node;
        private LNURLContext db;

        public PayController(Microsoft.Extensions.Options.IOptions<LNURLSettings> options, ILogger<PayController> logger, 
            IServiceProvider provider, LNURLContext context)
        {
            settings = options.Value;
            this.logger = logger;
            node = provider.GetRequiredService<LNDNodeConnection>();
            db = context;
        }

        [HttpGet]
        [Route("/pay/{Username}")]
        public async Task<string> MakeLNURLpInvoiceEndpoint()
        {           
            var username = Request.Path.Value.SplitOnLast("/").Last();
            var amount = long.Parse(Request.Query["amount"]);
            Request.Query.TryGetValue("comment", out var comment);
            if (comment.Count > 0)
            {
                if (comment[0].Length > settings.Pay.CommentMaxLength.GetValueOrDefault())
                {
                    //over limit
                    var error = new LNURLStatusResponse()
                    {
                        Status = "ERROR",
                        Reason = $"Comment length exceeds max length of {settings.Pay.CommentMaxLength.GetValueOrDefault()}.",
                    };
                    return error.ToJson();
                }
                //TODO: Should save the comment into DB
            }    
            if (amount < settings.Pay.MinSendable || amount > settings.Pay.MaxSendable)
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"Amount not in the limits of {settings.Pay.MinSendable}-{settings.Pay.MaxSendable} mSat",
                };
                return error.ToJson();
            }
            //TODO: At some point we probably need to pull this from a Datasource instead
            var metadata = new string[2, 2];
            metadata[0, 0] = @"text/plain";
            metadata[0, 1] = $"Send sats to {username}";
            metadata[1, 0] = @"text/identifier";
            metadata[1, 1] = $"{username}";
            var response = await LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayInvoiceResponseV2(node.LightningClient, amount, metadata, expiryInSeconds: settings.Pay.InvoiceExpiryInSeconds);
            var createDate = DateTime.UtcNow;
            db.Invoices.Add(new Invoice
            {
                Comment = comment,
                CreationDate  = createDate,
                ExpiryDate = createDate.AddSeconds(settings.Pay.InvoiceExpiryInSeconds),
                RHashBase64 = response.RHashBase64,
                DescriptionHash = response.DescriptionHash,
                Username =username,
                LNDServerPubkey = node.LocalNodePubKey,
                Metadata = metadata.ToJson(),
                Payreq = response.Response.pr,
            });
            db.SaveChanges();
            db.Dispose();
            return response.Response.ToJson();
        }

        [HttpGet]
        [Route("/lnurl/pay/{username}")]
        public IActionResult GenerateLNURLPayQR(string username)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            var lnurl = $"https://{settings.Domain}/.well-known/lnurlp/{username}@{settings.Domain}".ToLNURL().ToUpper();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"lightning:{lnurl}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            var ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return File(ms, "image/png");
        }

        [HttpGet]
        [Route("/.well-known/lnurlp/{username}")]
        public string WellKnownEndpoint()
        {
            var username = Request.Path.Value.SplitOnLast("/").Last();            
            var response = LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayResponse($"{username}@{settings.Domain}", $"https://{settings.Domain}/pay/{username}@{settings.Domain}", settings.Pay.MinSendable, settings.Pay.MaxSendable, settings.Pay.CommentMaxLength);
            return response.ToJson();
        }
    }
 
}
