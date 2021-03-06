using NUnit.Framework;
using System;
using System.Threading.Tasks;
using LNURLSharp.Logic;
using System.Diagnostics;
using Lnrpc;
using LNDroneController.LND;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Text;
using ServiceStack;

namespace LNURLSharp.Tests
{
    public class Tests
    {
        private Lightning.LightningClient lighningClient;

        public IConfigurationRoot configuration { get; private set; }
        private LNDNodeConnection lndNode;

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Tests>();
            configuration = builder.Build();
            LNDSettings settings = new LNDSettings();

            configuration.GetSection("LNDSettings").Bind(settings);

            lndNode = new LNDNodeConnection(settings);
            lighningClient = lndNode.LightningClient;
        }

        [Test]
        public async Task LNURLEncodeDecode()
        {
            var url = "lightning:https://safier.com/.well-known/lnurlp/richardj";
            var lnurl = url.ToLNURL();
            var decoded = lnurl.ParseLNURL();
            Assert.That(decoded.uri == new Uri(url));
        }
        [Test]
        public async Task LNURLDecode()
        {
            var result = "lnurl1dp68gup69uhhx6t8dejhgtncv4hx7m3wve6kuw3cxqhkcmn4wfkr7ufax9jrgdmrv56nzcmyv9snwwtzxfjr2vp4xgurzdtxxqek2dn9vs6rjwfhxucnvvr9vfjkxcfevsekxe34vserscf4vgurwvrrxucnvegtpvm27".ParseLNURL();
            Assert.That(result.uri == new Uri("http://signet.xenon.fun:80/lnurl?q=1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e"));
        }

        [Test]
        public async Task LNURLDecode2()
        {
            var result = "lnurl1dp68gurn8ghj7umpve5k2u3wvdhk6tewwajkcmpdddhx7amw9akxuatjd3cz7unfvd5xzunydg4cghmg".ParseLNURL();
            result.PrintDump();
        }
        [Test]
        public async Task LNURLPayRequest()
        {
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(new Uri("https://safier.com/.well-known/lnurlp/richardj"), new System.Net.Http.HttpClient()) as LNURLPayResponse;
            Assert.That(result != null);
            Assert.That(result.MinSendable == 1000);
            Assert.That(result.MaxSendable == 100000000000);
            Assert.That(result.Callback.UrlDecode() == "https://safier.com/pay/richardj@safier.com");
            Assert.That(result.CommentAllowed == null);
            Assert.That(result.Metadata == "[[\"text/plain\",\"Send sats to richardj@safier.com\"],[\"text/identifier\",\"richardj@safier.com\"]]");
            Assert.That(result.Tag == "payRequest");
        }

        [Test]
        public async Task LNURLWithdrawRequest()
        {
            var parsed = "lightning:LNURL1DP68GURN8GHJ7MRWW4EXCTNXD9SHG6NPVCHXXMMD9AKXUATJDSKHW6T5DPJ8YCTH8AEK2UMND9HKU0FEV3JX2V35VCUNXE348YMRVVEJ8QMN2DPCV5UNYER9XUMRXCECVSMNJDTRV3SN2WTYV3JR2CM9V4JNJE3SXP3RXEFSXVCRVEPJV9JR2RZJM8W".ParseLNURL();
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(parsed.uri, new System.Net.Http.HttpClient()) as LNURLWithdrawRequest;
            Assert.That(result != null);
            Assert.That(result.Callback.UrlDecode().StartsWith("https://lnurl.fiatjaf.com/lnurl-withdraw/callback/"));
            Assert.That(result.DefaultDescription == "sample withdraw");
            Assert.That(result.Tag == "withdrawRequest");
        }

        [Test]
        public async Task LNURLWithdrawRequestAndSubmission()
        {
            var parsed = "lightning:LNURL1DP68GURN8GHJ7MRWW4EXCTNXD9SHG6NPVCHXXMMD9AKXUATJDSKHW6T5DPJ8YCTH8AEK2UMND9HKU0FEV3JX2V35VCUNXE348YMRVVEJ8QMN2DPCV5UNYER9XUMRXCECVSMNJDTRV3SN2WTYV3JR2CM9V4JNJE3SXP3RXEFSXVCRVEPJV9JR2RZJM8W".ParseLNURL();
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(parsed.uri, new System.Net.Http.HttpClient()) as LNURLWithdrawRequest;
            Assert.That(result != null);
            Assert.That(result.Callback.UrlDecode().StartsWith("https://lnurl.fiatjaf.com/lnurl-withdraw/callback/"));
            Assert.That(result.DefaultDescription == "sample withdraw");
            Assert.That(result.Tag == "withdrawRequest");
            //now lets get some sats
            var inv = lighningClient.AddInvoice(new Invoice { ValueMsat = result.MinWithdrawable, Expiry=600 });
            var response = await result.SendRequest(inv.PaymentRequest, new System.Net.Http.HttpClient());
            response.PrintDump();
        }

        [Test]
        public async Task LNURLPayrequestGetInvoice()
        {
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(new Uri("https://safier.com/.well-known/lnurlp/richardj"), new System.Net.Http.HttpClient()) as LNURLPayResponse;
            Assert.That(result != null);
            Assert.That(result.MinSendable == 1000);
            Assert.That(result.MaxSendable == 100000000000);
            Assert.That(result.Callback.UrlDecode() == "https://safier.com/pay/richardj@safier.com");
            Assert.That(result.CommentAllowed == null);
            Assert.That(result.Metadata == "[[\"text/plain\",\"Send sats to richardj@safier.com\"],[\"text/identifier\",\"richardj@safier.com\"]]");
            Assert.That(result.Tag == "payRequest"); 
            var payableInvoice = await result.SendRequest(lighningClient, 1001, new System.Net.Http.HttpClient(),null);
            Assert.That(payableInvoice.pr != null);
        }
        [Test]
        public   async Task LNURLPayrequestShouldError()
        {
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(new Uri("https://zbd.gg/.well-known/lnurlp/xenonfun"), new System.Net.Http.HttpClient()) as LNURLPayResponse;
            var payableInvoice = await result.SendRequest(lighningClient, 1, new System.Net.Http.HttpClient());
            Assert.That(payableInvoice.Status.Equals("error", StringComparison.InvariantCultureIgnoreCase));
        }

        //[Test]
        //public async Task LNURLWithdrawlDecode()
        //{
        //    var result = "lnurl1dp68gup69uhhx6t8dejhgtncv4hx7m3wve6kuw3cxqhkcmn4wfkr7ufax9jrgdmrv56nzcmyv9snwwtzxfjr2vp4xgurzdtxxqek2dn9vs6rjwfhxucnvvr9vfjkxcfevsekxe34vserscf4vgurwvrrxucnvegtpvm27".ParseLNURL();
        //    Assert.That(result.uri == new Uri("http://signet.xenon.fun:80/lnurl?q=1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e"));
        //    Assert.That(result.tag == null);
        //    var info = await LNURLSharp.Logic.LNURLClient.FetchInformation(new Uri("http://signet.xenon.fun/lnurl?q=1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e"), new System.Net.Http.HttpClient());
        //    Debug.Print(info.ToString());
        //    //Assert.That(result.K1 == "1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e");
        //    //Assert.That(result.Tag == "withdrawRequest");
        //    //Assert.That(result.MinWithdrawable.MilliSatoshi == 1);
        //    //Assert.That(result.MaxWithdrawable.MilliSatoshi == 100000000);
        //    //Assert.That(result.Callback == new Uri("http://signet.xenon.fun/lnurl"));
        //}

        //[Test]
        //public async Task LNURLWithdrawlInvoiceFAIL()
        //{
        //    var uri = LNURL.LNURL.Parse("lnurl1dp68gup69uhhx6t8dejhgtncv4hx7m3wve6kuw3cxqhkcmn4wfkr7ufax9jrgdmrv56nzcmyv9snwwtzxfjr2vp4xgurzdtxxqek2dn9vs6rjwfhxucnvvr9vfjkxcfevsekxe34vserscf4vgurwvrrxucnvegtpvm27", out var tag, true);
        //    Assert.That(uri == new Uri("http://signet.xenon.fun:80/lnurl?q=1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e"));
        //    var result = await LNURL.LNURL.FetchInformation(uri, new System.Net.Http.HttpClient()) as LNURLWithdrawRequest;
        //    var invoice = "lntbs1210n1psk0z9gpp5jt9k38vnr5jd75yc44tzywa3s2vs25vledhcdvgd97rrmvs2a27qdqgdpjkcmr0cqzpgxqyz5vqsp547n0qntqzw9xwmn386qxzrtv285tk6dgn4cdpzqy29nsrjw725js9qyyssq6yt2zyglmcza8kjyh2nr7tyl2nfu5uw2whjv937pp6vqplsvu9v82hsp7yujmlwv5024fgwgr7532jh4p5k8zl7d8mgkp76q3aget6qq9y4zhe";
        //    var payRequest = await result.SendRequest(invoice, new System.Net.Http.HttpClient());
        //    Assert.That(payRequest.Status == "Failed");
        //}

        //[Test]
        //public async Task LNURLPayDecode()
        //{
        //    var uri = LNURL.LNURL.Parse("lnurl1dp68gup69uhhx6t8dejhgtncv4hx7m3wve6kuw3cxqhkcmn4wfkr7ufaxpjrxwfkxfnxvvm9xyckgcnyxp3rjephxqcxydmpvvuryefsxgcrjvfexs6nzd3exvmnqc3nv5unvefn8y6njcekxf3njv3exc6rjcgn6agpp", out var tag, true);
        //    Assert.That(uri == new Uri("http://signet.xenon.fun:80/lnurl?q=0d3962ff3e11dbd0b9d700b7ac82e02091945169370b3e96e3959c62c929649a"));
        //    var result = await LNURL.LNURL.FetchInformation(uri, new System.Net.Http.HttpClient()) as LNURLPayRequest;

        //    Assert.That(result.Callback == new Uri("http://signet.xenon.fun/lnurl/0d3962ff3e11dbd0b9d700b7ac82e02091945169370b3e96e3959c62c929649a"));
        //    Assert.That(result.CommentAllowed == null);
        //    Assert.That(result.MaxSendable.MilliSatoshi == 100000000);
        //    Assert.That(result.MinSendable.MilliSatoshi == 1000);
        //    Assert.That(result.Tag =="payRequest");
        //    Assert.That(result.ParsedMetadata[0].Value == "lorem ipsum blah blah");
        //}


    }
}