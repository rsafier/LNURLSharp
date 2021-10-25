# LNURLSharp
[![NuGet version (LNURLSharp.Logic)](https://img.shields.io/nuget/v/LNURLSharp.Logic.svg?style=flat-square)](https://www.nuget.org/packages/LNURLSharp.Logic)
[![Deploy Nuget Package](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml/badge.svg)](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml)
## A simple LNURL server and library package

### Build
---
```
git clone https://github.com/rsafier/LNURLSharp
cd LNURLSharp
dotnet restore
mkdir -p bin
dotnet build -c Release -o bin
```

### Configuration file sample (adjust for your node & domain)
```
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Information"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "log.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "AllowedHosts": "*",
  "LNURLSettings": {
    "Domain": "test.local",
    "EnableTorEndpoint": false,
    "Pay": {
      "InvoiceExpiryInSeconds": 600,
      "MaxSendable": 100000000000,
      "MinSendable": 1000,
    },
    "LNDNodes": [
      {
        "GrpcEndpoint": "https://127.0.0.1:10004",
        "TLSCertBase64": "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNLRENDQWMyZ0F3SUJBZ0lSQVBiNHc3NjZ3am56VXJwRTE1ejNzOG93Q2dZSUtvWkl6ajBFQXdJd01URWYKTUIwR0ExVUVDaE1XYkc1a0lHRjFkRzluWlc1bGNtRjBaV1FnWTJWeWRERU9NQXdHQTFVRUF4TUZZV3hwWTJVdwpIaGNOTWpFd09USTJNRFF6TnpNNVdoY05Nakl4TVRJeE1EUXpOek01V2pBeE1SOHdIUVlEVlFRS0V4WnNibVFnCllYVjBiMmRsYm1WeVlYUmxaQ0JqWlhKME1RNHdEQVlEVlFRREV3VmhiR2xqWlRCWk1CTUdCeXFHU000OUFnRUcKQ0NxR1NNNDlBd0VIQTBJQUJNL2JlTHR4c3hoNmRxMEE3OFlKeXJCdHB1ZkR0R1FncS81WnZidFN4MzBnVnZLMworb2dqRTZCL2dhSHFaNHdvSWhmMGdqbG1Gd2txSlZybDNORUQ0RE9qZ2NVd2djSXdEZ1lEVlIwUEFRSC9CQVFECkFnS2tNQk1HQTFVZEpRUU1NQW9HQ0NzR0FRVUZCd01CTUE4R0ExVWRFd0VCL3dRRk1BTUJBZjh3SFFZRFZSME8KQkJZRUZLSDFzZXFDUVdNaXh0VlVoTTVyZWZ2dGRXWFlNR3NHQTFVZEVRUmtNR0tDQldGc2FXTmxnZ2xzYjJOaApiR2h2YzNTQ0JXRnNhV05sZ2c1d2IyeGhjaTF1TVMxaGJHbGpaWUlFZFc1cGVJSUtkVzVwZUhCaFkydGxkSUlIClluVm1ZMjl1Ym9jRWZ3QUFBWWNRQUFBQUFBQUFBQUFBQUFBQUFBQUFBWWNFckJNQUFqQUtCZ2dxaGtqT1BRUUQKQWdOSkFEQkdBaUVBMVVLNlNLM3MwTWptOWRLSGNRdEs5clZzSXRlVzhKbEY4d0NQY1hrcVpsOENJUUR2Y214UAp4Z3ZPVFZ5WVJiT28wNnhGMHh5NGl1bTA5a0wrbFo4bTNJYklEdz09Ci0tLS0tRU5EIENFUlRJRklDQVRFLS0tLS0K",
        "MacaroonBase64": "AgEDbG5kAvgBAwoQqoXSrrARwAfLsmi3//VxTRIBMBoWCgdhZGRyZXNzEgRyZWFkEgV3cml0ZRoTCgRpbmZvEgRyZWFkEgV3cml0ZRoXCghpbnZvaWNlcxIEcmVhZBIFd3JpdGUaIQoIbWFjYXJvb24SCGdlbmVyYXRlEgRyZWFkEgV3cml0ZRoWCgdtZXNzYWdlEgRyZWFkEgV3cml0ZRoXCghvZmZjaGFpbhIEcmVhZBIFd3JpdGUaFgoHb25jaGFpbhIEcmVhZBIFd3JpdGUaFAoFcGVlcnMSBHJlYWQSBXdyaXRlGhgKBnNpZ25lchIIZ2VuZXJhdGUSBHJlYWQAAAYgoOToq3rQ6kdNZbcWbVCxVwq9UFLHwPbbG2Zkhmj2+jE=",
        "MaxFeePercentage": 0.005,
        "MaxFeeSats": 250,
        "MinFeeSats": 10
      },
      {
        "GrpcEndpoint": "https://127.0.0.1:10008",
        "TLSCertBase64": "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNIakNDQWNPZ0F3SUJBZ0lSQU15UVh4SGZhYVdNYTZadGJoMTliMHd3Q2dZSUtvWkl6ajBFQXdJd0x6RWYKTUIwR0ExVUVDaE1XYkc1a0lHRjFkRzluWlc1bGNtRjBaV1FnWTJWeWRERU1NQW9HQTFVRUF4TURZbTlpTUI0WApEVEl4TURreU5qQTBNemN6T1ZvWERUSXlNVEV5TVRBME16Y3pPVm93THpFZk1CMEdBMVVFQ2hNV2JHNWtJR0YxCmRHOW5aVzVsY21GMFpXUWdZMlZ5ZERFTU1Bb0dBMVVFQXhNRFltOWlNRmt3RXdZSEtvWkl6ajBDQVFZSUtvWkkKemowREFRY0RRZ0FFRTVJSUFpL1hnek9LOHhNZWdsdzRLQVN6eHJ3OHVaWHpxMnFLa1VYS1ZoR3VBcXQ0OURSeQpSaE1YTzJiaWR5L2N1ZVJjMEZWclJFb0ZqZmtHaUp5RHk2T0J2ekNCdkRBT0JnTlZIUThCQWY4RUJBTUNBcVF3CkV3WURWUjBsQkF3d0NnWUlLd1lCQlFVSEF3RXdEd1lEVlIwVEFRSC9CQVV3QXdFQi96QWRCZ05WSFE0RUZnUVUKQ2tOb2lLSVQzaHNoLzdBcnQ4Tmphd0VleUdFd1pRWURWUjBSQkY0d1hJSURZbTlpZ2dsc2IyTmhiR2h2YzNTQwpBMkp2WW9JTWNHOXNZWEl0YmpFdFltOWlnZ1IxYm1sNGdncDFibWw0Y0dGamEyVjBnZ2RpZFdaamIyNXVod1IvCkFBQUJoeEFBQUFBQUFBQUFBQUFBQUFBQUFBQUJod1NzRXdBRE1Bb0dDQ3FHU000OUJBTUNBMGtBTUVZQ0lRQ0UKdjd1WjhJZFgwVGJlalBNb3dYYmpRaTViOFl1aGQvbTQ2aEVTMm5KU3BnSWhBS29mb05ZL2ZraFJ6aWs4ZVNpMQo1cGdsRWhEdEZjSWRzbVcva0FRQ0t0c28KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=",
        "MacaroonBase64": "AgEDbG5kAvgBAwoQyX0gHA8sgTmituPzZss1thIBMBoWCgdhZGRyZXNzEgRyZWFkEgV3cml0ZRoTCgRpbmZvEgRyZWFkEgV3cml0ZRoXCghpbnZvaWNlcxIEcmVhZBIFd3JpdGUaIQoIbWFjYXJvb24SCGdlbmVyYXRlEgRyZWFkEgV3cml0ZRoWCgdtZXNzYWdlEgRyZWFkEgV3cml0ZRoXCghvZmZjaGFpbhIEcmVhZBIFd3JpdGUaFgoHb25jaGFpbhIEcmVhZBIFd3JpdGUaFAoFcGVlcnMSBHJlYWQSBXdyaXRlGhgKBnNpZ25lchIIZ2VuZXJhdGUSBHJlYWQAAAYgAXIj3erefR3HKonI/fFyz/GrQ6zXK+HvZONSyl9sSW0=",
        "MaxFeePercentage": 0.005,
        "MaxFeeSats": 250,
        "MinFeeSats": 10
      }
    ]
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      },
      "Https": {
        "Url": "https://localhost:5001"
      }
    }
  }
}

```

#### Notes
---
- Supports LNURL spec # 01 03 06 12 16
- Limited support of LUD 18 (has data structures and proper hashing but no mechenism yet to define response settings)
---
Lightning Tips to: tips@safier.com

LNURLp QR: 
![LNURLp Donation Link](https://github.com/rsafier/LNURLSharp/raw/master/richardj-qr-code.png)