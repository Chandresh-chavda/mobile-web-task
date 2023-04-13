
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace User_Interface.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        

        public AuthenticationController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email);
            if (user == null)
            {
                return BadRequest();
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, data.Password))
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

              
               

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                   

                }); 
            }
            else
            {
                return Unauthorized();
            }
        }
       



        [HttpPost]
        public async Task<IActionResult> Registration([FromBody] DealerRegistration data)
        {
            var userExists = await _userManager.FindByNameAsync(data.Email);
            if (userExists != null)
            {
                return BadRequest("user already exist");
            }
            else
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName=data.Name,
                    City=data.City,
                    PostalCode=data.PostalCode,
                    State=data.State,
                    IsActive=true,
                    Country=data.Country,
                    Email=data.Email,
                    status=Status.status.Approved,
                    PhoneNumber=data.Phonenumber,
                    SecurityStamp = Guid.NewGuid().ToString(),

                };
                var result = await _userManager.CreateAsync(user, data.Password);
                if (!result.Succeeded)
                    return BadRequest("something wrong");
                await _userManager.AddToRoleAsync(user, Roles.role.User.ToString());
                return Ok("Registeration success");
            }
        }
    }
}
