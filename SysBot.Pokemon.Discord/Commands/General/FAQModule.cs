using Discord.Commands;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public class FAQModule : ModuleBase<SocketCommandContext>
    {
        [Command("faq")]
        [Summary("Directs you to the info thread")]
        public async Task PingAsync()
        {
            await ReplyAsync("Read <#1090060306044170290> for help").ConfigureAwait(false);
        }
    }
}