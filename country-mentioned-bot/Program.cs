using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Country_Mentioned_Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//~ Code made by Civer_mau


namespace Country_Mentioned_Bot
{
    /// <summary>
    /// Main bot class handling Discord connection and message processing
    /// </summary>
    class Program
    {
        //^ ---------------------------------------- Discord client
        private readonly IConfiguration _config; // Configuration from config.json
        private InteractionService _commands;     // Handles slash commands
        private DiscordSocketClient _client;      // Main Discord client connection
        private ulong _testGuildId;               // ID for testing server (if used)


        //^ ---------------------------------------- Directories
        private static string? _imageDirectory;   // Path to country flag images
        private static string? _countriesPath;   // Path to country list JSON files

        //^ ---------------------------------------- Countries lists
        private readonly List<string> _englishCountries; // Country names in English
        private readonly List<string> _spanishCountries; // Country names in Spanish


        //^ ---------------------------------------- Main
        public static Task Main(string[] args) => new Program().MainAsync();

        /// <summary>
        /// Program constructor - initializes core components
        /// </summary>
        public Program()
        {
            // Configure Discord client with required intents:
            // - Guilds: Server information
            // - GuildMessages: Messages in servers
            // - MessageContent: Access to message content (required for reading messages)
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds |
                                GatewayIntents.GuildMessages |
                                GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);

            // Load configuration from config.json file
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();

            // Load country lists from JSON files
            _englishCountries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText("CountriesJsons/en-countries.json"))?["countries"] ?? new List<string>();
            _spanishCountries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText("CountriesJsons/es-countries.json"))?["countries"] ?? new List<string>();
            _commands = new InteractionService(_client);
        }

        /// <summary>
        /// Main bot entry point
        /// </summary>
        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var commands = services.GetRequiredService<InteractionService>();

                _client = client;
                _commands = commands;

                // Set up file paths
                _imageDirectory = AppContext.BaseDirectory + "/Images";
                _countriesPath = AppContext.BaseDirectory + "/CountriesJsons";

                // Register event handlers
                _client.Ready += Ready;                     // When bot connects successfully
                _client.MessageReceived += MessageReceivedAsync; // When any message is received

                Console.WriteLine("Connecting to Discord...\n");

                // Start connection to Discord
                _testGuildId = ulong.Parse(_config["TestGuildId"] ?? "0");

                await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                await _client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                // Keep the bot running indefinitely
                await Task.Delay(-1);
            }
        }

        /// <summary>
        /// Handles the Ready event - called when bot successfully connects
        /// </summary>
        private async Task Ready()
        {
            if (IsDebug())
            {
                System.Console.WriteLine($"In debug mode, adding commands to {_testGuildId}...");
                await _commands.RegisterCommandsToGuildAsync(_testGuildId);
            }
            else
            {
                await _commands.RegisterCommandsGloballyAsync(true);
            }

            Console.WriteLine($"Connected as -> {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} :)");
            Console.Write($"Bot is running on servers:");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var guild in _client.Guilds)
            {
                Console.Write($" | {guild.Name}");
            }
            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// Configures dependency injection services
        /// </summary>
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)  // Configuration
                .AddSingleton<DiscordSocketClient>()  // Discord client
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))  // Command system
                .AddSingleton<CommandHandler>()  // Our custom command handler
                .BuildServiceProvider();
        }

        private static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }

        /// <summary>
        /// Handles incoming messages and checks for country mentions
        /// </summary>
        /// <param name="message">Received message</param>
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Console.WriteLine($"Message received: '{message.Content}' from {message.Author.Username} in #{message.Channel.Name} (Guild: {(message.Channel as SocketGuildChannel)?.Guild.Name ?? "DM"})");
            List<string> _imagesPaths = new List<string>();
            string upperCaseMessage = message.Content.ToUpper();

            // Ignore messages from ourselves
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            // Check message against all known English country names
            foreach (var country in _englishCountries)
            {
                if (upperCaseMessage.Contains(country))
                {
                    string imagePath = $"{_imageDirectory}/{country}.jpg";
                    
                    // Add image if file exists
                    if (System.IO.File.Exists(imagePath))
                    {
                        _imagesPaths.Add(imagePath);
                    }
                }
            }

            // If we found any matching countries
            if (_imagesPaths.Count > 0)
            {
                List<FileAttachment> attachments = new List<FileAttachment>();

                // Prepare file attachments
                foreach (var imagePath in _imagesPaths)
                {
                    var filename = Path.GetFileName(imagePath);
                    var filestream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    var stream = new FileAttachment(filestream, filename);
                    attachments.Add(stream);
                }

                // Send images as reply to original message
                await message.Channel.SendFilesAsync(attachments, messageReference: new MessageReference(message.Id));
                
                // Log the activity
                Console.WriteLine($"Message received: '{message.Content}' from {message.Author.Username} in #{message.Channel.Name} (Guild: {(message.Channel as SocketGuildChannel)?.Guild.Name ?? "DM"})");

                // Clean up file streams
                foreach (var attachment in attachments)
                {
                    await attachment.Stream.DisposeAsync();
                }
            }
        }
    }
}