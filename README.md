# HMRC apprenticeship levy client
|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/static/images/govuk-crest-bb9e22aff7881b895c2ceb41d9340804451c474b883f09fe1b4026e76456f44b.png) ||
| Build | [![Build status](https://ci.appveyor.com/api/projects/status/jns8vb1c33dyhy4e?svg=true)](https://ci.appveyor.com/project/scottcowan/hmrc-apprenticeship-levy-api-client) |
| .Net Client |[![](https://img.shields.io/nuget/v/HMRC.ESFA.Levy.Api.Client.svg)](https://www.nuget.org/packages/HMRC.ESFA.Levy.Api.Client/)| 

## An API client for HMRC Apprenticeship Levy


## Usage

### Authentication

You'll need to talk to the ESFA

### Find all the apprenticeship levy declarations 

```c#
var tokenServiceConfig = new TokenServiceApiClientConfiguration
{
    ClientSecret = "<something>",
    ApiBaseUrl = "https://sfa-token-api/",
    ClientId = "<client-guid>",
    IdentifierUri = "<url service identifier for AD>",
    Tenant = "<SFA Azure AD>",
    TokenCertificate = null // used in prod
};

var tokenService = new SFA.DAS.TokenService.Api.Client.TokenServiceApiClient(tokenServiceConfig);
var tokenResult = await tokenService.GetPrivilegedAccessTokenAsync();
var httpClient = HmrcLevyApiClient.CreateHttpClient(tokenResult.AccessCode, "https://hmrc-api-url/");
var client = new HmrcLevyApiClient(httpClient);
var result = await _sut.GetEmployerLevyDeclarations("000/000000");
```