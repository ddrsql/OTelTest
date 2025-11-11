using Xunit;

namespace VoloAbp.OTel.EntityFrameworkCore;

[CollectionDefinition(OTelTestConsts.CollectionDefinitionName)]
public class OTelEntityFrameworkCoreCollection : ICollectionFixture<OTelEntityFrameworkCoreFixture>
{

}
