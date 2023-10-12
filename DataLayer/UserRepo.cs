using Amazon.DynamoDBv2.DataModel;

namespace Ana.DataLayer;

public class UserRepo : IUserRepo
{
    private readonly IDynamoDBContext db;

    public UserRepo(IDynamoDBContext db)
    {
        this.db = db;
    }

    public async Task Create(UserDbModel user)
    {
        user.Id = Guid.NewGuid();
        user.PartitionKey = user.Id.ToString();
        user.SortKey = user.PartitionKey;
        await this.db.SaveAsync(user);
    }

    public async Task<UserDbModel?> GetByUsername(string username)
    {
        var usersList = await this.db.QueryAsync<UserDbModel>(username, new DynamoDBOperationConfig
        {
            IndexName = "username-index",
        }).GetRemainingAsync();

        if (usersList.Count > 1) throw new Exception("Too many users with the same username");

        return usersList.FirstOrDefault();
    }
}
