using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Folke.Identity.Server.Controllers
{
    public class BaseRoleController<TRole, TRoleView, TKey> : TypedControllerBase
        where TRole : class
    {
        private readonly IRoleService<TRole, TRoleView> roleService;
        private readonly RoleManager<TRole> roleManager;

        public BaseRoleController(IRoleService<TRole, TRoleView> roleService, RoleManager<TRole> roleManager)
        {
            this.roleService = roleService;
            this.roleManager = roleManager;
        }

        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IHttpActionResult<TRoleView>> Get(string id)
        {
            return Ok(roleService.MapToRoleView(await roleManager.FindByIdAsync(id)));
        }

        [HttpPost]
        public async Task<IHttpActionResult<TRoleView>> Create([FromBody]TRoleView view)
        {
            var role = roleService.CreateNewRole(view);
            await roleManager.CreateAsync(role);
            return Created("GetRole", Convert.ChangeType(await roleManager.GetRoleIdAsync(role), typeof(TKey)), view);
        }
    }
}
