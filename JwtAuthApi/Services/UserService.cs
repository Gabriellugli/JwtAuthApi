using JwtAuthApi.Data;
using JwtAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;

            // Cria o admin padrão se não existir
            if (!_context.Users.Any(u => u.Username == "admin"))
            {
                _context.Users.Add(new User { Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = "admin" });
                _context.SaveChanges();
            }
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;
            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public bool Add(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public void Remove(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}