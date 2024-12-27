﻿using LearnApi.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace LearnApi.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly LearndataContext context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,LearndataContext context) : base(options, logger, encoder, clock)
        {
            this.context=context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No Header Found");
            }

            var headervalue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if(headervalue != null)
            {
                var bytes = Convert.FromBase64String(headervalue.Parameter);
                string credentiails=Encoding.UTF8.GetString(bytes);
                string[] array = credentiails.Split(":");
                string username = array[0];
                string password = array[1];
                var user= await this.context.TblUsers.FirstOrDefaultAsync(item=>item.Username == username && item.Password==password);
                 if(user is not null)
                 {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Username) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principle = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principle, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                 }
                else
                {
                    return AuthenticateResult.Fail("UnAuthrized");
                }
            }
            else
            {
                return AuthenticateResult.Fail("Empty Header");
            }
        }

    }
}
