using System.Security.Cryptography;
using IdentityAccess.Api.Services;
using IdentityAccess.Domain;
using IdentityAccess.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AppDbContext db,
    IPasswordHasher<User> hasher,
    JwtTokenService jwt,
    IConfiguration cfg) : ControllerBase
{
    public record RegisterRequest(string Username, string Password);
    public record LoginRequest(string Username, string Password);
    public record TokenResponse(string AccessToken, string RefreshToken);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (await db.Users.AnyAsync(u => u.Username == req.Username))
            return Conflict("Username já existe.");

        var user = new User { Username = req.Username, PasswordHash = "" };
        user.PasswordHash = hasher.HashPassword(user, req.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { user.Id }, new { user.Id, user.Username });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Username == req.Username);
        if (user is null) return Unauthorized();

        var ok = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password) != PasswordVerificationResult.Failed;
        if (!ok) return Unauthorized();

        var access = jwt.GenerateAccessToken(user);
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refresh,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(int.Parse(cfg["Jwt:RefreshTokenDays"]!))
        });
        await db.SaveChangesAsync();

        return Ok(new TokenResponse(access, refresh));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var rt = await db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);
        if (rt is null || !rt.IsActive) return Unauthorized();

        var user = await db.Users.FindAsync(rt.UserId);
        if (user is null) return Unauthorized();

        // Rotaciona
        rt.RevokedAtUtc = DateTime.UtcNow;
        var newRefresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefresh,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(int.Parse(cfg["Jwt:RefreshTokenDays"]!)),
            ReplacedByToken = rt.Token
        });

        await db.SaveChangesAsync();
        var access = jwt.GenerateAccessToken(user);
        return Ok(new TokenResponse(access, newRefresh));
    }

    [HttpPost("logout")]
    [Authorize] // opcional
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        var rt = await db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);
        if (rt is null || !rt.IsActive) return Ok(); // idempotente
        rt.RevokedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok();
    }
}