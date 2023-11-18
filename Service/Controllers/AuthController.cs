using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Ana.DataLayer.Models;
using Ana.DataLayer.Repositories;
using Ana.Service.DTOs;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ana.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly byte[] _key;

    public AuthController(IUserRepo userRepo, IOptions<ServiceConfig> config)
    {
        _userRepo = userRepo;
        _key = Convert.FromBase64String(config.Value.AuthJwtKeyBase64);
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<SignInResponseDto>> SignIn(SingInRequestDto requestDto)
    {
        UserDbModel? user = await _userRepo.GetByUsername(requestDto.Username);

        if (user is null || HashPassword(requestDto.Password, user.Salt) != user.HashedPassword)
            return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor()
        {
            Claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, user.Id.ToString("N") },
                { JwtRegisteredClaimNames.Name, user.Username },
            },
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256),
        });

        return new SignInResponseDto
        {
            Token = tokenHandler.WriteToken(token)
        };
    }

    [HttpPost("sign-up")]
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
        if (await _userRepo.GetByUsername(requestDto.Username) is not null)
            throw new ArgumentException("User with this username already exist");

        await _userRepo.Create(user);
    }

    private static string HashPassword(string password, string salt)
    {
        var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = Convert.FromBase64String(salt),
            Iterations = 2,
            MemorySize = 19 * 1024,
            DegreeOfParallelism = 1,
        };
        return Convert.ToBase64String(argon.GetBytes(128));
    }
}
