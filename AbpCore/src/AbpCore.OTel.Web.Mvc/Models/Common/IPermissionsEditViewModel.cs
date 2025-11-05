using AbpCore.OTel.Roles.Dto;
using System.Collections.Generic;

namespace AbpCore.OTel.Web.Models.Common;

public interface IPermissionsEditViewModel
{
    List<FlatPermissionDto> Permissions { get; set; }
}