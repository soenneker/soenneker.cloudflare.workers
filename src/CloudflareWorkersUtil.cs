using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;
using Soenneker.Cloudflare.OpenApiClient;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;
using Soenneker.Cloudflare.Workers.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Soenneker.Cloudflare.Workers;

///<inheritdoc cref="ICloudflareWorkersUtil"/>
public sealed class CloudflareWorkersUtil : ICloudflareWorkersUtil
{
    private readonly ILogger<CloudflareWorkersUtil> _logger;
    private readonly ICloudflareClientUtil _clientUtil;

    public CloudflareWorkersUtil(ICloudflareClientUtil clientUtil, ILogger<CloudflareWorkersUtil> logger)
    {
        _clientUtil = clientUtil;
        _logger = logger;
    }

    public async ValueTask<Workers_scriptResponseSingle?> Create(string accountId, string name, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating or updating Worker {Name} in account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            var multipartBody = new MultipartBody();
            multipartBody.AddOrReplacePart("script", "text/plain", scriptContent);
            Workers_scriptResponseSingle? result =
                await client.Accounts[accountId].Workers.Scripts[name].Content.PutAsync(multipartBody, null, cancellationToken);
            _logger.LogInformation("Successfully created or updated Worker {Name}", name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create or update Worker {Name}", name);
            throw;
        }
    }

    public async ValueTask<Worker_script_download_worker_Response_200_multipart_form_data?> Get(string accountId, string name,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Worker {Name} from account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            Worker_script_download_worker_Response_200_multipart_form_data? result =
                await client.Accounts[accountId].Workers.Scripts[name].GetAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully retrieved Worker {Name}", name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Worker {Name}", name);
            throw;
        }
    }

    public ValueTask<Workers_scriptResponseSingle?> Update(string accountId, string name, string scriptContent,
        CancellationToken cancellationToken = default) => Create(accountId, name, scriptContent, cancellationToken);

    public async ValueTask Delete(string accountId, string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Worker {Name} from account {AccountId}", name, accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            var body = new Worker_script_delete_worker_RequestBody_application_json();
            await client.Accounts[accountId].Workers.Scripts[name].DeleteAsync(body, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully deleted Worker {Name}", name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Worker {Name}", name);
            throw;
        }
    }

    public async ValueTask<IEnumerable<Workers_scriptResponse>> List(string accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing Workers in account {AccountId}", accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            Workers_scriptResponseCollection? result = await client.Accounts[accountId].Workers.Scripts.GetAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully listed Workers");
            return result != null && result.Result != null ? result.Result : new List<Workers_scriptResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list Workers");
            throw;
        }
    }

    public async ValueTask<Workers_scriptResponseSingle?> UploadFromFile(string accountId, string name, string filePath,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading Worker {Name} from file {FilePath}", name, filePath);

        try
        {
            string scriptContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            return await Create(accountId, name, scriptContent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload Worker {Name} from file {FilePath}", name, filePath);
            throw;
        }
    }

    public async ValueTask<Workers_domainResponseSingle> AddCustomDomain(string accountId, string workerName, string domainName, string zoneId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding custom domain {DomainName} to Worker {WorkerName}", domainName, workerName);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            var domain = new Worker_domain_attach_to_domain_RequestBody_application_json
            {
                Hostname = domainName,
                ZoneId = zoneId,
                Service = workerName
            };

            Workers_domainResponseSingle? result = await client.Accounts[accountId].Workers.Domains.PutAsync(domain, cancellationToken: cancellationToken);
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
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            var body = new Worker_domain_detach_from_domain_RequestBody_application_json();
            await client.Accounts[accountId].Workers.Domains[domainId].DeleteAsync(body, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully removed custom domain {DomainId} from Worker", domainId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove custom domain {DomainId} from Worker", domainId);
            throw;
        }
    }

    public async ValueTask<IEnumerable<Workers_domain>> ListCustomDomains(string accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing custom domains for Workers in account {AccountId}", accountId);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken);
        try
        {
            Workers_domainResponseCollection? result = await client.Accounts[accountId].Workers.Domains.GetAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully listed custom domains for Workers");
            return result != null && result.Result != null ? result.Result : new List<Workers_domain>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list custom domains for Workers");
            throw;
        }
    }
}