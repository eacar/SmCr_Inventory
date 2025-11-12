using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inv.Application.Contracts.Security;
using Inv.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inv.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly AuthSettings _authSettings;

        public TokenService(
            IOptions<AuthSettings> authOptions)
        {
            _authSettings = authOptions.Value;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string CreateToken(List<Claim> claims)
        {
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_authSettings.Expires),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _authSettings.Issuer,
                NotBefore = DateTime.UtcNow
            };
            var securityToken = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return _jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}