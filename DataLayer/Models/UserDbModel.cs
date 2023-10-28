using System.Text.Json.Serialization;

namespace Ana.DataLayer.Models;

public class UserDbModel : BaseDbModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;

    [JsonPropertyName("hashedPassword")]
    public string HashedPassword { get; set; } = default!;

    [JsonPropertyName("salt")]
    public string Salt { get; set; } = default!;

    public override void SetKeys()
    {
        PartitionKey = Id.ToString();
        SortKey = Username;
    }
}
