using Microsoft.AspNetCore.Mvc;
using SignUpInOut_Backend_AspNetCore.Services;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CaptchaCacheService _captchaCacheService;
        private readonly UserService _userService;

        public UsersController(CaptchaCacheService captchaCache, UserService userService)
        {
            _captchaCacheService = captchaCache;
            _userService = userService;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser([FromBody] SignupForm signupForm)
        {
            var email = signupForm.Email;
            var password = signupForm.Password;
            var captchaId = signupForm.CaptchaId;
            var captchaAnswer = signupForm.CaptchaAnswer;

            if (!_captchaCacheService.VerifyCaptcha(captchaId, captchaAnswer))
            {
                return BadRequest("Invalid captcha");
            }

            var user = await _userService.SignUpAsync(email, password);
            if (user == null)
            {
                return BadRequest("User already exists");
            }

            return CreatedAtAction(nameof(PostUser), UserService.UserToDTO(user));
        }

        // POST: api/Users/Signin
        [HttpPost("Signin")]
        public async Task<ActionResult<UserDTO>> Signin([FromBody] Credentials credentials)
        {
            var user = await _userService.SignInAsync(credentials.Email, credentials.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            return UserService.UserToDTO(user);
        }
    }
}
