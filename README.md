# LNURLSharp
[![NuGet version (LNURLSharp.Logic)](https://img.shields.io/nuget/v/LNURLSharp.Logic.svg?style=flat-square)](https://www.nuget.org/packages/LNURLSharp.Logic)
[![Deploy Nuget Package](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml/badge.svg)](https://github.com/rsafier/LNURLSharp/actions/workflows/PublishNuget.yml)
## A simple LNURL server and library package

### Install
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
  "LNDSettings": {
    "GrpcEndpoint": "https://127.0.0.1:10004",
    "TLSCertBase64": "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNLRENDQWMyZ0F3SUJBZ0lSQVBiNHc3NjZ3am56VXJwRTE1ejNzOG93Q2dZSUtvWkl6ajBFQXdJd01URWYKTUIwR0ExVUVDaE1XYkc1a0lHRjFkRzluWlc1bGNtRjBaV1FnWTJWeWRERU9NQXdHQTFVRUF4TUZZV3hwWTJVdwpIaGNOTWpFd09USTJNRFF6TnpNNVdoY05Nakl4TVRJeE1EUXpOek01V2pBeE1SOHdIUVlEVlFRS0V4WnNibVFnCllYVjBiMmRsYm1WeVlYUmxaQ0JqWlhKME1RNHdEQVlEVlFRREV3VmhiR2xqWlRCWk1CTUdCeXFHU000OUFnRUcKQ0NxR1NNNDlBd0VIQTBJQUJNL2JlTHR4c3hoNmRxMEE3OFlKeXJCdHB1ZkR0R1FncS81WnZidFN4MzBnVnZLMworb2dqRTZCL2dhSHFaNHdvSWhmMGdqbG1Gd2txSlZybDNORUQ0RE9qZ2NVd2djSXdEZ1lEVlIwUEFRSC9CQVFECkFnS2tNQk1HQTFVZEpRUU1NQW9HQ0NzR0FRVUZCd01CTUE4R0ExVWRFd0VCL3dRRk1BTUJBZjh3SFFZRFZSME8KQkJZRUZLSDFzZXFDUVdNaXh0VlVoTTVyZWZ2dGRXWFlNR3NHQTFVZEVRUmtNR0tDQldGc2FXTmxnZ2xzYjJOaApiR2h2YzNTQ0JXRnNhV05sZ2c1d2IyeGhjaTF1TVMxaGJHbGpaWUlFZFc1cGVJSUtkVzVwZUhCaFkydGxkSUlIClluVm1ZMjl1Ym9jRWZ3QUFBWWNRQUFBQUFBQUFBQUFBQUFBQUFBQUFBWWNFckJNQUFqQUtCZ2dxaGtqT1BRUUQKQWdOSkFEQkdBaUVBMVVLNlNLM3MwTWptOWRLSGNRdEs5clZzSXRlVzhKbEY4d0NQY1hrcVpsOENJUUR2Y214UAp4Z3ZPVFZ5WVJiT28wNnhGMHh5NGl1bTA5a0wrbFo4bTNJYklEdz09Ci0tLS0tRU5EIENFUlRJRklDQVRFLS0tLS0K",
    "MacaroonBase64": "AgEDbG5kAvgBAwoQqoXSrrARwAfLsmi3//VxTRIBMBoWCgdhZGRyZXNzEgRyZWFkEgV3cml0ZRoTCgRpbmZvEgRyZWFkEgV3cml0ZRoXCghpbnZvaWNlcxIEcmVhZBIFd3JpdGUaIQoIbWFjYXJvb24SCGdlbmVyYXRlEgRyZWFkEgV3cml0ZRoWCgdtZXNzYWdlEgRyZWFkEgV3cml0ZRoXCghvZmZjaGFpbhIEcmVhZBIFd3JpdGUaFgoHb25jaGFpbhIEcmVhZBIFd3JpdGUaFAoFcGVlcnMSBHJlYWQSBXdyaXRlGhgKBnNpZ25lchIIZ2VuZXJhdGUSBHJlYWQAAAYgoOToq3rQ6kdNZbcWbVCxVwq9UFLHwPbbG2Zkhmj2+jE="
  },
  "LNURLSettings": {
    "Domain": "test.local",
    "InvoiceExpiryInSeconds": 600
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

Tips can be send via Lightning Address to: tips@safier.com