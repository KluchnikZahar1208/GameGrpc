using GameGrpc.Api.Models;

namespace GameGrpc.Api.Services.IServices
{
    public interface IUserService
    {
        User CreateUser(string username);
        bool CheckUserExists(string username);
        decimal GetUserBalance(string username);
    }
}
