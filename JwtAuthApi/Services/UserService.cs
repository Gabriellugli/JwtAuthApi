using JwtAuthApi.Models;

namespace JwtAuthApi.Services
{
    public class UserService
    {
        private static List<User> _users = new List<User>
        {
            new User { Username = "admin", Password = "123", Role = "admin" }
        };

        public User? GetByUsernameAndPassword(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public List<User> GetAll()
        {
            return _users;
        }

        public bool Add(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return false;

            _users.Add(user);
            return true;
        }

        public void Remove(string username)
        {
            _users.RemoveAll(u => u.Username == username);
        }
    }
}