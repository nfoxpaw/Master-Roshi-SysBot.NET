using Discord.Commands;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public class HiModule : ModuleBase<SocketCommandContext>
    {
        [Command("hi")]
        [Summary("Say hello to the bot and get a response.")]
        public async Task PingAsync()
        {
            var str = SysCordSettings.Settings.HelloResponse;
            var msg = string.Format(str, Context.User.Mention);
            await ReplyAsync(msg).ConfigureAwait(false);
            await ReplyAsync("https://tenor.com/view/master-roshi-smoking-puff-gif-13772269").ConfigureAwait(false);
        }
    }
}