using Abp.Localization;
using System.Collections.Generic;

namespace AbpCore.OTel.Web.Views.Shared.Components.RightNavbarLanguageSwitch;

public class RightNavbarLanguageSwitchViewModel
{
    public LanguageInfo CurrentLanguage { get; set; }

    public IReadOnlyList<LanguageInfo> Languages { get; set; }
}
