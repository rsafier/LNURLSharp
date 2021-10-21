using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LNURLSharp.DB
{
    public class LNURLContext : DbContext
    {
        public DbSet<LNDServer> LNDServers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PaySetup> PaySetups { get; set; }

        public string DbPath { get; private set; }

        public LNURLContext()
        {
            var path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{System.IO.Path.DirectorySeparatorChar}LNURLSharp.db";
            DbPath = path;
        }
        public LNURLContext(string path)
        {
            DbPath = path;
        }
        public LNURLContext(IOptions<LNURLSettings> settings)
        {
            var path =settings.Value.DbPath ?? $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{System.IO.Path.DirectorySeparatorChar}LNURLSharp.db"; 
            DbPath = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class PaySetup
    {
        public int PaySetupId { get; set; }
    }

    [Index(nameof(Pubkey), IsUnique =true)]
    public class LNDServer
    {
        [Key]
        public string Pubkey { get; set; }
        public List<Invoice> Invoices { get; } = new List<Invoice>();
    }

    [Index(nameof(Payreq), IsUnique =true)]
    [Index(nameof(Username), IsUnique =false)]
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string Comment { get; set; }
        public string Metadata { get; set; }
        public string Payreq { get; set; }
        public string RHashBase64 { get; set; }
        public string DescriptionHash { get; set; }
        public string FallbackAddr { get; set; }
        public string Username { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        //Flags
        public bool Paid { get; set; }
        public bool Expired { get; set; }

        public string LNDServerPubkey { get; set; }
        public LNDServer LNDServer { get; set; }
    }
}
