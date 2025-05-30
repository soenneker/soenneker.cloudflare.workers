using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.Utils.Client.Registrars;
using Soenneker.Cloudflare.Workers.Abstract;

namespace Soenneker.Cloudflare.Workers.Registrars;

/// <summary>
/// A utility for managing Cloudflare Workers
/// </summary>
public static class CloudflareWorkersUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareWorkersUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareWorkersUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddSingleton<ICloudflareWorkersUtil, CloudflareWorkersUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareWorkersUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareWorkersUtilAsScoped(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddScoped<ICloudflareWorkersUtil, CloudflareWorkersUtil>();

        return services;
    }
}
