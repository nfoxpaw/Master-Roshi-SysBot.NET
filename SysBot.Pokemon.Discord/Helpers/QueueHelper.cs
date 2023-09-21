﻿using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using PKHeX.Core;
using System;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public static class QueueHelper<T> where T : PKM, new()
    {
        private const uint MaxTradeCode = 9999_9999;

        public static async Task AddToQueueAsync(SocketCommandContext context, int code, string trainer, RequestSignificance sig, T trade, PokeRoutineType routine, PokeTradeType type, SocketUser trader)
        {
            if ((uint)code > MaxTradeCode)
            {
                await context.Channel.SendMessageAsync("Trade code should be 00000000-99999999!").ConfigureAwait(false);
                return;
            }

            try
            {
                const string helper = "Added you to the queue!\r\nBegin search once I tell you to.";
                IUserMessage test = await trader.SendMessageAsync(helper).ConfigureAwait(false);

                // Try adding
                var result = AddToTradeQueue(context, trade, code, trainer, sig, routine, type, trader, out var msg);

                // Notify in channel
                // await context.Channel.SendMessageAsync(msg).ConfigureAwait(false); 

                string embedMsg, embedTitle, embedAuthor;
                bool CanGMax = false;
                uint FormArgument = 0;
                var me = SysCord<T>.Runner;
                string botversion = "";

                if (me is not null)
                    botversion = me.ToString()!.Substring(46, 3);
                var gamever = botversion switch
                {
                    "PK9" => "Scarlet & Violet",
                    "PK8" => "Sword & Shield",
                    "PA8" => "Legends Arceus",
                    "PB8" => "Brilliant Diamond & Shining Pearl",
                    _ => "Let's Go Pikachu & Eevee",
                };

                switch (trade.Generation)
                {
                    case (int)GameVersion.X or (int)GameVersion.Y:
                        PK6 mon6 = (PK6)trade.Clone();
                        FormArgument = mon6.FormArgument;
                        break;
                    case (int)GameVersion.SN or (int)GameVersion.MN or (int)GameVersion.US or (int)GameVersion.UM:
                        PK7 mon7 = (PK7)trade.Clone();
                        FormArgument = mon7.FormArgument;
                        break;
                    case (int)GameVersion.GP or (int)GameVersion.GE:
                        PB7 monLGPE = (PB7)trade.Clone();
                        FormArgument = monLGPE.FormArgument;
                        break;
                    case (int)GameVersion.SW or (int)GameVersion.SH:
                        PK8 mon8 = (PK8)trade.Clone();
                        CanGMax = mon8.CanGigantamax;
                        FormArgument = mon8.FormArgument;
                        break;
                    case (int)GameVersion.BD or (int)GameVersion.SP:
                        PB8 monBDSP = (PB8)trade.Clone();
                        CanGMax = monBDSP.CanGigantamax;
                        FormArgument = monBDSP.FormArgument;
                        break;
                    case (int)GameVersion.PLA:
                        PA8 monLA = (PA8)trade.Clone();
                        CanGMax = monLA.CanGigantamax;
                        FormArgument = monLA.FormArgument;
                        break;
                    case (int)GameVersion.SL or (int)GameVersion.VL:
                        PK9 mon9 = (PK9)trade.Clone();
                        FormArgument = mon9.FormArgument;
                        break;
                }
                if (routine == PokeRoutineType.Clone || routine == PokeRoutineType.Dump || routine == PokeRoutineType.DirectTrade || routine == PokeRoutineType.SeedCheck)
                {
                    var cd = SysCordSettings.HubConfig.TradeAbuse.TradeCooldown;
                    var p = SysCordSettings.Settings.CommandPrefix;

                    Color embedMsgColor = new();
                    embedTitle = $"Search once bot DMs you Initializing trade\n";
                    if (context.User.GlobalName == null)
                        embedAuthor = $"{context.User.Username}'s ";
                    else
                        embedAuthor = $"{context.User.GlobalName}'s ";
                    embedMsg = $"";

                    Random luck = new();
                    int lucky = luck.Next(0, 10);

                    if (routine == PokeRoutineType.SeedCheck)
                    {
                        embedMsgColor = 0xF9F815;
                        embedAuthor += "Seed Check";
                        embedMsg += $"Current Game: **{gamever}**\n";
                        embedMsg += $"Show a Pokémon caught from a raid to check seed\n";
                        embedMsg += $"This function only works on SWSH\n\n";
                        embedMsg += $"Enjoy & Please come again !";
                    }
                    else if (routine == PokeRoutineType.Clone)
                    {
                        embedMsgColor = 0xF9F815;
                        embedAuthor += "Clone Request";
                        embedMsg += $"Current Game: **{gamever}**\n";
                        embedMsg += $"Show a Pokémon to be cloned & Hit B to change your offer\n";
                        embedMsg += $"Offer a trash Pokémon to receive your clone\n\n";
                        embedMsg += $"Current Cooldown: **{cd}** mins\n";
                        embedMsg += $"Enjoy & Please come again !";
                    }
                    else if (routine == PokeRoutineType.Dump)
                    {
                        embedMsgColor = 0x6015F9;
                        embedAuthor += "Dump Request";
                        embedMsg += $"Current Game: **{gamever}**\n";
                        embedMsg += $"Show Pokémon(s) to be dumped\n";
                        embedMsg += $"For **{SysCordSettings.HubConfig.Trade.MaxDumpTradeTime}** seconds, show up to **{SysCordSettings.HubConfig.Trade.MaxDumpsPerTrade}** Pokémon\n\n";
                        embedMsg += $"Current Cooldown: **{cd}** mins\n";
                        embedMsg += $"Enjoy & Please come again !";
                    }
                    else if (routine == PokeRoutineType.DirectTrade)
                    {
                        
                        if (type == PokeTradeType.LinkSV || type == PokeTradeType.LinkSWSH || type == PokeTradeType.LinkLA)
                        {
                            embedAuthor += "Direct Trade Request";
                            embedMsg += $"Nickname/Features trade using [**Click for Nicknames**](<{SysCordSettings.HubConfig.Distribution.SheetLink}>)\n";
                            embedMsg += $"Game: **{gamever}**\n";
                            embedMsg += $"Cooldown: **{cd}** mins\n\n";
                            embedMsg += $"Commands:\n**{p}rsv**, **{p}rme**, **{p}t**, **{p}it**, **{p}tc**, **{p}dump**, **{p}clone**, **{p}checkcd**, **{p}dtl**, **{p}spf**\n";
                            embedMsg += $"Enjoy trading !";
                        }
                    }
                    var icon = context.User.GetAvatarUrl() switch
                    {
                        null => context.User.GetDefaultAvatarUrl(),
                        _ => context.User.GetAvatarUrl(),
                    };
                    EmbedAuthorBuilder embedAuthorBuild = new()
                    {
                        IconUrl = icon,
                        Name = embedAuthor,
                    };
                    EmbedFooterBuilder embedFtr = new()
                    {
                        Text = $"Current Position: " + SysCord<T>.Runner.Hub.Queues.Info.Count.ToString() + ".\nEstimated Wait: " + Math.Round(((SysCord<T>.Runner.Hub.Queues.Info.Count) * 1.65), 1).ToString() + " minutes.",
                        IconUrl = "https://cdn.discordapp.com/emojis/1132202155789537280.webp?size=128&quality=lossless"
                    };

                    Roshicl traderoshi = new();
                    string embedThumbUrl = "https://images-ext-1.discordapp.net/external/QvIHlXFAFbgPEyZkFkPOW_TlSliYS_ZICoyibAuWmSE/https/m.media-amazon.com/images/I/81SFFfP%2BlWL.jpg?width=468&height=662";

                    EmbedBuilder builder = new()
                    {
                        Color = embedMsgColor,
                        Author = embedAuthorBuild,
                        Title = embedTitle,
                        Description = embedMsg,
                        ThumbnailUrl = embedThumbUrl,
                        Footer = embedFtr
                    };
                    await context.Channel.SendMessageAsync("", false, builder.Build()).ConfigureAwait(false);
                }
                else
                {
                    var list = FormConverter.GetFormList(trade.Species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, trade.Context);
                    string HeldItem = Roshicl.FixHeldItemName(((EmbedItem)trade.HeldItem).ToString());
                    embedTitle = trade.IsShiny ? ":sparkles:" : "";
                    if (trade.Form != 0)
                        embedTitle += $"{list[trade.Form]} {(Species)trade.Species}";
                    else
                        embedTitle += $"{(Species)trade.Species}";
                    if (trade.Gender == 0)
                        embedTitle += " (M)";
                    else if (trade.Gender == 1)
                        embedTitle += " (F)";
                    if (trade.HeldItem > 0)
                        embedTitle += $" **➜** {HeldItem}";
                    if(context.User.GlobalName == null)
                        embedAuthor = $"{context.User.Username}'s ";
                    else
                        embedAuthor = $"{context.User.GlobalName}'s ";
                    embedAuthor += trade.IsShiny ? "Shiny " : "";
                    embedAuthor += "Pokémon:";

                    embedMsg = $"**Ability**: {(Ability)trade.Ability}";
                    embedMsg += $"\n**Level**: {trade.CurrentLevel}";
                    if (gamever == "SV")
                    {
                        PK9 tradesv = (PK9)(PKM)trade;
                        embedMsg += $"\n**Tera**: {tradesv.TeraType}";
                    }
                    embedMsg += $"\n**Nature**: {(Nature)trade.Nature}";
                    embedMsg += $"\n**IVs**: {trade.IV_HP}/{trade.IV_ATK}/{trade.IV_DEF}/{trade.IV_SPA}/{trade.IV_SPD}/{trade.IV_SPE}";
                    if (trade.EVTotal != 0)
                        embedMsg += $"\n**EVs**: {trade.EV_HP}/{trade.EV_ATK}/{trade.EV_DEF}/{trade.EV_SPA}/{trade.EV_SPD}/{trade.EV_SPE}";
                    embedMsg += $"\n**__Moves__**";
                    if (trade.Move1 != 0)
                        embedMsg += $"\n- {(Move)trade.Move1}";
                    if (trade.Move2 != 0)
                        embedMsg += $"\n- {(Move)trade.Move2}";
                    if (trade.Move3 != 0)
                        embedMsg += $"\n- {(Move)trade.Move3}";
                    if (trade.Move4 != 0)
                        embedMsg += $"\n- {(Move)trade.Move4}";
                    if (trade.EncryptionConstant == 0)
                        embedMsg += $"\n### :bangbang: **Pokémon generated with 0 EC** :bangbang:";
                    if (botversion == "PK9")
                    {
                        PK9 tradesv = (PK9)(PKM)trade;
                        if (trade.Generation != 9 && tradesv.Tracker == 0 && !trade.IsEgg)
                            embedMsg += $"\n## :bangbang: **Pokémon has NO HOME Tracker** :bangbang:\n\n";
                    }
                    embedMsg += $"\n\n{trader.Mention} - Added to the LinkTrade queue";

                    EmbedAuthorBuilder embedAuthorBuild = new()
                    {
                        IconUrl = "https://raw.githubusercontent.com/PhantomL98/HomeImages/main/Ballimg/50x50/" + ((Ball)trade.Ball).ToString().ToLower() + "ball.png",
                        Name = embedAuthor,
                    };
                    EmbedFooterBuilder embedFtr = new()
                    {
                        Text = $"Current Position: " + SysCord<T>.Runner.Hub.Queues.Info.Count.ToString() + ".\nEstimated Wait: " + Math.Round(((SysCord<T>.Runner.Hub.Queues.Info.Count) * 1.65), 1).ToString() + " minutes.",
                        IconUrl = "https://raw.githubusercontent.com/PhantomL98/HomeImages/main/Sprites/200x200/poke_capture_0363_000_mf_n_00000000_f_n.png"
                    };

                    Color embedMsgColor = new((uint)Enum.Parse(typeof(EmbedColor), Enum.GetName(typeof(Ball), trade.Ball)));
                    Roshicl tradespheal = new();
                    string embedThumbUrl = await tradespheal.EmbedImgUrlBuilder(trade, CanGMax, FormArgument.ToString("00000000")).ConfigureAwait(false);

                    EmbedBuilder builder = new()
                    {
                        //Optional color
                        Color = embedMsgColor,
                        Author = embedAuthorBuild,
                        Title = embedTitle,
                        Description = embedMsg,
                        ThumbnailUrl = embedThumbUrl,
                        Footer = embedFtr
                    };
                    await context.Channel.SendMessageAsync("", false, builder.Build()).ConfigureAwait(false);
                }

                // Notify in PM to mirror what is said in the channel.
                await trader.SendMessageAsync($"{msg}\nYour trade code will be **{code:0000 0000}**.").ConfigureAwait(false);

                // Clean Up
                if (result)
                {
                    // Delete the user's join message for privacy
                    if (!context.IsPrivate)
                        await context.Message.DeleteAsync(RequestOptions.Default).ConfigureAwait(false);
                }
                else
                {
                    // Delete our "I'm adding you!", and send the same message that we sent to the general channel.
                    await test.DeleteAsync().ConfigureAwait(false);
                }
            }
            catch (HttpException ex)
            {
                await HandleDiscordExceptionAsync(context, trader, ex).ConfigureAwait(false);
            }
        }

        public static async Task AddToQueueAsync(SocketCommandContext context, int code, string trainer, RequestSignificance sig, T trade, PokeRoutineType routine, PokeTradeType type)
        {
            await AddToQueueAsync(context, code, trainer, sig, trade, routine, type, context.User).ConfigureAwait(false);
        }

        private static bool AddToTradeQueue(SocketCommandContext context, T pk, int code, string trainerName, RequestSignificance sig, PokeRoutineType type, PokeTradeType t, SocketUser trader, out string msg)
        {
            var user = trader;
            var userID = user.Id;
            var name = user.Username;

            var trainer = new PokeTradeTrainerInfo(trainerName, userID);
            var notifier = new DiscordTradeNotifier<T>(pk, trainer, code, user);
            var detail = new PokeTradeDetail<T>(pk, trainer, notifier, t, code, sig == RequestSignificance.Favored);
            var trade = new TradeEntry<T>(detail, userID, type, name);

            var hub = SysCord<T>.Runner.Hub;
            var Info = hub.Queues.Info;
            var added = Info.AddToTradeQueue(trade, userID, sig == RequestSignificance.Owner);

            if (added == QueueResultAdd.AlreadyInQueue)
            {
                msg = "Sorry, you are already in the queue.";
                return false;
            }

            var position = Info.CheckPosition(userID, type);

            var ticketID = "";
            if (TradeStartModule<T>.IsStartChannel(context.Channel.Id))
                ticketID = $", unique ID: {detail.ID}";

            var pokeName = "";
            if (t == PokeTradeType.Specific && pk.Species != 0)
                pokeName = $" Receiving: {GameInfo.GetStrings(1).Species[pk.Species]}.";
            msg = $"I've added you to the queue, {user.Mention}!\r\nCurrent Position: {position.Position}.{pokeName}";

            var botct = Info.Hub.Bots.Count;
            if (position.Position > botct)
            {
                var eta = Info.Hub.Config.Queues.EstimateDelay(position.Position, botct);
                msg += $" Estimated: {eta:F1} minutes.";
            }
            return true;
        }

        private static async Task HandleDiscordExceptionAsync(SocketCommandContext context, SocketUser trader, HttpException ex)
        {
            string message = string.Empty;
            switch (ex.DiscordCode)
            {
                case DiscordErrorCode.InsufficientPermissions or DiscordErrorCode.MissingPermissions:
                    {
                        // Check if the exception was raised due to missing "Send Messages" or "Manage Messages" permissions. Nag the bot owner if so.
                        var permissions = context.Guild.CurrentUser.GetPermissions(context.Channel as IGuildChannel);
                        if (!permissions.SendMessages)
                        {
                            // Nag the owner in logs.
                            message = "You must grant me \"Send Messages\" permissions!";
                            Base.LogUtil.LogError(message, "QueueHelper");
                            return;
                        }
                        if (!permissions.ManageMessages)
                        {
                            var app = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                            var owner = app.Owner.Id;
                            message = $"<@{owner}> You must grant me \"Manage Messages\" permissions!";
                        }
                    }
                    break;
                case DiscordErrorCode.CannotSendMessageToUser:
                    {
                        // The user either has DMs turned off, or Discord thinks they do.
                        message = context.User == trader ? "You must enable private messages in order to be queued!" : "The mentioned user must enable private messages in order for them to be queued!";
                    }
                    break;
                default:
                    {
                        // Send a generic error message.
                        message = ex.DiscordCode != null ? $"Discord error {(int)ex.DiscordCode}: {ex.Reason}" : $"Http error {(int)ex.HttpCode}: {ex.Message}";
                    }
                    break;
            }
            await context.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
