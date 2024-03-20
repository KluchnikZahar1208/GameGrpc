using GameGrpc.Api.Models;
using GameGrpc.Api.Services.IServices;

namespace GameGrpc.Api.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext.DataContext _context;

        public UserService(DataContext.DataContext context)
        {
            _context = context;
        }

        public User CreateUser(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                user = new User() { Username = username, Balance = 100 };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            return user;
        }

        public bool CheckUserExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public decimal GetUserBalance(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user?.Balance ?? 0;
        }
    }
}
