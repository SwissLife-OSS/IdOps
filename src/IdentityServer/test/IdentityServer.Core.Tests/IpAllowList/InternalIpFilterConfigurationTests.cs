using IdOps.IdentityServer;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace IdOps.IpAllowList;

public class InternalIpFilterConfigurationTests
{
    [Fact]
    public void JsonConfig_Bind_AppliedCorrect()
    {
        string jsonString = "{\"InternalIpFilter\": {\"InternalIpAllowList\": [\"127.0.0.1\"]}}";

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        IConfigurationRoot? config = new ConfigurationBuilder()
            .AddJsonStream(memoryStream)
            .Build();

        InternalIpFilterConfiguration? internalIpFilterConfig =
            config.GetSection("InternalIpFilter").Get<InternalIpFilterConfiguration>();

        Assert.NotNull(internalIpFilterConfig);
        Assert.NotEmpty(internalIpFilterConfig.InternalIpAllowList);
        Assert.NotEmpty(internalIpFilterConfig.InternalIpAllowListParsed.Value);
    }
}
