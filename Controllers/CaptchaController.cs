using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;
using Hei.Captcha;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CaptchaController : ControllerBase
  {
    private readonly SecurityCodeHelper _securityCode;

    public CaptchaController(SecurityCodeHelper securityCodeHelper)
    {
      _securityCode = securityCodeHelper;
    }

    // GET: api/Captcha
    [HttpGet]
    public IActionResult GenerateCaptcha()
    {
      var captchaId = _securityCode.GetRandomEnDigitalText(4);
      return Ok(new { captchaId });
    }

    // GET: api/Captcha/:captchaId
    [HttpGet("{captchaId}")]
    public IActionResult CaptchaImage(string captchaId)
    {
      var imgbyte = _securityCode.GetEnDigitalCodeByte(captchaId);
      return File(imgbyte, "image/png");
    }
  }
}