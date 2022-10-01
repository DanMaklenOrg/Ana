using System.Text.Json.Serialization;

namespace Ana.Service.DTOs;

public class SignInResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = default!;
}
