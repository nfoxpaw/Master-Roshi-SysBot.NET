using PKHeX.Core;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System;
using System.IO;
using System.Threading;

namespace SysBot.Pokemon.Discord
{
    [Summary("Custom Master Roshi Commands")]
    public class RoshiModule<T> : ModuleBase<SocketCommandContext> where T : PKM, new()
    {
        private static TradeQueueInfo<T> Info => SysCord<T>.Runner.Hub.Queues.Info;

        [Command("directTrade")]
        [Alias("drt", "rsv")]
        [Summary("Starts a Distribution Trade through Discord")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task DRT()
        {
            var code = Info.GetRandomTradeCode();
            var sig = Context.User.GetFavor();
            var me = SysCord<T>.Runner;
            string botversion;
            if (me is not null)
            {
                botversion = me.ToString()!.Substring(46, 3);
                switch (botversion)
                {
                    case "PK9":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkSV).ConfigureAwait(false);
                        break;
                    case "PK8":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkSWSH).ConfigureAwait(false);
                        break;
                    case "PA8":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkLA).ConfigureAwait(false);
                        break;
                    case "PB8":
                        await ReplyAsync($"This command is disabled for BDSP").ConfigureAwait(false);
                        break;
                }
            }
        }

        [Command("directTrade")]
        [Alias("drt", "rsv")]
        [Summary("Starts a Distribution Trade through Discord")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task DRT([Summary("Trade Code")] int code)
        {
            var sig = Context.User.GetFavor();
            var me = SysCord<T>.Runner;
            string botversion;
            if (me is not null)
            {
                botversion = me.ToString()!.Substring(46, 3);
                switch (botversion)
                {
                    case "PK9":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkSV).ConfigureAwait(false);
                        break;
                    case "PK8":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkSWSH).ConfigureAwait(false);
                        break;
                    case "PA8":
                        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.DirectTrade, PokeTradeType.LinkLA).ConfigureAwait(false);
                        break;
                    case "PB8":
                        await ReplyAsync($"This command is disabled for BDSP").ConfigureAwait(false);
                        break;
                }
            }
        }

        [Command("DTList")]
        [Alias("dtl")]
        [Summary("List the users in the DirectTrade queue.")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task GetDTListAsync()
        {
            string msg = Info.GetTradeList(PokeRoutineType.DirectTrade);
            var embed = new EmbedBuilder();
            embed.AddField(s =>
            {
                s.Name = "Pending Trades";
                s.Value = msg;
                s.IsInline = false;
            });
            await ReplyAsync("These are the users who are currently waiting:", embed: embed.Build()).ConfigureAwait(false);
        }


    }
}