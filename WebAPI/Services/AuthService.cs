using BoardCasterWebAPI.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Entities;

namespace BoardCasterWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
            private readonly IConfiguration _configuration;
            public AuthService(UserManager<ApplicationUser> userManager, 
                RoleManager<IdentityRole> roleManager, 
                IConfiguration configuration,
                SignInManager<ApplicationUser> signInManager)
            {
                this.userManager = userManager;
                this.roleManager = roleManager;
                _configuration = configuration;
                _signInManager = signInManager;

    }
    public async Task<(int, string)> Registeration(RegistrationModel model, string role)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return (0, "User already exists");
    ApplicationUser user = new ApplicationUser()
    {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username,
        Name = model.Name
    };
    //User registration with the help of default UserManager
    await userManager.SetUserNameAsync(user,model.Username);
    await userManager.SetEmailAsync(user, model.Email);

    var createUserResult = await userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
            return (0, "User creation failed! Please check user details and try again.");

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        await userManager.AddToRoleAsync(user, role);
        return (1, "User created successfully!");
    }

            public async Task<(int, string)> Login(LoginModel model)
            {
                var user = await userManager.FindByNameAsync(model.Username);
                if (user == null)
                    return (0, "Invalid username");
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                    return (0, "Invalid password");

                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                string token = GenerateToken(authClaims);
                return (1, token);
            }


            private string GenerateToken(IEnumerable<Claim> claims)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    //Expires = DateTime.UtcNow.AddHours(3),
                    Expires= DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                    SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                    Subject = new ClaimsIdentity(claims)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
}
