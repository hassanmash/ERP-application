using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Persistence
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection["Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.Name),
                    // Custom claim — read explicitly by the tenant-resolution middleware,
                    // not relied upon implicitly via ClaimTypes mapping.
                    new("is_platform_admin", user.IsPlatformAdmin.ToString().ToLowerInvariant())
                };

            // org_id claim is OMITTED entirely for platform admins, not set to an
            // empty/sentinel value. The tenant middleware treats "claim absent" as
            // the actual signal, matching ICurrentTenantService's nullable design.
            if (!user.IsPlatformAdmin && user.OrganizationId.HasValue)
            {
                claims.Add(new Claim("org_id", user.OrganizationId.Value.ToString()));
            }

            if (user.IsOrgAdmin)
            {
                claims.Add(new Claim("is_org_admin", "true"));
            }

            if (user.RoleId.HasValue)
            {
                claims.Add(new Claim("role_id", user.RoleId.Value.ToString()));
            }

            if (!user.Id.ToString().Equals(""))
            {
                claims.Add(new Claim("user_id", user.Id.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
