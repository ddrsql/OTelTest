using VoloAbp.OTel.Samples;
using Xunit;

namespace VoloAbp.OTel.EntityFrameworkCore.Applications;

[Collection(OTelTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<OTelEntityFrameworkCoreTestModule>
{

}
