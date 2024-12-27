using LearnApi.Modal;
using LearnApi.Repos.Models;
using LearnApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService userRoleService;

        public UserRoleController(IUserRoleService roleService) 
        { 
          this.userRoleService=roleService;
        }


        [HttpPost("assignrolepermission")]
        public async Task<IActionResult> assignrolepermission(List<TblRolepermission> rolepermissions)
        {
            var data = await this.userRoleService.AssignRolePermission(rolepermissions);
            return Ok(data);
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await this.userRoleService.GetAllRoles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await this.userRoleService.GetAllMenus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenusbyrole")]
        public async Task<IActionResult> GetAllMenusbyrole(string userrole)
        {
            var data = await this.userRoleService.GetAllMenubyRole(userrole);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetMenupermissionbyrole")]
        public async Task<IActionResult> GetMenupermissionbyrole(string userrole,string menucode)
        {
            var data = await this.userRoleService.GetMenupermissionbyrole(userrole,menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
