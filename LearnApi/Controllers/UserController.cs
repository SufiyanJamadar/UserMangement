using LearnApi.Modal;
using LearnApi.Repos;
using LearnApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService service)
        {
            this.userService = service;
        }

        [HttpPost("userregistration")]
        public async Task<IActionResult> UserRegistration(UserRegister userRegister)
        {
            var data= await this.userService.UserRegistration(userRegister);
            return Ok(data);
        }

        [HttpPost("confirmregistration")]
        public async Task<IActionResult> ConfirmRegistration(int userid, string username,string otptext)
        {
            var data =await this.userService.ConfirmRegister(userid,username,otptext);
            return Ok(data);
        }


        [HttpPost("resetpassword")]
        public async Task<IActionResult> resetpassword( string username,string oldpassword,string newpassword)
        {
            var data = await this.userService.ResetPassword(username,oldpassword,newpassword);
            return Ok(data);
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> forgetpassword(string username)
        {
            var data = await this.userService.ForgetPassword(username);
            return Ok(data);
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> updatepassword(string username,string password,string otptext)
        {
            var data = await this.userService.UpdatePassword(username,password,otptext);
            return Ok(data);
        }

        [HttpPost("updatestatus")]
        public async Task<IActionResult> updatestatus(string username,bool status)
        {
            var data = await this.userService.UpdateStatus(username,status);
            return Ok(data);
        }

        [HttpPost("updaterole")]
        public async Task<IActionResult> updaterole(string username, string role)
        {
            var data = await this.userService.UpdateRole(username,role);
            return Ok(data);
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
           var data=  await this.userService.Getall();
            if (data == null)
            {
                return NotFound(); 
            }

            return Ok(data);
        }


        [HttpGet("Getbycode")]
        public async Task<IActionResult> Getbycode(string code)
        {
            var data = await this.userService.Getbycode(code);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}
