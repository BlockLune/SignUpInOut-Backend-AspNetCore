using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.SignalR;
using SignUpInOut_Backend_AspNetCore.Hubs;

namespace SignUpInOut_Backend_AspNetCore.Models
{
    public class DiscordBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly IHubContext<ChatHub> _hubContext;


        public DiscordBotService(DiscordSocketClient client, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _client = client;
            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;

            _token = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_BOT_TOKEN") ??
                throw new InvalidOperationException(
                    "Discord token not found"
                );
        }

        public async Task InitializeAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage messageParam)
        {
            Console.WriteLine(messageParam.Content);

            if (messageParam.Author.IsBot) return;
            if (messageParam.Content.Contains("您好"))
            {
                await _hubContext.Clients.All.SendAsync("messageReceived", "DiscordBot", "您也好呀");
                await messageParam.Channel.SendMessageAsync("您也好呀");
            }
        }
    }
}
