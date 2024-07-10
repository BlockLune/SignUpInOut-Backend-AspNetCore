using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignupinoutDbContext _context;

        public UsersController(SignupinoutDbContext context)
        {
            _context = context;
        }

        /*
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            UserDTO[] users = await _context.Users.Select(user =>
                new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email
                }).ToArrayAsync();
            return users;
        }
        */

        /*
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email
            };
        }
        */

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser([FromBody] SignupForm signupForm)
        {
            var email = signupForm.Email;
            var password = signupForm.Password;
            var captchaId = signupForm.CaptchaId;
            var captchaAnswer = signupForm.CaptchaAnswer;
            // !! IS THIS CORRECT??? I'M NOT SURE.
            if (!captchaId.Equals(captchaAnswer))
            {
                return BadRequest("Invalid captcha");
            }

            // bcrypt
            password = BCrypt.Net.BCrypt.HashPassword(password, 10);

            _context.Users.Add(
            new User
            {
                Email = email,
                Password = password
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostUser), new UserDTO
            {
                Id = GetUserId(email)!.Value,
                Email = email
            });
        }

        // POST: api/Users/Signin
        [HttpPost("Signin")]
        public async Task<ActionResult<UserDTO>> Signin([FromBody] Credentials credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == credentials.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password))
            {
                return Unauthorized();
            }

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private int? GetUserId(string email)
        {
            return _context.Users.FirstOrDefault(e => e.Email == email)?.Id;
        }
    }
}
