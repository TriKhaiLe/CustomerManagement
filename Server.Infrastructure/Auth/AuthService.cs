using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagement.Shared.Auth;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Auth;
using Server.Application.Common.Exceptions;

namespace Server.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IValidator<LoginRequest> _validator;

    public AuthService(IConfiguration configuration, IValidator<LoginRequest> validator)
    {
        _configuration = configuration;
        _validator = validator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new AppValidationException(validationResult.Errors);
        }

        var expectedUsername = _configuration["AdminUser:Username"] ?? "admin";
        var expectedPassword = _configuration["AdminUser:Password"] ?? "Admin@123";

        if (!string.Equals(request.Username, expectedUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
        {
            throw new UnauthorizedAppException("Invalid username or password.");
        }

        var jwtSettings = _configuration.GetSection("Jwt");
        var secret = jwtSettings["Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "CustomerManagement.Api";
        var audience = jwtSettings["Audience"] ?? "CustomerManagement.Client";
        var expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return await Task.FromResult(new LoginResponse
        {
            Username = request.Username,
            AccessToken = tokenString,
            ExpiresAtUtc = expiresAtUtc
        });
    }
}
