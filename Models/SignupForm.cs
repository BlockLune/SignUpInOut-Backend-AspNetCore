public class SignupForm
{
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string CaptchaId { get; set; } = null!;
  public string CaptchaAnswer { get; set; } = null!;
}