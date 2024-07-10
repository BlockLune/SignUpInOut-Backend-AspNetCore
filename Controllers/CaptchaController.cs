using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CaptchaController : ControllerBase
  {
    private readonly CaptchaCache _captchaCache;

    public CaptchaController(CaptchaCache captchaCache)
    {
      _captchaCache = captchaCache;
    }

    // GET: api/Captcha
    [HttpGet]
    public IActionResult GenerateCaptcha()
    {
      var captchaId = _captchaCache.GetCaptchaId();
      return Ok(new { captchaId });
    }

    // GET: api/Captcha/:captchaId
    [HttpGet("{captchaId}")]
    public IActionResult CaptchaImage(string captchaId)
    {
      try
      {
        var imgbyte = _captchaCache.GetCaptcha(captchaId);
        return File(imgbyte, "image/png");
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}