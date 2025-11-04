using Abp.Application.Navigation;

namespace AbpFramework.OTel.WebMpa.Models.Layout
{
    public class SideBarNavViewModel
    {
        public UserMenu MainMenu { get; set; }

        public string ActiveMenuItemName { get; set; }
    }
}