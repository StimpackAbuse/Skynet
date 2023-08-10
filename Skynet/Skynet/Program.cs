using System;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Encryption;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using System.Runtime.InteropServices;

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

        private static string ENV_FILE = "Skynet_ENV.json";
        private static string PREFIX = "!";

        private static string clientID = "";
        private static string clientSecret = "";
        private static string botToken = "";

        public static string BotToken { get => botToken; }
        public static string ClientID { get => clientID; }
        public static string ClientSecret { get => clientSecret; }

        private InteractionService? interactionService;
        private DiscordSocketClient? client;
        private InteractionService? interaction;
        private CommandService? command;
        private IServiceProvider? services;

        private static MainModulePreferences? mainModulePreferences = null;

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

            DiscordSocketConfig config = new DiscordSocketConfig { WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance, 
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent };

            services = new ServiceCollection().AddSingleton(config).AddSingleton<DiscordSocketClient>().
                AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>())).AddSingleton<CommandHandler>().BuildServiceProvider();

            client = services.GetRequiredService<DiscordSocketClient>();
            interaction = services.GetRequiredService<InteractionService>();

            command = new CommandService();

            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            client.Log += ClientLog;
            client.Ready += RegisterCommandsAsync;

            await HookMessageAsync();
            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Call this function to register commands.
        /// </summary>
        public async Task RegisterCommandsAsync()
        {
            if (client == null || interaction == null || services == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_INSTANCE_INVALID, true);
                return;
            }

            interactionService = new InteractionService(client);

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            //await interactionService.RegisterCommandsToGuildAsync(739841223250018344);
            await interactionService.RegisterCommandsGloballyAsync(true); //This might take some time - use guildasync for tests

            Console.WriteLine("Connected as {0}", client.CurrentUser);
        }

        public async Task HookMessageAsync()
        {
            if (client == null || services == null || command == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_INSTANCE_INVALID, true);
                return;
            }

            await command.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            client.MessageReceived += HandleMessageAsync;
        }

        /// <summary>
        /// This function is called on message input.
        /// </summary>
        /// <param name="arg">Discord socket message.</param>
        public async Task HandleMessageAsync(SocketMessage arg)
        {
            SocketUserMessage? msg = arg as SocketUserMessage;
            SocketCommandContext? context = new SocketCommandContext(client, msg);

            if (msg == null || command == null || client == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_INSTANCE_INVALID, true);
                return;
            }

            if (mainModulePreferences == null)
            {
                mainModulePreferences = new MainModulePreferences(context.Guild.Id.ToString());
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

            JSONUtility.Serialize(Directory.GetCurrentDirectory(), ENV_FILE, data);
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

            ConfigureData data = JSONUtility.Deserialize<ConfigureData>(Directory.GetCurrentDirectory(), ENV_FILE);
            clientID = data.id;
            clientSecret = data.secret;
            botToken = data.token;
        }
        #endregion

        public static void UpdatePreferences(MainModulePreferences pref)
        {
            mainModulePreferences = pref;
            PREFIX = pref.Prefix;
        }

        private static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
    }
}
