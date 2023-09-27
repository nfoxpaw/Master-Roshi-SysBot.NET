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
            await ReplyAsync("Read <#1115122854376247356> for help").ConfigureAwait(false);
        }
    }
}