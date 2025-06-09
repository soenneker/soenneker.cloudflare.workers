using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient.Models;

namespace Soenneker.Cloudflare.Workers.Abstract;

/// <summary>
/// Utility for interacting with Cloudflare Workers via OpenAPI
/// </summary>
public interface ICloudflareWorkersUtil
{
    /// <summary>
    /// Creates or updates a Worker script
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="scriptContent">The content of the Worker script</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_scriptResponseSingle?> Create(string accountId, string name, string scriptContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a Worker script by name
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Worker_script_download_worker_Response_200_multipart_form_data?>
        Get(string accountId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a Worker script (alias for Create)
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="scriptContent">The content of the Worker script</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_scriptResponseSingle?> Update(string accountId, string name, string scriptContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a Worker script
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask Delete(string accountId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all Workers in an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<IEnumerable<Workers_scriptResponse>> List(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a Worker script from a file
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="filePath">The path to the script file</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_scriptResponseSingle?> UploadFromFile(string accountId, string name, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a custom domain to a Worker
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="workerName">The name of the Worker</param>
    /// <param name="domainName">The custom domain name to add</param>
    /// <param name="zoneId">The ID of the zone where the domain is registered</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_domainResponseSingle> AddCustomDomain(string accountId, string workerName, string domainName, string zoneId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a custom domain from a Worker
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="domainId">The ID of the custom domain to remove</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask RemoveCustomDomain(string accountId, string domainId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all custom domains for Workers in an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<IEnumerable<Workers_domain>> ListCustomDomains(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a Worker from a GitHub repository using owner and repository name
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="owner">The GitHub repository owner</param>
    /// <param name="repository">The GitHub repository name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_scriptResponseSingle?> CreateFromGit(string accountId, string name, string owner, string repository,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a Worker from a Git repository URL
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="name">The name of the Worker</param>
    /// <param name="repositoryUrl">The Git repository URL</param>
    /// <param name="cancellationToken">The cancellation token</param>
    ValueTask<Workers_scriptResponseSingle?> CreateFromGit(string accountId, string name, string repositoryUrl,
        CancellationToken cancellationToken = default);
}