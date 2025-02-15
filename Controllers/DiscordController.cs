using Discord.Rest;
using Discord;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using SignUpInOut_Backend_AspNetCore.Services;

namespace SignUpInOut_Backend_AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserService _userService;
        private readonly string _clientId;
        private readonly string _clientSecret;
        public DiscordController(IHttpClientFactory httpClientFactory, UserService userService)
        {
            _httpClientFactory = httpClientFactory;
            _userService = userService;
            _clientId = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_CLIENT_ID") ??
                throw new InvalidOperationException(
                    "Discord Client ID Not Found"
                );
            _clientSecret = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_CLIENT_SECRET") ??
                throw new InvalidOperationException(
                    "Discord Client Secret Not Found"
                );
        }

        // POST api/Discord/signin
        [HttpPost("signin")]
        public async Task<ActionResult<Credentials>> Signin([FromQuery] string code)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/v10/oauth2/token");

                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", "http://localhost:3000/signin")
                });

                request.Content = content;

                var authenticationString = $"{_clientId}:{_clientSecret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                if (tokenResponse != null)
                {
                    Console.WriteLine("Bearer Token: " + tokenResponse.AccessToken);

                    await using var discordRestClient = new DiscordRestClient();
                    await discordRestClient.LoginAsync(TokenType.Bearer, tokenResponse.AccessToken);

                    var email = discordRestClient.CurrentUser.Email;
                    Console.WriteLine("Discord User Email: ", email);

                    if (await _userService.UserExistsAsync(email))
                    {
                        return Ok("User with email " + email + " already exists");
                    }
                    else
                    {
                        var randomPassword = Guid.NewGuid().ToString();
                        var user = await _userService.SignUpAsync(email, randomPassword);
                        if (user != null)
                        {
                            return Created("New user created", new Credentials
                            {
                                Email = user.Email,
                                Password = randomPassword
                            });
                        }
                        else
                        {
                            return StatusCode(500, "Failed to create user");
                        }
                    }

                }
                return Unauthorized();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return Unauthorized();
            }
        }
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;
}