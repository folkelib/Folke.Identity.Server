using Folke.Identity.Server.Services;
using Folke.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Folke.Identity.Server.Controllers
{
    [Route("api/role")]
    public class RoleController<TRole, TRoleView, TKey, TUser> : TypedControllerBase
        where TRole : class
        where TUser : class
    {
        private readonly IRoleService<TRole, TRoleView> roleService;
        private readonly RoleManager<TRole> roleManager;
        private readonly UserManager<TUser> userManager;

        public RoleController(IRoleService<TRole, TRoleView> roleService, RoleManager<TRole> roleManager, UserManager<TUser> userManager)
        {
            this.roleService = roleService;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Authorize("Role")]
        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IHttpActionResult<TRoleView>> Get(string id)
        {
            return Ok(roleService.MapToRoleView(await roleManager.FindByIdAsync(id)));
        }

        [Authorize("Role")]
        [HttpGet("")]
        public IHttpActionResult<IEnumerable<TRoleView>> GetAll()
        {
            return Ok(roleManager.Roles.ToList().Select(x => roleService.MapToRoleView(x)));
        }

        [Authorize("Role")]
        [HttpPost("")]
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

        [Authorize("Role")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return Ok();
        }
        
        [Authorize("Role")]
        [HttpPost("~/api/user/{userId}/role")]
        public async Task<IActionResult> AddUser([FromBody]string roleName,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.AddToRoleAsync(user, roleName);
            return Ok();
        }

        [Authorize("Role")]
        [HttpDelete("~/api/user/{userId}/role")]
        public async Task<IActionResult> DeleteUser([FromBody]string roleName, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.RemoveFromRoleAsync(user, roleName);
            return Ok();
        }

        [Authorize("Role")]
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
                ModelState.AddModelError(error.Code, error.Description);
            }
        }
    }
}
