using System;
using NLua;
using Discord;
using Discord.WebSocket;

namespace DBotLua
{
    internal class Program
    {
        // you fucking dumbass
        private static Lua luaState { get => LuaState.state; }
        private static string luaPath = "";
        private readonly DiscordSocketClient _client;
        private static string token { get => LuaState.state["token"].ToString()!; }
        private static string prefix { get => LuaState.state["prefix"].ToString()!; }

        static void Main(string[] args)
        {
            luaPath = Path.Join(Directory.GetCurrentDirectory(), "bot.lua");
            if (!File.Exists(luaPath))
            {
                File.Create(luaPath, 0);
                Console.WriteLine("Write 'token = token_here' in your LUA file!");
                Environment.Exit(1);
            }

            LuaState.refresh(luaPath);
            LuaState.check("token");
            LuaState.check("prefix");

            Console.WriteLine("Token seems fine, proceeding with startup");

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return LuaState.execute("onReady");
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == $"{prefix}refresh")
            {
                var res = await LuaState.refresh(luaPath);
                var ans = (res == "Success" ? "Refreshed LUA State" : $"Failed to refresh LUA State: {res}");
                await message.Channel.SendMessageAsync(ans);
                return;
            }

            await LuaState.execute("onMessage", message);
        }
    }
}