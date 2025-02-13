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
        private static string? _countriesPath;
        private readonly List<string> _englishCountries;
        private readonly List<string> _spanishCountries;
        public static Task Main(string[] args) => new Program().MainAsync();

        public Program()
        {
            var config = new DiscordSocketConfig
            {
                // Intents are used to specify the events that the bot will receive, required for the bot to function 
                // (I broke my head trying to figure out why it wasn't working until I found this)
                GatewayIntents = GatewayIntents.Guilds |
                                GatewayIntents.GuildMessages |
                                GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);

            // Methods suscriptions to the events of the client
            _client.Ready += Ready;
            _client.MessageReceived += MessageReceivedAsync;

            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();

            _englishCountries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText("CountriesJsons/en-countries.json"))?["countries"] ?? new List<string>();
            _spanishCountries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText("CountriesJsons/es-countries.json"))?["countries"] ?? new List<string>();
        }

        public async Task MainAsync()
        {
            Console.WriteLine("Connecting to Discord...\n");

            _imageDirectory = AppContext.BaseDirectory + "/Images";
            _countriesPath = AppContext.BaseDirectory + "/CountriesJsons";

            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task Ready()
        {
            Console.WriteLine($"Connected as -> {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} :)");
            Console.Write($"Bot is running on servers:");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var guild in _client.Guilds)
            {
                Console.Write($" | {guild.Name}");
            }
            Console.WriteLine("");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            List<string> _imagesPaths = new List<string>();

            string upperCaseMessage = message.Content.ToUpper();

            // Avoid responding to your own messages
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            foreach (var country in _englishCountries)
            {
                if (upperCaseMessage.Contains(country))
                {
                    string imagePath = $"{_imageDirectory}/{country}.jpg";

                    if (System.IO.File.Exists(imagePath))
                    {
                        _imagesPaths.Add(imagePath);
                    }
                }
            }
            if (_imagesPaths.Count > 0)
            {
                List<FileAttachment> attachments = new List<FileAttachment>();

                foreach (var imagePath in _imagesPaths)
                {
                    var filename = Path.GetFileName(imagePath);
                    var filestream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    var stream = new FileAttachment(filestream, filename);
                    attachments.Add(stream);
                }

                await message.Channel.SendFilesAsync(attachments, messageReference: new MessageReference(message.Id));
                Console.WriteLine($"Message received: '{message.Content}' from {message.Author.Username} in #{message.Channel.Name} (Guild: {(message.Channel as SocketGuildChannel)?.Guild.Name ?? "DM"})");
                //The (Guild: {(message.Channel as SocketGuildChannel)?.Guild.Name ?? "DM"}) part is interesting, it tires to cast the channel as a SocketGuildChannel, if it fails, it means is not a guild channel, so it's an MD, but if it works, it means it's a guild channel, so it's a server.

                foreach (var attachment in attachments)
                {
                    await attachment.Stream.DisposeAsync();
                }
            }
        }
    }
}