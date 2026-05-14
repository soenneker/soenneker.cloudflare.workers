using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;
using Soenneker.Cloudflare.OpenApiClient;
using Soenneker.Cloudflare.OpenApiClient.Accounts.Item.Workers.Scripts.Item;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;
using Soenneker.Cloudflare.Workers.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.File.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.Workers;

///<inheritdoc cref="ICloudflareWorkersUtil"/>
public sealed class CloudflareWorkersUtil : ICloudflareWorkersUtil
{
    private readonly ILogger<CloudflareWorkersUtil> _logger;
    private readonly ICloudflareClientUtil _clientUtil;
    private readonly IFileUtil _fileUtil;

    public CloudflareWorkersUtil(ICloudflareClientUtil clientUtil, ILogger<CloudflareWorkersUtil> logger, IFileUtil fileUtil)
    {
        _clientUtil = clientUtil;
        _logger = logger;
        _fileUtil = fileUtil;
    }

    public async ValueTask<WorkersScriptResponseSingle?> Create(string accountId, string name, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating or updating Worker {Name} in account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            var multipartBody = new MultipartBody();
            multipartBody.AddOrReplacePart("script", "text/plain", scriptContent);
            WorkersScriptResponseSingle? result =
                await client.Accounts[accountId].Workers.Scripts[name].Content.PutAsync(multipartBody, null, cancellationToken).NoSync();
            _logger.LogInformation("Successfully created or updated Worker {Name}", name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create or update Worker {Name}", name);
            throw;
        }
    }

    public async ValueTask<string?> Get(string accountId, string name,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Worker {Name} from account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            string? result = await client.Accounts[accountId].Workers.Scripts[name].GetAsync(cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully retrieved Worker {Name}", name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Worker {Name}", name);
            throw;
        }
    }

    public ValueTask<WorkersScriptResponseSingle?> Update(string accountId, string name, string scriptContent,
        CancellationToken cancellationToken = default) => Create(accountId, name, scriptContent, cancellationToken);

    public async ValueTask Delete(string accountId, string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Worker {Name} from account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            var body = new WithScript_nameDeleteRequestBody();
            await client.Accounts[accountId].Workers.Scripts[name].DeleteAsync(body, cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully deleted Worker {Name}", name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Worker {Name}", name);
            throw;
        }
    }

    public async ValueTask<IEnumerable<WorkersScriptResponseCollection_result>> List(string accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing Workers in account {AccountId}", accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            WorkersScriptResponseCollection? result = await client.Accounts[accountId].Workers.Scripts.GetAsync(cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully listed Workers");
            return result != null && result.Result != null ? result.Result : new List<WorkersScriptResponseCollection_result>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list Workers");
            throw;
        }
    }

    public async ValueTask<WorkersScriptResponseSingle?> UploadFromFile(string accountId, string name, string filePath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading Worker {Name} from file {FilePath}", name, filePath);

        try
        {
            string scriptContent = await _fileUtil.Read(filePath, log: false, cancellationToken).NoSync();
            return await Create(accountId, name, scriptContent, cancellationToken).NoSync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload Worker {Name} from file {FilePath}", name, filePath);
            throw;
        }
    }

    public async ValueTask<WorkersDomainsUpdate200?> AddCustomDomain(string accountId, string workerName, string domainName, string zoneId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding custom domain {DomainName} to Worker {WorkerName}", domainName, workerName);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            var domain = new WorkersDomainsUpdate
            {
                Hostname = domainName,
                ZoneId = zoneId,
                Service = workerName
            };

            WorkersDomainsUpdate200? result = await client.Accounts[accountId].Workers.Domains.PutAsync(domain, cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully added custom domain {DomainName} to Worker {WorkerName}", domainName, workerName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add custom domain {DomainName} to Worker {WorkerName}", domainName, workerName);
            throw;
        }
    }

    public async ValueTask RemoveCustomDomain(string accountId, string domainId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing custom domain {DomainId} from Worker", domainId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            await client.Accounts[accountId].Workers.Domains[domainId].DeleteAsync(cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully removed custom domain {DomainId} from Worker", domainId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove custom domain {DomainId} from Worker", domainId);
            throw;
        }
    }

    public async ValueTask<IEnumerable<WorkersDomain>> ListCustomDomains(string accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing custom domains for Workers in account {AccountId}", accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        try
        {
            WorkersDomainsList200? result = await client.Accounts[accountId].Workers.Domains.GetAsync(cancellationToken: cancellationToken).NoSync();
            _logger.LogInformation("Successfully listed custom domains for Workers");
            return result != null && result.Result != null ? result.Result : new List<WorkersDomain>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list custom domains for Workers");
            throw;
        }
    }
}
