using System.Text.Json.Serialization;

namespace Ana.Service.DTOs;

public class SignupRequestDto
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;
}
