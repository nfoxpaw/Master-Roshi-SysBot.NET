﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using PKHeX.Core;
using SysBot.Base;

namespace SysBot.Pokemon.Discord
{
    public class SudoModule<T> : ModuleBase<SocketCommandContext> where T : PKM, new()
    {
        [Command("ledyspecies")]
        [Alias("ls")]
        [Summary("Changes the Ledy species for idle distribution.")]
        [RequireSudo]
        public async Task ChangeLedySpecies([Remainder] string input)
        {
            if (input.ToLower() == "none")
            {
                SysCordSettings.HubConfig.Distribution.LedySpecies = (ushort)Species.None;
                await ReplyAsync("LedySpecies has been changed to None.").ConfigureAwait(false);
                EchoUtil.Echo("LedySpecies has been changed to None.");
                return;
            }
            bool isSpecies = Enum.TryParse(input, true, out Species result);
            if (!isSpecies)
            {
                await ReplyAsync("Please enter a valid Species.").ConfigureAwait(false);
                return;
            }
            var mode = typeof(T).ToString();
            IPersonalTable infoTable = mode switch
            {
                "PK9" => PersonalTable.SV,
                "PK8" => PersonalTable.SWSH,
                "PA8" => PersonalTable.LA,
                "PB8" => PersonalTable.BDSP,
                _ => PersonalTable.SV
            };
            bool isInGame = infoTable.IsSpeciesInGame((ushort)result);
            if (!isInGame)
            {
                await ReplyAsync("Please enter a Species available in the current game.").ConfigureAwait(false);
                return;
            }
            else
            {
                SysCordSettings.HubConfig.Distribution.LedySpecies = result;
                await ReplyAsync($"LedySpecies has been changed to {result}.").ConfigureAwait(false);
                EchoUtil.Echo($"LedySpecies has been changed to {result}.");
            }
        }

        /*[Command("metlocation")]
        [Alias("ml")]
        [Summary("Changes the required Met Location for TradeMon.")]
        [RequireSudo]
        public async Task ChangeMetLocation([Remainder] string input)
        {
            if (input.ToLower() == "none")
            {
                SysCordSettings.HubConfig.TradeAbuse.OfferedMetLocation = string.Empty;
                await ReplyAsync("Met Location has been changed to None.").ConfigureAwait(false);
                EchoUtil.Echo("Met Location has been changed to None.");
                return;
            }
                        
            else
            {
                SysCordSettings.HubConfig.TradeAbuse.OfferedMetLocation = input;
                await ReplyAsync($"Met Location has been changed to {input}.").ConfigureAwait(false);
                EchoUtil.Echo($"Met Location has been changed to {input}.");
            }
        }*/

        [Command("cooldown")]
        [Summary("Changes cooldown in minutes.")]
        [Alias("cd")]
        [RequireSudo]
        public async Task UpdateCooldown([Remainder] string input)
        {
            bool res = uint.TryParse(input, out var cooldown);
            if (res)
            {
                SysCordSettings.HubConfig.TradeAbuse.TradeCooldown = cooldown;
                SysCordSettings.HubConfig.TradeAbuse.CooldownUpdate = $"{DateTime.Now:yyyy.MM.dd - HH:mm:ss}";
                await ReplyAsync($"Cooldown has been updated to {cooldown} minutes.").ConfigureAwait(false);
                return;
            }
            else
            {
                await ReplyAsync("Please enter a valid number of minutes.").ConfigureAwait(false);
            }
        }

        [Command("blacklist")]
        [Summary("Blacklists mentioned user.")]
        [RequireSudo]
        // ReSharper disable once UnusedParameter.Global
        public async Task BlackListUsers([Remainder] string _)
        {
            var users = Context.Message.MentionedUsers;
            var objects = users.Select(GetReference);
            SysCordSettings.Settings.UserBlacklist.AddIfNew(objects);
            await ReplyAsync("Done.").ConfigureAwait(false);
        }

        [Command("blacklistComment")]
        [Summary("Adds a comment for a blacklisted user ID.")]
        [RequireSudo]
        // ReSharper disable once UnusedParameter.Global
        public async Task BlackListUsers(ulong id, [Remainder] string comment)
        {
            var obj = SysCordSettings.Settings.UserBlacklist.List.Find(z => z.ID == id);
            if (obj is null)
            {
                await ReplyAsync($"Unable to find a user with that ID ({id}).").ConfigureAwait(false);
                return;
            }

            var oldComment = obj.Comment;
            obj.Comment = comment;
            await ReplyAsync($"Done. Changed existing comment ({oldComment}) to ({comment}).").ConfigureAwait(false);
        }

        [Command("unblacklist")]
        [Summary("Un-Blacklists mentioned user.")]
        [RequireSudo]
        // ReSharper disable once UnusedParameter.Global
        public async Task UnBlackListUsers([Remainder] string _)
        {
            var users = Context.Message.MentionedUsers;
            var objects = users.Select(GetReference);
            SysCordSettings.Settings.UserBlacklist.RemoveAll(z => objects.Any(o => o.ID == z.ID));
            await ReplyAsync("Done.").ConfigureAwait(false);
        }

        [Command("blacklistId")]
        [Summary("Blacklists IDs. (Useful if user is not in the server).")]
        [RequireSudo]
        public async Task BlackListIDs([Summary("Comma Separated Discord IDs")][Remainder] string content)
        {
            var IDs = GetIDs(content);
            var objects = IDs.Select(GetReference);
            SysCordSettings.Settings.UserBlacklist.AddIfNew(objects);
            await ReplyAsync("Done.").ConfigureAwait(false);
        }

        [Command("unBlacklistId")]
        [Summary("Un-Blacklists IDs. (Useful if user is not in the server).")]
        [RequireSudo]
        public async Task UnBlackListIDs([Summary("Comma Separated Discord IDs")][Remainder] string content)
        {
            var IDs = GetIDs(content);
            SysCordSettings.Settings.UserBlacklist.RemoveAll(z => IDs.Any(o => o == z.ID));
            await ReplyAsync("Done.").ConfigureAwait(false);
        }

        [Command("blacklistSummary")]
        [Alias("printBlacklist", "blacklistPrint")]
        [Summary("Prints the list of blacklisted users.")]
        [RequireSudo]
        public async Task PrintBlacklist()
        {
            var lines = SysCordSettings.Settings.UserBlacklist.Summarize();
            var msg = string.Join("\n", lines);
            await ReplyAsync(Format.Code(msg)).ConfigureAwait(false);
        }

        private RemoteControlAccess GetReference(IUser channel) => new()
        {
            ID = channel.Id,
            Name = channel.Username,
            Comment = $"Added by {Context.User.Username} on {DateTime.Now:yyyy.MM.dd-hh:mm:ss}",
        };

        private RemoteControlAccess GetReference(ulong id) => new()
        {
            ID = id,
            Name = "Manual",
            Comment = $"Added by {Context.User.Username} on {DateTime.Now:yyyy.MM.dd-hh:mm:ss}",
        };

        protected static IEnumerable<ulong> GetIDs(string content)
        {
            return content.Split(new[] { ",", ", ", " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(z => ulong.TryParse(z, out var x) ? x : 0).Where(z => z != 0);
        }
    }
}