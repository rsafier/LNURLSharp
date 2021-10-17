# LNURLSharp.Logic
[![NuGet version (LNURLSharp.Logic)](https://img.shields.io/nuget/v/LNURLSharp.Logic.svg?style=flat-square)](https://www.nuget.org/packages/LNURLSharp.Logic)
[![Deploy Nuget Package](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml/badge.svg)](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml)
## Simple LNURL Server-side logic

### Very simplistic example use cases
BuildLNURLPayResponse for a lightning address endpoint
```
[HttpGet]
[Route("/.well-known/lnurlp/{username}")]
public string WellKnownEndpoint()
{
    var username = Request.Path.Value.SplitOnLast("/").Last();
    var response = LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayResponse($"{username}@{settings.Domain}", $"https://{settings.Domain}/pay/{username}@{settings.Domain}");
    return response.ToJson();
}
```

BuildLNURLPayInvoiceResponse returning LNURLp endpoint
```
[HttpGet]
[Route("/pay/{Username}")]
public async Task<string> MakeLNURLpInvoiceEndpoint()
{
    var username = Request.Path.Value.SplitOnLast("/").Last();
    var amount = long.Parse(Request.Query["amount"]);

    var metadata = new string[2, 2];
    metadata[0, 0] = @"text/plain";
    metadata[0, 1] = $"Send sats to {username}";
    metadata[1, 0] = @"text/identifier";
    metadata[1, 1] = $"{username}";
    var response = await LNURLSharp.Logic.LNURLPayLogic.BuildLNURLPayInvoiceResponse(node.LightningClient, amount, metadata, expiryInSeconds: settings.InvoiceExpiryInSeconds);
    return response.ToJson();
        }
```


Lightning Tips to: tips@safier.com