using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using AbpCore.OTel.Authorization;
using AbpCore.OTel.Controllers;
using AbpCore.OTel.Roles;
using AbpCore.OTel.Web.Models.Roles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AbpCore.OTel.Web.Controllers;

[AbpMvcAuthorize(PermissionNames.Pages_Roles)]
public class RolesController : OTelControllerBase
{
    private readonly IRoleAppService _roleAppService;

    public RolesController(IRoleAppService roleAppService)
    {
        _roleAppService = roleAppService;
    }

    public async Task<IActionResult> Index()
    {
        var permissions = (await _roleAppService.GetAllPermissions()).Items;
        var model = new RoleListViewModel
        {
            Permissions = permissions
        };

        return View(model);
    }

    public async Task<ActionResult> EditModal(int roleId)
    {
        var output = await _roleAppService.GetRoleForEdit(new EntityDto(roleId));
        var model = ObjectMapper.Map<EditRoleModalViewModel>(output);

        return PartialView("_EditModal", model);
    }
}
