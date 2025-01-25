using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment3_Backend.Models;
using Assignment3_Backend.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment3_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
     
        private readonly IRepository _repository;
        private readonly IConfiguration _config;
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsPrincipalFactory;
        private readonly UserManager<AppUser> _userConfig;

        public AuthenticationController(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsPrincipalFactory, IConfiguration configuration, IRepository repository)
        {
            _userConfig = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _config = configuration;
            _repository = repository;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Register(UserViewModel userVM)
        {
            //check if an user in the db has the email registered already
            var user = await _userConfig.FindByIdAsync(userVM.emailaddress);
            if (user == null)
            {
                //add the new user's username/email if the user doesnt exist
                user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userVM.emailaddress,
                    Email = userVM.emailaddress,
                
                };
                //add the users password
                var result = await _userConfig.CreateAsync(user, userVM.password);

                if (result.Errors.Count() > 0) return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
            else
            {
                return Forbid("This user account already exists.");
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(UserViewModel userVm)
        {
            var user = await _userConfig.FindByNameAsync(userVm.emailaddress);

            if (user != null && await _userConfig.CheckPasswordAsync(user, userVm.password))
            {
                try
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
                    return GenJWTToken(user);
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
                }
            }
            else
            {
                return NotFound("Does not exist");
            }
            //var loggedInUser = new UserViewModel{ emailaddress = userVm.emailaddress, password = userVm.password};
            //return Ok(loggedInUser);
        }

        [HttpGet]
        private ActionResult GenJWTToken(AppUser user)
        {
            // Create JWT Token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                claims,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddHours(3)
            );

            return Created("", new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                user = user.Email
            });
        }

    }
}
