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
        private MainModulePreferences? preference = null;

        public MainModuleCommands(CommandHandler handler)
        {
            this.handler = handler;
        }
        /*
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
        */
        [SlashCommand("prefix", "Change Prefix")]
        public async Task ChangePrefix(string prefix)
        {
            if (preference == null)
            {
                preference = new MainModulePreferences(Context.Guild.Id.ToString());
            }

            preference.UpdateServerPreference(new MainModulePreferences.Preference(Context.Guild.Id.ToString(), prefix));
            Program.UpdatePreferences(preference);
            await RespondAsync(string.Format("Prefix changed to {0}", prefix));
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
