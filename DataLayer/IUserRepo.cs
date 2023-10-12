using Ana.DataLayer.Models;

namespace Ana.DataLayer;

public interface IUserRepo
{
    Task Create(UserDbModel user);
    Task<UserDbModel?> GetByUsername(string username);
}
