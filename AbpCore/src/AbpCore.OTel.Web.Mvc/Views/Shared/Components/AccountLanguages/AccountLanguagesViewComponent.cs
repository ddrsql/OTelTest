using Abp.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AbpCore.OTel.Web.Views.Shared.Components.AccountLanguages;

public class AccountLanguagesViewComponent : OTelViewComponent
{
    private readonly ILanguageManager _languageManager;

    public AccountLanguagesViewComponent(ILanguageManager languageManager)
    {
        _languageManager = languageManager;
    }

    public Task<IViewComponentResult> InvokeAsync()
    {
        var model = new LanguageSelectionViewModel
        {
            CurrentLanguage = _languageManager.CurrentLanguage,
            Languages = _languageManager.GetLanguages().Where(l => !l.IsDisabled).ToList()
            .Where(l => !l.IsDisabled)
            .ToList(),
            CurrentUrl = Request.Path
        };

        return Task.FromResult(View(model) as IViewComponentResult);
    }
}
