using Volo.Abp.Settings;

namespace VoloAbp.OTel.Settings;

public class OTelSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(OTelSettings.MySetting1));
    }
}
