using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Discord.Interactions;

namespace Skynet
{
    /// <summary>
    /// Class for commands towards bot.
    /// </summary>
    class MainModuleCommands : InteractiveBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
            {
                Title = "HELP",
                Description = "Command lists, etc."
            };
            embed.AddField("!set {name}", "Set sepcific player to analyze.");
            embed.AddField("!mmr", "View MMR of designated player.");
            embed.AddField("!match", "View match results of designated player.");
            embed.WithAuthor(Context.Client.CurrentUser)
                .WithColor(Color.Blue)
                .WithTitle("Help Message")
                .WithDescription("Commands list, etc.")
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
