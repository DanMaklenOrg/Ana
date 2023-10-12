using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Ana.DataLayer;
using Ana.DataLayer.Models;
using Ana.Service.DTOs;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ana.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepo userRepo;
    private readonly byte[] key;

    public AuthController(IUserRepo userRepo, IOptions<ServiceConfig> config)
    {
        this.userRepo = userRepo;
        this.key = Convert.FromBase64String(config.Value.AuthJwtKeyBase64);
    }

    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponseDto>> SignIn(SingInRequestDto requestDto)
    {
        // this.userRepo
        UserDbModel? user = await this.userRepo.GetByUsername(requestDto.Username);

        if (user is null || HashPassword(requestDto.Password, user.Salt) != user.HashedPassword)
            return this.Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor()
        {
            Claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, user.Id.ToString("N") },
                { JwtRegisteredClaimNames.Name, user.Username },
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
            Id = Guid.NewGuid(),
            Username = requestDto.Username,
            HashedPassword = hashedPassword,
            Salt = salt,
        };

        // This is not a transaction which can lead into issues with large scale. Should fix this.
        if (await this.userRepo.GetByUsername(requestDto.Username) is not null)
            throw new ArgumentException("User with this username already exist");

        await this.userRepo.Create(user);
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
