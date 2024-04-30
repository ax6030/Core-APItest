using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todo.Dto;
using Todo.Models;

namespace Todo.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TodoListContext _todoListContext;
        private readonly IConfiguration _configuration;

        public LoginController(TodoListContext todoListContext, IConfiguration configuration)
        {
            _todoListContext = todoListContext;
            _configuration = configuration;
        }

        [HttpGet("NoLogin")]
        public string NoLogin()
        {
            return "未登入";
        }
        [HttpGet("NoAccess")]
        public string noAccess()
        {
            return "沒有權限";
        }







        [HttpPost]
        public string login(LoginPostDto value)
        {
            var user = _todoListContext.Employees
                .Where(x => x.Account == value.Account && x.Password == value.Password)
                .SingleOrDefault();
            if(user == null)
            {
                return "帳號密碼錯誤";
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim("FullName", user.Name),
                    new Claim("EmployeeId", user.EmployeeId.ToString())
                    //new Claim(ClaimTypes.Role, "select")  //直接設定Role = "select"
                };

                var role = _todoListContext.Roles.Where(x => x.EmployeeId == user.EmployeeId);

                foreach(var temp in role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, temp.Name));
                }

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(5)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return "Ok";
            }
        }

        [HttpPost("jwtLogin")]
        public string jwtlogin(LoginPostDto value)
        {
            var user = _todoListContext.Employees
                .Where(x => x.Account == value.Account && x.Password == value.Password)
                .SingleOrDefault();
            if (user == null)
            {
                return "帳號密碼錯誤";
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email,user.Account),
                    new Claim("FullName", user.Name),
                    new Claim(JwtRegisteredClaimNames.NameId, user.EmployeeId.ToString()),
                    new Claim("EmployeeId", user.EmployeeId.ToString())

                };

                var role = _todoListContext.Roles.Where(x => x.EmployeeId == user.EmployeeId);

                foreach (var temp in role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, temp.Name));
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

                var jwt = new JwtSecurityToken
                    (
                    issuer : _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims : claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                    );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                
                return token;
            }
        }



        [HttpDelete]
        public void loginout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        
    }
}
