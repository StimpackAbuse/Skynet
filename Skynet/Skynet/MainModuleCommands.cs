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
using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

namespace Skynet
{
    /// <summary>
    /// Class for commands towards bot.
    /// </summary>
    public class MainModuleCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private CommandHandler handler;
        const int argIdx = 1;

        public MainModuleCommands(CommandHandler handler)
        {
            this.handler = handler;
        }

        [SlashCommand("help", "Ask for help")]
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

            await ReplyAsync(null, false, embed.Build());
            await RespondAsync("Done");
        }
    }

    public class MainModuleMessageCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong");
        }
    }
}
