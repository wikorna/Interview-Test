using Interview_Test.Infrastructure.DTOs;
using Interview_Test.Models;

namespace Interview_Test.Infrastructure.Interfaces;

public interface IUserRepository
{
    //dynamic GetUserById(string id);
    Task<UserDetailDto?> GetByUserId(string userId, CancellationToken token = default);
    int CreateUser(UserModel user);
}