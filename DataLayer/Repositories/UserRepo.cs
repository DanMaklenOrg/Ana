using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Ana.DataLayer.Models;

namespace Ana.DataLayer.Repositories;

public class UserRepo : IUserRepo
{
    private const string TableName = "AnaTable";
    private const string UsernameIndex = "username-index";

    private readonly IAmazonDynamoDB _db;

    public UserRepo(IAmazonDynamoDB db)
    {
        _db = db;
    }

    public async Task Create(UserDbModel user)
    {
        var serializedItem = BaseDbModel.SetKeysAndSerialize(user);
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = serializedItem,
        };
        await _db.PutItemAsync(request);
    }

    public async Task<UserDbModel?> GetByUsername(string username)
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            KeyConditionExpression = "username = :usernameValue",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":usernameValue", new AttributeValue(username) },
            },
            IndexName = UsernameIndex,
        };
        var response = await _db.QueryAsync(request);
        var userList = response.Items.ConvertAll(BaseDbModel.Deserialize<UserDbModel>);

        if (userList.Count > 1) throw new Exception("Too many users with the same username");

        return userList.FirstOrDefault();
    }
}
