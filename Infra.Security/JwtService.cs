using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Domain.Entities.Dtos;
using Infra.Security.Constants;
using Infra.Utils.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Security;

public class JwtService(IOptionsSnapshot<AppSettings> appSettings) : IJwtService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public string CreateToken(UserDto user)
    {
        var secret = _appSettings.Jwt?.Secret!;
        var expirationTime = Convert.ToInt32(_appSettings.Jwt?.SessionExpirationHours);
        
        var signinKey = new SigningCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(secret)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = GetClaims(user),
            Expires = DateTime.UtcNow.AddHours(expirationTime),
            SigningCredentials = signinKey
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);

        return tokenHandler.WriteToken(token);
    }
    
    private static ClaimsIdentity GetClaims(UserDto user)
    {
        var identity = new ClaimsIdentity("JWT");
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        identity.AddClaim(new Claim(JwtClaims.CAIM_USER_PROFILE, JsonSerializer.Serialize(user, serializeOptions)));
        identity.AddClaim(new Claim(JwtClaims.CLAIM_SCOPES, user.Role.ToString()));
        return identity;
    }
}