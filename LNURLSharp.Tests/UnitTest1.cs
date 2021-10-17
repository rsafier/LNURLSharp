using NUnit.Framework;
using System;
using System.Threading.Tasks;
using LNURLSharp.Logic;
using System.Diagnostics;

namespace LNURLSharp.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public async Task LNURLDecode()
        {
            var result = "lnurl1dp68gup69uhhx6t8dejhgtncv4hx7m3wve6kuw3cxqhkcmn4wfkr7ufax9jrgdmrv56nzcmyv9snwwtzxfjr2vp4xgurzdtxxqek2dn9vs6rjwfhxucnvvr9vfjkxcfevsekxe34vserscf4vgurwvrrxucnvegtpvm27".ParseLNURL();
            Assert.That(result.uri == new Uri("http://signet.xenon.fun:80/lnurl?q=1d47ce51cdaa79b2d5052815f03e6ed49977160ebeca9d3cf5d28a5b870c716e"));
        }

        [Test]
        public async Task LNURLPayrequest()
        {
            var result = await LNURLSharp.Logic.LNURLClient.FetchInformation(new Uri("https://safier.com/.well-known/lnurlp/richardj"), new System.Net.Http.HttpClient()) as LNURLPayResponse;
            Assert.That(result != null);
            Assert.That(result.MinSendable == 1000);
            Assert.That(result.MaxSendable == 100000000000);
            Assert.That(result.Callback == "https://safier.com/pay/richardj@safier.com");
            Assert.That(result.CommentAllowed == null);
            Assert.That(result.Metadata == "[[\"text/plain\",\"Send sats to richardj@safier.com\"],[\"text/identifier\",\"richardj@safier.com\"]]");
            Assert.That(result.Tag == "payRequest");
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