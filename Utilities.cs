﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace DogMeat
{
    public class Utilities
    {
        #region Variables

        private static bool ContinueShutdown;

        public static IRole GetMasterRole(SocketGuild Guild)
        {
            foreach (SocketRole Role in Guild.Roles)
                if (Role.Name == "Master")
                    return Role;
            return null;
        }

        public static IRole GetMutedRole(SocketGuild Guild)
        {
            foreach (SocketRole Role in Guild.Roles)
                if (Role.Name == "Muted")
                    return Role;
            return null;
        }

        public static IRole GetRole(SocketGuild Guild, ulong ID)
        {
            foreach (SocketRole Role in Guild.Roles)
                if (Role.Id == ID)
                    return Role;
            return null;
        }

        private static async Task<String[]> DogmeatResponsesAsync()
        {
            HttpClient Client = new HttpClient();
            String url = "http://198.245.61.226/kr4ken/dogmeat_replies.txt";
            HttpResponseMessage Response = await Client.GetAsync(url);
            string Content = await Response.Content.ReadAsStringAsync();
            return Content.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static async Task<String[]> DogmeatMemesAsync()
        {
            HttpClient Client = new HttpClient();
            String url = "http://198.245.61.226/kr4ken/dogmeat_memes.txt";
            HttpResponseMessage Response = await Client.GetAsync(url);
            string Content = await Response.Content.ReadAsStringAsync();
            return Content.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static async Task<String[]> DogmeatAnswersAsync()
        {
            HttpClient Client = new HttpClient();
            String url = "http://198.245.61.226/kr4ken/dogmeat_answers.txt";
            HttpResponseMessage Response = await Client.GetAsync(url);
            string Content = await Response.Content.ReadAsStringAsync();
            return Content.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static async Task<String> ResponsePickerAsync(String Content)
        {
            String[] Responses = await DogmeatResponsesAsync();
            Random r = new Random();

            if (Content.Contains("MASTER") ||
                Content.Contains("CREATOR") ||
                Content.Contains("DEVELOPER"))
                return "*I am reluctantly groomed by that faggot Kr4ken*";

            else if (Content.Contains("?"))
            {
                String[] Answers = await DogmeatAnswersAsync();
                return Answers[new Random().Next(0, Answers.Length)];
            }

            #region Mexican

            else if (Content.Contains("JUAN") ||
                Content.Contains("MEXICO") ||
                Content.Contains("MEXICAN") ||
                Content.Contains("EDUARDO") ||
                Content.Contains("TRUMP") ||
                Content.Contains("DONALD") ||
                Content.Contains("PRESIDENT"))
                return Responses[r.Next(19, 21)];

            #endregion Mexican

            #region Jew

            else if (Content.Contains("JEW"))
                return Responses[r.Next(16, 19)];

            #endregion Jew

            #region AA

            else if (Content.Contains("BLACK") ||
                Content.Contains("NIGGER"))
                return Responses[15];

            #endregion AA

            #region Clinton

            else if (Content.Contains("HILLARY") ||
                Content.Contains("CLINTON") ||
                Content.Contains("MEME QUEEN"))
                return Responses[9];

            #endregion Clinton

            #region Insult

            else if (Content.Contains("FUCK") ||
                Content.Contains("CUNT") ||
                Content.Contains("ASSHOLE") ||
                Content.Contains("DOUCHE") ||
                Content.Contains("KYS") ||
                Content.Contains("ROAST") ||
                Content.Contains("COCK"))
                return Responses[4];

            #endregion Insult

            #region LGBT

            else if (Content.Contains("GAY") ||
                Content.Contains("LESBIAN") ||
                Content.Contains("TRANS") ||
                Content.Contains("SEXUAL"))
                return Responses[5];

            #endregion LGBT

            #region Arya

            else if (Content.Contains("RACIS") ||
                Content.Contains("HITLER") ||
                Content.Contains("RACE"))
                return Responses[6];

            #endregion Arya

            #region Female

            else if (Content.Contains("WOMEN") ||
                Content.Contains("GIRL") ||
                Content.Contains("WOMAN") ||
                Content.Contains("GRILL") ||
                Content.Contains("VAGINA") ||
                Content.Contains("PUSSY"))
                return Responses[7];

            #endregion Female

            else
                return Responses[r.Next(0, Responses.Length)];
        }

        #endregion Variables

        #region Functions

        public static async Task AccessAsync(SocketMessage e)
        {
            (e.Author as SocketGuildUser).AddRoleAsync(Vars.PointBlank.GetRole(333334103858348033));
            e.DeleteAsync();
            Utilities.Log("Underwent Initiation", ((SocketGuildChannel)e.Channel).Guild, e.Author);
        }

        public static async Task WrongChannelAsync(SocketMessage e)
        {
            var channel = await e.Author.GetOrCreateDMChannelAsync();
            channel.SendMessageAsync("You are not permitted to chat in that channel.");
            e.DeleteAsync();
            Utilities.Log("Attempted to chat in a restricted channel.", ((SocketGuildChannel)e.Channel).Guild, e.Author);
        }

        public static async Task MentionedAsync(SocketMessage e)
        {
            e.Channel.SendMessageAsync(await Utilities.ResponsePickerAsync(e.Content.ToUpper()));
            Utilities.Log("Mentioned me.", ((SocketGuildChannel)e.Channel).Guild, e.Author);
        }

        #endregion Functions

        public static void MaintainConnection()
        {
            while (Vars.KeepAlive)
            {
                Thread.Sleep(1000);
                if (Vars.Client.ConnectionState == ConnectionState.Disconnected)
                {
                    Vars.Client.LoginAsync(TokenType.Bot, Vars.Token);
                    Vars.Client.StartAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(DateTime.Now + ": Dogmeat has disconnected and automagically reconnected.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(DateTime.Now + ": Client still connected.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Thread.Sleep(3599000);
            }
        }

        public static void AwaitInput()
        {
            Vars.Client.MessageReceived += async (msg) =>
            {
                if (msg.Channel.Id != Vars.Commands.Id || msg.Content.Contains("~") || msg.Author.Id == Vars.Client.CurrentUser.Id)
                    return;
                string Input = msg.Content;
                if (Input == null)
                    return;
                String[] Inputs = Input.Split('"')
                     .Select((element, index) => index % 2 == 0
                                           ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           : new string[] { element })
                     .SelectMany(element => element).ToArray();

                Log("Issued command " + Inputs[0], Vars.Main as SocketGuild, msg.Author);

                switch (Inputs[0].ToUpperInvariant())
                {
                    case "ANNOUNCE":
                    case "SAY":
                        ulong.TryParse(Inputs[1], out ulong ID);
                        String output = Inputs[2];

                        foreach (SocketGuild Guild in Vars.Client.Guilds)
                            foreach (SocketGuildChannel Channel in Guild.Channels)
                                if (Channel is SocketTextChannel && Channel.Id == ID)
                                    ((SocketTextChannel)Channel).SendMessageAsync(output);
                        break;
                    case "SHUTDOWN":
                    case "QUIT":
                    case "EXIT":
                        Shutdown();
                        break;
                    case "DISCONNECT":
                        Disconnect();
                        break;
                    case "RECONNECT":
                        Vars.KeepAlive = true;
                        Log("Dogmeat was revived.", ConsoleColor.Green);
                        break;
                    case "CANCEL":
                        Log("Shutdown canceled", ConsoleColor.Green);
                        ContinueShutdown = false;
                        break;
                    default:
                        Log("That is not a command.", ConsoleColor.Red);
                        break;
                }
            };
        }

        private static void Shutdown()
        {
            Vars.KeepAlive = false;
            ContinueShutdown = true;
            Log("Client is shutting down in five seconds.", ConsoleColor.Red);
            new Thread(() =>
            {
                if (!ContinueShutdown) return;
                Log("5", ConsoleColor.Red);
                Thread.Sleep(1000);
                if (!ContinueShutdown) return;
                Log("4", ConsoleColor.Red);
                Thread.Sleep(1000);
                if (!ContinueShutdown) return;
                Log("3", ConsoleColor.Red);
                Thread.Sleep(1000);
                if (!ContinueShutdown) return;
                Log("2", ConsoleColor.Red);
                Thread.Sleep(1000);
                if (!ContinueShutdown) return;
                Log("1", ConsoleColor.Red);
                Thread.Sleep(1000);
                if (!ContinueShutdown) return;
                Disconnect();
                Environment.Exit(0);
            }).Start();
        }

        private static void Disconnect()
        {
            Vars.KeepAlive = false;
            Vars.Client.StopAsync();
            Log("Dogmeat disconnected.", ConsoleColor.Red);
        }

        #region Logging

        public static void Log(String Message)
        {
            Log(Message, ConsoleColor.Gray);
        }

        public static void Log(String Message, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine(DateTime.Now + ": " + Message);
            Console.ResetColor();
            ((SocketTextChannel)Vars.Logging).SendMessageAsync(DateTime.Now + ": " + Message);
        }

        public static void Log(String Message, SocketGuild Guild, SocketUser User)
        {
            Log(Message, ConsoleColor.Gray, Guild, User);
        }

        public static void Log(String Message, ConsoleColor Color, SocketGuild Guild, SocketUser User)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine(DateTime.Now + ": [" + Guild.Name + "] " + User.Username + " " + Message);
            Console.ResetColor();
            ((SocketTextChannel)Vars.Logging).SendMessageAsync(DateTime.Now + ": [" + Guild.Name + "] " + User.Username + " " + Message);
        }

        #endregion Logging

        #region Users

        private static async Task<IGuildUser> GetUserByName(IGuild Guild, String Name)
        {
            IReadOnlyCollection<IGuildUser> users = await Guild.GetUsersAsync();
            foreach (IGuildUser user in users)
            {
                if (user.Username.Contains(Name))
                    return user;
                else if (user.Nickname != null && user.Nickname.Contains(Name))
                    return user;
            }
            return null;
        }

        private static async Task<IGuildUser> GetUserByID(IGuild Guild, String ID)
        {
            if (ulong.TryParse(ID, out ulong result))
                if (await Guild.GetUserAsync(result) != null)
                    return await Guild.GetUserAsync(result);
            return null;
        }

        public static async Task<IGuildUser> GetUser(IGuild Guild, String Input)
        {
            IGuildUser nameResult = await GetUserByName(Guild, Input);
            IGuildUser IDResult = await GetUserByID(Guild, Input);
            if (nameResult != null)
                return nameResult;
            else if (IDResult != null)
                return IDResult;
            else
                return null;
        }

        #endregion Users
    }
}