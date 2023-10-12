using Amazon.DynamoDBv2.DataModel;

namespace Ana.DataLayer;

[DynamoDBTable("AnaTable")]
public class UserDbModel
{
    [DynamoDBHashKey("pk")]
    public string PartitionKey { get; set; } = default!;

    [DynamoDBRangeKey("sk")]
    public string SortKey { get; set; } = default!;

    [DynamoDBProperty("id")]
    public Guid Id { get; set; }

    [DynamoDBProperty("username")]
    public string Username { get; set; } = default!;

    [DynamoDBProperty("hashedPassword")]
    public string HashedPassword { get; set; } = default!;

    [DynamoDBProperty("salt")]
    public string Salt { get; set; } = default!;
}
