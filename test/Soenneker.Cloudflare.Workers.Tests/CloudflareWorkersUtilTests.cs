using Soenneker.Cloudflare.Workers.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cloudflare.Workers.Tests;

[Collection("Collection")]
public sealed class CloudflareWorkersUtilTests : FixturedUnitTest
{
    private readonly ICloudflareWorkersUtil _util;

    public CloudflareWorkersUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICloudflareWorkersUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
