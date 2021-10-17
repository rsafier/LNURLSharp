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

namespace LNURLSharp.Controllers
{
    [ApiController]
    public class PayController : ControllerBase
    {
        private LNURLSettings settings;
        private ILogger<PayController> logger;
        private LNDNodeConnection node;

        public PayController(IOptions<LNURLSettings> options, ILogger<PayController> logger, IServiceProvider provider)
        {
            settings = options.Value;
            this.logger = logger;
            node = provider.GetRequiredService<LNDNodeConnection>();
        }

        [HttpGet]
        [Route("/pay/{Username}")]
        public async Task<string> MakeLNURLpInvoiceEndpoint()
        {
            var username = Request.Path.Value.SplitOnLast("/").Last();
            var amount = long.Parse(Request.Query["amount"]);

            //TODO: At some point we probably need to pull this from a Datasource instead
            var metadata = new string[2, 2];
            metadata[0, 0] = @"text/plain";
            metadata[0, 1] = $"Send sats to {username}";
            metadata[1, 0] = @"text/identifier";
            metadata[1, 1] = $"{username}";
            var response = await LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayInvoiceResponse(node.LightningClient, amount, metadata, expiryInSeconds: settings.InvoiceExpiryInSeconds);
            return response.ToJson();
        }

        [HttpGet]
        [Route("/.well-known/lnurlp/{username}")]
        public string WellKnownEndpoint()
        {
            var username = Request.Path.Value.SplitOnLast("/").Last();
            var response = LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayResponse($"{username}@{settings.Domain}", $"https://{settings.Domain}/pay/{username}@{settings.Domain}", settings.MinSendable, settings.MaxSendable, settings.CommentAllowed);
            return response.ToJson();
        }
    }
 
}
