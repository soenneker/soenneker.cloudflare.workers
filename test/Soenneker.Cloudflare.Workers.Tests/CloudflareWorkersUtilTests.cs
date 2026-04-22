using Microsoft.Extensions.Configuration;
using Soenneker.Cloudflare.Workers.Abstract;
using Soenneker.Tests.Attributes.Local;
using Soenneker.Tests.HostedUnit;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.Workers.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloudflareWorkersUtilTests : HostedUnitTest
{
    private readonly ICloudflareWorkersUtil _util;
    private readonly IConfiguration _config;

    public CloudflareWorkersUtilTests(Host host) : base(host)
    {
        _util = Resolve<ICloudflareWorkersUtil>(true);
        _config = Resolve<IConfiguration>();
    }

    [Test]
    public void Default()
    {

    }

    [LocalOnly]
    public async ValueTask CreateFromGit()
    { 

    }
}
