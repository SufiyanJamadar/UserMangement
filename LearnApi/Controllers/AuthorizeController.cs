using LearnApi.Container;
using LearnApi.Modal;
using LearnApi.Repos;
using LearnApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearndataContext context;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshHandler refresh;
        public AuthorizeController(LearndataContext context,IOptions<JwtSettings> options, IRefreshHandler refresh)
        {
            this.context= context;
            this.jwtSettings= options.Value;
            this.refresh= refresh;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
        {
            var user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == userCred.username && item.Password == userCred.password && item.Isactive==true);
            if (user is not null)
            {
                // Generate Token 
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                  {
                      new Claim(ClaimTypes.Name, user.Username),
                      new Claim(ClaimTypes.Role, user.Role),
                  }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey),SecurityAlgorithms.HmacSha256)
                };
                var token = tokenhandler.CreateToken(tokendesc);
                var finaltoken = tokenhandler.WriteToken(token);
                return Ok(new TokenResponse() { Token=finaltoken,RefreshToken= await this.refresh.GenerateToken(userCred.username),
                UserRole = user.Role
                });

            }
            else
            {
                return Unauthorized();
            }
             
        }


        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            var _refreshtoken = await this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Refreshtoken ==token.RefreshToken);
            if (_refreshtoken is not null)
            {
                //Generate Token 
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                SecurityToken securityToken;
                var principle = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);


                var _token= securityToken as JwtSecurityToken;
                if(_token is not null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256) )
                {
                    string username = principle.Identity?.Name;
                    var _existdata=  this.context.TblRefreshtokens.FirstOrDefault(item=>item.Userid==username
                    && item.Refreshtoken==token.RefreshToken);
                     if(_existdata != null)
                     {
                        var _newtoken = new JwtSecurityToken(
                             claims:principle.Claims.ToArray(),
                             expires:DateTime.Now.AddSeconds(30),
                             signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securitykey))
                             ,SecurityAlgorithms.HmacSha256)
                            );
                        var _finaltoken = tokenhandler.WriteToken(_newtoken);
                        return Ok(new TokenResponse() { Token = _finaltoken, RefreshToken = await this.refresh.GenerateToken(username),
                        UserRole=token.UserRole});
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
                else
                {
                    return Unauthorized();
                }
                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //  {
                //      new Claim(ClaimTypes.Name, user.Username),
                //      new Claim(ClaimTypes.Role, user.Role),
                //  }),
                //    Expires = DateTime.UtcNow.AddMinutes(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                //};
                //var token = tokenhandler.CreateToken(tokendesc);
                //var finaltoken = tokenhandler.WriteToken(token);
                //return Ok(new TokenResponse() { Token = finaltoken, RefreshToken = await this.refresh.GenerateToken(userCred.username) });

            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
