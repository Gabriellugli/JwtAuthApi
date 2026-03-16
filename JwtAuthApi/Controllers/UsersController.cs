using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtAuthApi.Models;
using JwtAuthApi.Services;

namespace JwtAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAll()
                .Select(u => new UserDto { Username = u.Username, Role = u.Role });
            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            var created = _userService.Add(user);

            if (!created)
                return Conflict("Usuário já existe!");

            return Ok(_userService.GetAll());
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        public IActionResult DeleteUser(string username)
        {
            _userService.Remove(username);
            return Ok(_userService.GetAll());
        }
    }
}