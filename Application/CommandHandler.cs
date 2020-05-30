using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using DataLayer.Entities;
using Application.Services;

namespace Application
{
    public class CommandHandler
    {
        #region Dependency Injection
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;
        private readonly ConfigService _configService;
        private readonly Config _config;

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services, ConfigService configService, Config config)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
            _configService = configService;
            _config = config;
        }
        #endregion Dependency Injection

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync;
            _client.Ready += StatusAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            if (socketMessage.Author.IsBot) return;

            var userMessage = socketMessage as SocketUserMessage;
            if (userMessage is null)
                return;

            if (!userMessage.HasStringPrefix(_config.Prefix, ref argPos))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _cmdService.ExecuteAsync(context, argPos, _services);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }

        public async Task StatusAsync()
        {
            await _client.SetGameAsync("Moldtelecom | !play");
        }
    }
}
