using GrpcServer.Protos;

namespace GrpcServer.Services.IServices
{
    public interface IUserService
    {
        User CreateUser(string username);
        bool CheckUserExists(string username);
        double GetUserBalance(string username);
    }
}
