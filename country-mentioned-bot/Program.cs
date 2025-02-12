
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;


namespace Country_Mentioned_Bot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private static string? _imageDirectory;
        private readonly List<string> _countries;

        public static Task Main(string[] args) => new Program().MainAsync();

        public Program()
        {
            _client = new DiscordSocketClient();
            //Hook into the client ready event, to handle the bot ready event
            _client.Ready += Ready;

            //Hook into the message received event, to handle the country mentioned event
            _client.MessageReceived += MessageReceivedAsync;

            //Create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();

            _countries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText("countries.json"))?["countries"] ?? new List<string>();
        }

        public async Task MainAsync()
        {
            _imageDirectory = GetImageDirectoryPath();
            //This is where we get the Token value from the configuration file
            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task Ready()
        {
            Console.WriteLine($"Connected as -> {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} :)");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            string upperCaseMessage = message.Content.ToUpper();
            

            // Avoid responding to your own messages
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            Console.WriteLine($"Message received: '{message.Content}' from {message.Author.Username}");

            foreach (var country in _countries)
            {
                if (upperCaseMessage.Contains(country))
                {
                    string imagePath = $"{_imageDirectory}/Images/{country}.jpg";
                    Console.WriteLine(imagePath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        await message.Channel.SendFileAsync(imagePath, messageReference: new MessageReference(message.Id));
                    }
                    return;
                }
            }
        }

        private static string GetImageDirectoryPath()
        {
            Console.WriteLine(AppContext.BaseDirectory);
            return AppContext.BaseDirectory;
        }

    }
}