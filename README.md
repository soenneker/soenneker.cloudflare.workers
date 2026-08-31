[![](https://img.shields.io/nuget/v/soenneker.cloudflare.workers.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.workers/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.workers/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.workers/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.workers.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.workers/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.workers/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.workers/actions/workflows/codeql.yml)

# Soenneker.Cloudflare.Workers

Uploads, retrieves, lists, and deletes Cloudflare Worker scripts and manages Worker custom domains.

## Installation

```bash
dotnet add package Soenneker.Cloudflare.Workers
```

## Configuration

```json
{
  "Cloudflare": {
    "ApiKey": "your-api-token"
  }
}
```

The token needs Workers Scripts permissions and, for custom domains, the corresponding Workers and zone permissions.

## Registration

```csharp
using Soenneker.Cloudflare.Workers.Registrars;

services.AddCloudflareWorkersUtilAsScoped();
```

Singleton registration is available with `AddCloudflareWorkersUtilAsSingleton()`.

## Scripts

```csharp
using Soenneker.Cloudflare.Workers.Abstract;

const string source = "export default { async fetch() { return new Response('ok'); } };";

await workers.Create(accountId, "status-worker", source, cancellationToken);
```

`Create` uploads the supplied text to the named script and replaces an existing script with the same name. `Update` is an alias for the same operation. `UploadFromFile` reads the entire file as text before uploading it.

```csharp
string? source = await workers.Get(accountId, "status-worker", cancellationToken);
var scripts = await workers.List(accountId, cancellationToken);
```

## Custom domains

```csharp
var result = await workers.AddCustomDomain(
    accountId,
    workerName: "status-worker",
    domainName: "status.example.com",
    zoneId,
    cancellationToken);
```

`RemoveCustomDomain` requires Cloudflare's domain ID, not its hostname. Retrieve IDs with `ListCustomDomains` before removal.

Script upload and domain attachment are separate remote operations. This utility does not perform a transactional deployment, configure routes, upload bindings or secrets, or roll back a domain when a later operation fails. `Delete` permanently removes the named script; reconcile domains and other Worker configuration deliberately before deletion.

Generated Cloudflare API exceptions are propagated. Nullable responses indicate that the generated client can represent an empty response body.
