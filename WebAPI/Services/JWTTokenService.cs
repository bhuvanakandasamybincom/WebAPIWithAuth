using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoardCasterWebAPI.Services
{
    public class JWTTokenService
    {
        public string GenerateJwtToken()
        {
            //ConfigurationHelper.GetAppSetting("DB");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalModel.SecKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: GlobalModel.SecIssuer,
                audience: GlobalModel.SecAudience,
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
