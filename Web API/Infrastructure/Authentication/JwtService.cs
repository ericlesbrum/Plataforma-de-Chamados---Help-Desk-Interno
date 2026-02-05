using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web_API.Application.Interfaces;

namespace Web_API.Infrastructure.Authentication;

public class JwtService : IJwtService
{
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
    {
        var jwtSettings = _config.GetSection("JWT");
        if (!jwtSettings.Exists())
            throw new InvalidOperationException("JWT configuration section is missing.");

        var keyString = jwtSettings["Key"];
        if (string.IsNullOrWhiteSpace(keyString))
            throw new InvalidOperationException("JWT Key is missing.");

        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var validityMinutes = jwtSettings.GetValue<int?>("TokenValidityInMinutes") ?? 30;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(validityMinutes),
            signingCredentials: signingCredentials
        );

    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[128];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token must be provided", nameof(token));

        var jwtSection = _config.GetSection("JWT");
        if (!jwtSection.Exists())
            throw new InvalidOperationException("JWT configuration section is missing.");

        var key = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT Key is missing.");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
           !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
           StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public TokenValidationParameters GetValidationParameters(IConfiguration config)
    {
        var jwt = config.GetSection("JWT");

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    }
}
