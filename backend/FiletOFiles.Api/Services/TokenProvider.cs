using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FiletOFiles.Api.Services;

public sealed class TokenProvider
{
    private readonly AuthOptions _jwtAuthOptions;

    public TokenProvider(IOptions<AuthOptions> jwtAuthOptions)
    {
        _jwtAuthOptions = jwtAuthOptions.Value;
    }

    public AccessTokenDto Create(TokenRequest tokenRequest)
    {
        return new AccessTokenDto(GenerateAccessToken(tokenRequest), GenerateRefreshToken());
    }

    private string GenerateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            new Claim(JwtRegisteredClaimNames.Email, tokenRequest.Email),
            .. tokenRequest.Roles.Select(role => new Claim(JwtCustomClaimNames.Role, role)),
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtAuthOptions.Issuer,
            Audience = _jwtAuthOptions.Audience,
        };

        var handler = new JsonWebTokenHandler();

        var accessToken = handler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    private string GenerateRefreshToken()
    {
        byte[] ulidBytes = Encoding.UTF8.GetBytes(Ulid.NewUlid().ToString());
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String([.. ulidBytes, .. randomBytes]);
    }
}
