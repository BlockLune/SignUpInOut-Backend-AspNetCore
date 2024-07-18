using Hei.Captcha;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

public class CaptchaCacheService
{
  private readonly SecurityCodeHelper _securityCode;
  private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

  public CaptchaCacheService(SecurityCodeHelper securityCodeHelper)
  {
    _securityCode = securityCodeHelper;
  }

  public string GetCaptchaId()
  {
    var captchaAnswer = _securityCode.GetRandomEnDigitalText(4);
    var captchaId = ComputeSha256Hash(captchaAnswer);
    _cache.Set(captchaId, captchaAnswer, TimeSpan.FromMinutes(5));
    return captchaId;
  }

  public byte[] GetCaptcha(string captchaId)
  {
    var captchaAnswer = _cache.Get<string>(captchaId);
    Console.WriteLine($"CaptchaAnswer: {captchaAnswer}");
    if (captchaAnswer == null) throw new Exception("CaptchaId is out of time.");
    return _securityCode.GetEnDigitalCodeByte(captchaAnswer);
  }

  public bool VerifyCaptcha(string captchaId, string captchaAnswer)
  {
    var captchaAnswerExpected = _cache.Get<string>(captchaId);
    if (captchaAnswerExpected == null) return false;
    return captchaAnswerExpected == captchaAnswer;
  }

  private static string ComputeSha256Hash(string rawData)
  {
    using (SHA256 sha256Hash = SHA256.Create())
    {
      byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
      StringBuilder builder = new();
      for (int i = 0; i < bytes.Length; i++)
      {
        builder.Append(bytes[i].ToString("x2"));
      }
      return builder.ToString().Substring(0, 8);
    }
  }
}