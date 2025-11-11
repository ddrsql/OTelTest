using Microsoft.AspNetCore.Builder;
using VoloAbp.OTel;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("VoloAbp.OTel.Web.csproj"); 
await builder.RunAbpModuleAsync<OTelWebTestModule>(applicationName: "VoloAbp.OTel.Web");

public partial class Program
{
}
