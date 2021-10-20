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

namespace LNURLSharp.Controllers
{
    [ApiController]
    public class PayController : ControllerBase
    {
        private LNURLSettings settings;
        private ILogger<PayController> logger;
        private LNDNodeConnection node;
        private LNURLContext db;

        public PayController(IOptions<LNURLSettings> options, ILogger<PayController> logger, 
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
            var response = await LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayInvoiceResponse(node.LightningClient, amount, metadata, expiryInSeconds: settings.Pay.InvoiceExpiryInSeconds);

            db.Invoices.Add(new Invoice
            {
                Comment = comment,
                CreateDate = DateTime.UtcNow,
                Username =username,
                LNDServerPubkey = node.LocalNodePubKey,
                Metadata = metadata.ToJson(),
                Payreq = response.pr,
            });
            db.SaveChanges();
            db.Dispose();
            return response.ToJson();
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
