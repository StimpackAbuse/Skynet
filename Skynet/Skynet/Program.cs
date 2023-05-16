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
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Encryption;

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

        private static string ENV_FILE = "SkyNet_env";

        private DiscordSocketClient client;
        private CommandService command;
        private IServiceProvider services;

        public async Task RunBotAsync()
        {
            
        }

        private static void Main(string[] args)
        {
            //최초 실행 시 
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ENV_FILE)))
            {
                Console.WriteLine("ENV FILE NOT FOUND - CONFIGURING MODE INITIATED");
                Console.Write("CLIENT ID: ");
                string id = Console.ReadLine();
                Console.Write("CLIENT SECRET: ");
                string secret = Console.ReadLine();
                Console.Write("BOT TOKEN: ");
                string token = Console.ReadLine();

                ConfigureData data;
                data.id = id;
                data.secret = secret;
                data.token = token;

                string configureJson = JsonConvert.SerializeObject(data);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), ENV_FILE), configureJson);
            }
        }
    }
}
