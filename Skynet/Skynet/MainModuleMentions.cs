using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skynet
{
    /// <summary>
    /// Class for mentions towards bot.
    /// </summary>
    class MainModuleMentions
    {
        public static async Task MentionAction(SocketCommandContext context, SocketUserMessage msg)
        {
            bool isEveryone = msg.MentionedEveryone;
            bool isMultipleMention = msg.MentionedUsers.Count > 1;

            if (isEveryone)
            {
                await WarnEveryoneMention(context, msg);
            }
            else if (isMultipleMention)
            {
                
            }
            else
            {
                await OnSingleMention(context, msg);
            }
        }

        private static async Task WarnEveryoneMention(SocketCommandContext context, SocketUserMessage msg)
        {
            await context.Channel.SendMessageAsync("@everyone Mention Detected");
        }

        private static async Task OnSingleMention(SocketCommandContext context, SocketUserMessage msg)
        {
            await context.Channel.SendMessageAsync("Specific Mention Detected");
        }
    }
}
