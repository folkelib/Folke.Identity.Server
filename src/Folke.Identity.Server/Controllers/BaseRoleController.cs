using Folke.Identity.Server.Services;
using Folke.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;

namespace Folke.Identity.Server.Controllers
{
    [Authorize("Role")]
    public class BaseRoleController<TRole, TRoleView, TKey, TUser> : TypedControllerBase
        where TRole : class
        where TUser : class
    {
        private readonly IRoleService<TRole, TRoleView> roleService;
        private readonly RoleManager<TRole> roleManager;
        private readonly UserManager<TUser> userManager;

        public BaseRoleController(IRoleService<TRole, TRoleView> roleService, RoleManager<TRole> roleManager, UserManager<TUser> userManager)
        {
            this.roleService = roleService;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IHttpActionResult<TRoleView>> Get(string id)
        {
            return Ok(roleService.MapToRoleView(await roleManager.FindByIdAsync(id)));
        }

        [HttpGet]
        public IHttpActionResult<IEnumerable<TRoleView>> GetAll()
        {
            return Ok(roleManager.Roles.ToList().Select(x => roleService.MapToRoleView(x)));
        }

        [HttpPost]
        public async Task<IHttpActionResult<TRoleView>> Create([FromBody]string name)
        {
            var role = roleService.CreateNewRole(name);
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return BadRequest<TRoleView>(ModelState);
            }
            return Created("GetRole", Convert.ChangeType(await roleManager.GetRoleIdAsync(role), typeof(TKey)), roleService.MapToRoleView(role));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return Ok();
        }

        [HttpPost("~/api/user/{userId}/role")]
        public async Task<IActionResult> AddUser([FromBody]string roleName,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.AddToRoleAsync(user, roleName);
            return Ok();
        }

        [HttpDelete("~/api/user/{userId}/role")]
        public async Task<IActionResult> DeleteUser([FromBody]string roleName, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.RemoveFromRoleAsync(user, roleName);
            return Ok();
        }

        [HttpGet("~/api/user/{userId}/role")]
        public async Task<IHttpActionResult<IList<string>>> GetForUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var roles = await userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
