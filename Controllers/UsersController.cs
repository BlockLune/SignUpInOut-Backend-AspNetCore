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
        private readonly CaptchaCache _captchaCache;

        public UsersController(SignupinoutDbContext context, CaptchaCache captchaCache)
        {
            _context = context;
            _captchaCache = captchaCache;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser([FromBody] SignupForm signupForm)
        {
            var email = signupForm.Email;
            var password = signupForm.Password;
            var captchaId = signupForm.CaptchaId;
            var captchaAnswer = signupForm.CaptchaAnswer;
            if (!_captchaCache.VerifyCaptcha(captchaId, captchaAnswer))
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
