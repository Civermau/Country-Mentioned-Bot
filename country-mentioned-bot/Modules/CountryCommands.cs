using Discord;
using Discord.Interactions;

namespace Country_Mentioned_Bot.Services
{
    public class CountryCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService? Commands { get; set; }
        private CommandHandler _handler;
        private bool exactMatch = false;

        public CountryCommands(CommandHandler handler){
            _handler = handler;
        }

        [SlashCommand("set-language", "Set the language for the bot")]
        public async Task SetLanguage(string language)
        {
            await RespondAsync($"Language set to: {language}");
        }

        [SlashCommand("set-multi-language", "Set multiple languages for the bot")]
        public async Task SetMultiLanguage(string languages)
        {
            await RespondAsync($"Languages set to: {languages}");
        }


        [SlashCommand("toggle-exact-match", "Toggle exact match for country names")]
        public async Task ToggleExactMatch()
        {
            await RespondAsync("Exact match is now: " + (exactMatch ? "enabled" : "disabled"));
        }

        [SlashCommand("embed-test", "Testing the embed")]
        public async Task EmbedTest()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Test");
            embed.WithDescription("This is a test");
            embed.WithUrl("https://www.crunchyroll.com/es/series/GXJHM3N2E/k-on");
            embed.WithThumbnailUrl("https://i.pinimg.com/474x/6a/7e/d9/6a7ed9c56ad7001cec4bce0e5cbd2c43.jpg");
            embed.WithImageUrl("https://i.pinimg.com/736x/75/89/86/758986915aaff55bd3f04ea845d82024.jpg");
            embed.WithCurrentTimestamp();
            // embed.WithTimestamp(DateTimeOffset.UtcNow);
            embed.WithColor(Color.DarkRed);
            embed.WithAuthor("Test Author", "https://pm1.aminoapps.com/6215/b0b94fe9b5b078270be893c7e505e2dadda710e4_128.jpg", "https://www.crunchyroll.com/es/series/GXJHM3N2E/k-on");
            embed.WithFooter("Test Footer", "https://i.pinimg.com/474x/55/69/9f/55699f0af7b167001f5970c5bc17d33a.jpg");
            embed.AddField("Test Field", "This is a test field");

            await RespondAsync(embed: embed.Build());
        }
    }
}
