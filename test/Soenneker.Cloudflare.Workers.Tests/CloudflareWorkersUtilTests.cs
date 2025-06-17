using Microsoft.Extensions.Configuration;
using Soenneker.Cloudflare.Workers.Abstract;
using Soenneker.Facts.Local;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Cloudflare.Workers.Tests;

[Collection("Collection")]
public sealed class CloudflareWorkersUtilTests : FixturedUnitTest
{
    private readonly ICloudflareWorkersUtil _util;
    private readonly IConfiguration _config;

    public CloudflareWorkersUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICloudflareWorkersUtil>(true);
        _config = Resolve<IConfiguration>();
    }

    [Fact]
    public void Default()
    {

    }

    [LocalFact]
    public async ValueTask CreateFromGit()
    { 

    }
}
