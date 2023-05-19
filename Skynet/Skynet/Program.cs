﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Diagnostics.Tracing;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Encryption;
using Microsoft.Extensions.DependencyInjection;

namespace Skynet
{
    class Program
    {
        [Serializable]
        private struct ConfigureData
        {
            public string id;
            public string secret;
            public string token;
        }

        private static string ENV_FILE = "SkyNet_ENV.json";
        private static string PREFIX = "#";

        private string clientID = "";
        private string clientSecret = "";
        private string botToken = "";

        private DiscordSocketClient? client;
        private CommandService? command;
        private IServiceProvider? services;

        private Task ClientLog(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        #region Bot Initialization
        /// <summary>
        /// Bot initialization function.
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync()
        {
            await Task.Run(() => TokenInitialize().GetAwaiter().GetResult());

            DiscordSocketConfig config = new DiscordSocketConfig { WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance };

            client = new DiscordSocketClient(config);
            command = new CommandService();
            services = new ServiceCollection().AddSingleton(client).AddSingleton(command).AddSingleton<InteractiveService>().BuildServiceProvider();

            client.Log += ClientLog;

            await RegisterCommandsAsync();
            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Call this function to register commands.
        /// </summary>
        public async Task RegisterCommandsAsync()
        {
            if (client == null || command == null)
            {
                Console.WriteLine("ERROR: ESSENTIAL INSTANCE IS NULL - SHUTTING DOWN");
                Environment.Exit((int)ErrorCode.MAINMDL_INSTANCE_INVALID);
            }

            client.MessageReceived += HandleCommandAsync;
            await command.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        /// <summary>
        /// This function is called on command input.
        /// </summary>
        /// <param name="arg">Discord socket message.</param>
        public async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage? msg = arg as SocketUserMessage;
            SocketCommandContext? context = new SocketCommandContext(client, msg);

            if (msg == null || command == null || client == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_INSTANCE_INVALID, true);
                return;
            }

            int argPos = 0;

            if (msg.Author.IsBot)
                return;

            if (msg.HasStringPrefix(PREFIX, ref argPos))
            {
                var result = await command.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
            if (msg.MentionedUsers.Where((item) => item.Id == ulong.Parse(clientID)).Count() > 0 || msg.MentionedEveryone)
            {
                await MainModuleMentions.MentionAction(context, msg);
            }
        }
        #endregion

        #region Token Initialization
        /// <summary>
        /// Call this function to create ENV file which contains Discord tokens.
        /// </summary>
        private async Task CreateTokenJSON()
        {
            Console.WriteLine("ENV FILE NOT FOUND - CONFIGURING MODE INITIATED");
            Console.Write("CLIENT ID: ");
            string? id = await Task.Run(() => Console.ReadLine());
            Console.Write("CLIENT SECRET: ");
            string? secret = await Task.Run(() => Console.ReadLine());
            Console.Write("BOT TOKEN: ");
            string? token = await Task.Run(() => Console.ReadLine());

            if (id == null || secret == null || token == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_WRONG_CONFIG, true);
                return;
            }

            ConfigureData data;
            data.id = id;
            data.secret = secret;
            data.token = token;
            string configureJson = JsonConvert.SerializeObject(data);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), ENV_FILE), configureJson);
        }

        /// <summary>
        /// Token initialize function.
        /// </summary>
        private async Task TokenInitialize()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ENV_FILE)))
            {
                await CreateTokenJSON();
            }

            string tokenJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), ENV_FILE));
            ConfigureData data = JsonConvert.DeserializeObject<ConfigureData>(tokenJson);
            clientID = data.id;
            clientSecret = data.secret;
            botToken = data.token;
        }
        #endregion

        private static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
    }
}
