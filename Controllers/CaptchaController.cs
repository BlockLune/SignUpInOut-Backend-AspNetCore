using Microsoft.AspNetCore.Mvc;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CaptchaController : ControllerBase
  {
    private readonly CaptchaCacheService _captchaCacheService;

    public CaptchaController(CaptchaCacheService captchaCacheService)
    {
      _captchaCacheService = captchaCacheService;
    }

    // GET: api/Captcha
    [HttpGet]
    public IActionResult GenerateCaptcha()
    {
      var captchaId = _captchaCacheService.GetCaptchaId();
      return Ok(new { captchaId });
    }

    // GET: api/Captcha/:captchaId
    [HttpGet("{captchaId}")]
    public IActionResult CaptchaImage(string captchaId)
    {
      try
      {
        var imgbyte = _captchaCacheService.GetCaptcha(captchaId);
        return File(imgbyte, "image/png");
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}