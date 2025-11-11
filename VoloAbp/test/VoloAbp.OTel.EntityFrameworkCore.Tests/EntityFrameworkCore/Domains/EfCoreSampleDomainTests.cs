using VoloAbp.OTel.Samples;
using Xunit;

namespace VoloAbp.OTel.EntityFrameworkCore.Domains;

[Collection(OTelTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<OTelEntityFrameworkCoreTestModule>
{

}
