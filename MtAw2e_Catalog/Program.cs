using Discord;
using Discord.WebSocket;

namespace MtAw2e_Catalog
{
    public class Program
    {
        private DiscordSocketClient? client;
        [STAThread]
        public static Task Main() => new Program().MainAsync();
        public async Task MainAsync()
        {
            client = new();
            client.Log += Log;
            // make this secret eventually
            var token = "";
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