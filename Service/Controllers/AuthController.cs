using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Ana.DataLayer;
using Ana.DataLayer.Model;
using Ana.Service.DTOs;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ana.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AnaDbContext db;
    private readonly byte[] key;

    public AuthController(AnaDbContext db, IOptions<ServiceConfig> config)
    {
        this.db = db;
        this.key = Convert.FromBase64String(config.Value.AuthKeyBase64);
    }

    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponseDto>> SignIn(SingInRequestDto requestDto)
    {
        UserDbModel? user = await this.db.Users.SingleOrDefaultAsync(user => user.Username == requestDto.Username);

        if (user is null || HashPassword(requestDto.Password, user.HashSalt) != user.HashedPassword)
            return this.Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor()
        {
            Claims = new Dictionary<string, object>
            {
                { "sub", user.Guid.ToString("N") },
                { "name", user.Username },
            },
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(this.key), SecurityAlgorithms.HmacSha256),
        });

        return new SignInResponseDto
        {
            Token = tokenHandler.WriteToken(token)
        };
    }

    [HttpPost("signup")]
    public async Task SingUp(SignupRequestDto requestDto)
    {
        string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        string hashedPassword = HashPassword(requestDto.Password, salt);

        var user = new UserDbModel
        {
            Username = requestDto.Username,
            HashedPassword = hashedPassword,
            HashSalt = salt,
        };

        await this.db.Users.AddAsync(user);
        await this.db.SaveChangesAsync();
    }

    private static string HashPassword(string password, string salt)
    {
        var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = Convert.FromBase64String(salt),
            Iterations = 2,
            MemorySize = 15 * 1024,
            DegreeOfParallelism = 1,
        };
        return Convert.ToBase64String(argon.GetBytes(128));
    }
}
