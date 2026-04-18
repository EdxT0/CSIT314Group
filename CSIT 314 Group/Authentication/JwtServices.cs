using CSIT_314_Group.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CSIT_314_Group.Authentication
{
    public class JwtServices
    {
        //private readonly JwtOptions _jwtOptions;
        //public JwtServices(JwtOptions jwtOptions){
        //    _jwtOptions = jwtOptions;
        //}

        //public string GenerateJwtToken(UserAccount userAccount)
        //{
        //    if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
        //    {
        //        throw new InvalidOperationException("JWT key is missing.");
        //    }
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier , userAccount.id.ToString()),
        //        new Claim(ClaimTypes.Name, userAccount.Name),
        //        new Claim(ClaimTypes.Email, userAccount.Email),
        //        new Claim(ClaimTypes.Role, userAccount.Profile)
        //    };

        //    var signingKey = new SymmetricSecurityKey(
        //    Convert.FromBase64String(_jwtOptions.Key));

        //    var credentials = new SigningCredentials(
        //        signingKey,
        //        SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _jwtOptions.Issuer,
        //        audience: _jwtOptions.Audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(1),
        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
