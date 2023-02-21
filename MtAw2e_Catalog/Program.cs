using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace MtAw2e_Catalog
{
    public class Program
    {
        private DiscordSocketClient? client;
        [STAThread]
        public static Task Main() => new Program().MainAsync();
        public async Task MainAsync()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var secretProvider = config.Providers.First();
            secretProvider.TryGet("NaomiToken", out var token);

            client = new();
            client.Log += Log;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            ApplicationConfiguration.Initialize();
            ServerInfoForm sif = new(ref client);
            Application.Run(sif);
        }
        private Task Log(LogMessage msg) // PLACEHOLDER, REPLACE BODY WITH ACTUAL LOGGING SHIT
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}