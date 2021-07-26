using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Backend.Services;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Backend.Models;

namespace Backend.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly UserService _service;
        public AuthController(UserService service)
        {
            _service = service;
        }
        
        [HttpPut("login")]
        public IActionResult Login([FromBody]Login login)
        {
            var user = _service.GetSingleUserName(login.Username);
            bool existUser = (user != null && login.Password == "pwd");
            if (existUser)
            {
                var requestAt = DateTime.Now;
                var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;
                var token = GenerateToken(login, expiresIn, user);

                return Ok(
                    new
                    {
                        requertAt = requestAt,
                        expiresIn = TokenAuthOption.ExpiresSpan.TotalSeconds,
                        tokeyType = TokenAuthOption.TokenType,
                        token = token
                    }
                );
            }
            else
            {
                //return Unauthorized();
                // return StatusCode(401, new{
                //     Msg = "Username or password is invalid"
                // });
                return StatusCode(401, "Username or password is invalid");
            }
        }

        private string GenerateToken(Login login, DateTime expires, User user)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                // new GenericIdentity(login.Username, "TokenAuth"),
                new[] {
                    new Claim(ClaimTypes.Name, login.Username.ToString()),
                    new Claim("username", login.Username.ToString()),
                    new Claim(ClaimTypes.Role,  $"{(int)user.Role}"),
                    new Claim("id", user.Id.ToString()),
                    }
            );

            var securityToken = handler
            .CreateToken(
                new SecurityTokenDescriptor
                {
                    Issuer = TokenAuthOption.Issuer,
                    Audience = TokenAuthOption.Audience,
                    SigningCredentials = TokenAuthOption.SigningCredentials,
                    Subject = identity,
                    Expires = expires
                });
            return handler.WriteToken(securityToken);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            
            return Ok(new
            {
                Username = claimsIdentity.Name,
                Role = claimsIdentity.Claims.Single(
                    m => m.Type == ClaimTypes.Role).Value
            });
        }

        [AllowAnonymous]
        [HttpGet("anonymous")]
        public IActionResult Anonymous()
        {
            return new ContentResult() { Content = $@"For all anonymous." };
        }
    }
}