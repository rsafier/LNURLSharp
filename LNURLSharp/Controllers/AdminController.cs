using LNDroneController.LND;
using LNURLSharp.APIKey;
using LNURLSharp.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNURLSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class AdminController : ControllerBase
    {
        private ILogger<AdminController> logger;
        private LNDNodeConnection node;
        private LNURLContext db;

        public AdminController(ILogger<AdminController> logger,
            IServiceProvider provider, LNURLContext context, LNDNodeConnection lnd)
        {
            this.logger = logger;
            node = lnd;
            db = context;
        }

        [HttpPost]
        [Route("/lnurl/withdraw/create")]
        public async Task<WithdrawResponse> WithdrawRequest(WithdrawRequest req)
        {
            var record = new DB.WithdrawSetup
            {
                MinWithdrawable = req.MinWithdrawable,
                MaxWithdrawable = req.MaxWithdrawable,
            };
            
            db.Add(record);
            await db.SaveChangesAsync();
            return new WithdrawResponse { Id = record.WithdrawSetupId };
        }
    }

    public class WithdrawRequest
    {
        public long MinWithdrawable { get; set; }
        public long MaxWithdrawable { get; set; }
    }
    public class WithdrawResponse
    {
        public int Id { get; set; }
    }
}
