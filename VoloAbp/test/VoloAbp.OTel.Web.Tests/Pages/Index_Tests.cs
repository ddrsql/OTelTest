using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace VoloAbp.OTel.Pages;

[Collection(OTelTestConsts.CollectionDefinitionName)]
public class Index_Tests : OTelWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
