﻿using Discord;
using Discord.WebSocket;

namespace SignUpInOut_Backend_AspNetCore.Models
{
    public class DiscordBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly string _clientId;
        private readonly string _clientSecret;


        public DiscordBotService(DiscordSocketClient client)
        {
            _client = client;
            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;

            _token = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_BOT_TOKEN") ??
                throw new InvalidOperationException(
                    "Discord token not found"
                );
            _clientId = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_CLIENT_ID") ??
                throw new InvalidOperationException(
                    "Discord client ID not found"
                );
            _clientSecret = Environment
                .GetEnvironmentVariable("SIGNUPINOUT_DISCORD_CLIENT_SECRET") ??
                throw new InvalidOperationException(
                    "Discord client secret not found"
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
            if (messageParam.Author.IsBot) return;
            if (messageParam.Content == "您好")
            {
                await messageParam.Channel.SendMessageAsync("您也好呀");
            }
        }
    }
}
