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
using System.Security.Cryptography;
using Grpc.Core;

namespace LNURLSharp.Controllers
{
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private LNURLSettings settings;
        private ILogger<WithdrawController> logger;
        private LNDNodeConnection node;
        private LNURLContext db;
        private Random r = new Random();
        public WithdrawController(Microsoft.Extensions.Options.IOptions<LNURLSettings> options, ILogger<WithdrawController> logger,
            IServiceProvider provider, LNURLContext context, LNDNodeConnection lnd)
        {
            settings = options.Value;
            this.logger = logger;
            node = lnd;
            db = context;
        }

        [HttpGet]
        [Route("/lnurlw/{id}/payme")]
        public async Task<string> WithdrawPay(int id)
        {
            var record = db.WithdrawSetups.SingleOrDefault(x => x.WithdrawSetupId == id);
            if (record == null)
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = "id is not valid Withdraw Setup",
                };
                return error.ToJson();
            }
            if (record.Completed)
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"Withdraw Request {id} has already been completed",
                };
                return error.ToJson();
            }

            if (record.K1.IsNullOrEmpty())
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"K1 not generated",
                };
                return error.ToJson();
            }
            if (!Request.Query.TryGetValue("k1", out var k1))
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"k1 not provided",
                };
                return error.ToJson();
            }
            if (record.K1 != k1[0])
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"K1 does not match",
                };
                return error.ToJson();
            }
            if (!Request.Query.TryGetValue("pr", out var pr))
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"pr not provided",
                };
                return error.ToJson();
            }

            //We passed the first checks, not lets figure out if invoice is valid
            var decodedInvoice = await node.LightningClient.DecodePayReqAsync(new Lnrpc.PayReqString { PayReq = pr });
            if (decodedInvoice.NumMsat < record.MinWithdrawable || decodedInvoice.NumMsat > record.MaxWithdrawable)
            {
                var error = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = $"Payment request is not in range {record.MinWithdrawable}-{record.MaxWithdrawable} mSats",
                };
                return error.ToJson();
            }
            //Lets try paying this
            LNURLStatusResponse response;
            try
            {
                var payResponse = await node.PayPaymentRequest(pr, 10);
                response = new LNURLStatusResponse
                {
                    Status = payResponse.Status == Lnrpc.Payment.Types.PaymentStatus.Succeeded ? "OK" : "ERROR",
                    Reason = payResponse.Status == Lnrpc.Payment.Types.PaymentStatus.Succeeded ? String.Empty : payResponse.FailureReason.ToString()
                };
            }
            catch (RpcException e)
            {
                response = new LNURLStatusResponse()
                {
                    Status = "ERROR",
                    Reason = e.Message,
                };
            }
            record.Completed = true;
            await db.SaveChangesAsync();

            return response.ToJson();

        }

        [HttpGet]
        [Route("/lnurlw/{id}")]
        public async Task<string> WithdrawRequest(int id)
        {
            var record = db.WithdrawSetups.SingleOrDefault(x => x.WithdrawSetupId == id);
            if (record == null)
                throw new ArgumentException("id is not valid Withdraw Setup");
            if (record.Completed)
                throw new Exception($"Withdraw Request {id} has already been completed");

            var randomData = new byte[32];
            r.NextBytes(randomData);
            var sha256 = SHA256.Create();
            var k1 = sha256.ComputeHash(randomData).ToHex();

            record.K1 = k1;
            await db.SaveChangesAsync();

            var response = new LNURLWithdrawResponse
            {
                Callback = $"https://{settings.Domain}/lnurlw/{id}/payme",
                DefaultDescription = $"Withdrawl for id {id}",
                K1 = k1,
                MinWithdrawable = record.MinWithdrawable,
                MaxWithdrawable = record.MaxWithdrawable,
            };
            return response.ToJson();
        }

        [HttpGet]
        [Route("/lnurlw/qr/{id}")]
        public IActionResult WithdrawQR(int id)
        {
            var record = db.WithdrawSetups.SingleOrDefault(x => x.WithdrawSetupId == id);
            if (record == null)
                throw new ArgumentException("id is not valid Withdraw Setup");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            var lnurl = $"https://{settings.Domain}/lnurlw/{id}".ToLNURL().ToUpper();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"lightning:{lnurl}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            var ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return File(ms, "image/png");
        }
    }
}
