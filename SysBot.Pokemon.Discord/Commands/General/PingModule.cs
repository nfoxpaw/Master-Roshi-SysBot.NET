using Discord.Commands;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Makes the bot respond, indicating that it is running.")]
        public async Task PingAsync()
        {
            await ReplyAsync("https://i.pinimg.com/originals/f4/61/f9/f461f95baef69078742a859aa86beaab.gif").ConfigureAwait(false);
        }
    }
}