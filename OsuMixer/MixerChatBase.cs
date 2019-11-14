using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using OsuSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OsuMixer {
    class MixerChatBase {
        public readonly string moduleName = "Mixer Chat";
        private readonly MixerConnection connection;
        private static ChatClient chatClient;
        private static OsuIRC osuIRC;
        readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
            {
                OAuthClientScopeEnum.chat__bypass_links,
                OAuthClientScopeEnum.chat__bypass_slowchat,
                OAuthClientScopeEnum.chat__chat,
                OAuthClientScopeEnum.chat__connect,
                OAuthClientScopeEnum.chat__remove_message,
                OAuthClientScopeEnum.chat__view_deleted,
            };
        public MixerChatBase () {
            var clientID = File.ReadAllText(Config.mixerPath);
            try {
                connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes).Result;
            } catch (RestServiceRequestException e) {
                FancyConsole.WriteLine(e.Content, moduleName, FancyConsole.LogSeverity.Error);
            }
            FancyConsole.WriteLine("Starting IRC.", moduleName);
            osuIRC = new OsuIRC();
            osuIRC.SendChatMessage("takoBot has been activated.", OsuIRC.channel);

            UserModel user = connection.Users.GetCurrentUser().Result;
            ExpandedChannelModel channel = connection.Channels.GetChannel(user.username).Result;
            chatClient = ChatClient.CreateFromChannel(connection, channel).Result;

            if (chatClient.Connect().Result && chatClient.Authenticate().Result) {
                FancyConsole.WriteLine("Connection established to " + chatClient.User.username, moduleName);
                chatClient.SendMessage("Connection established.");
                new Thread(new ThreadStart(ListenMessages)).Start();
            }
        }

        private void ListenMessages () {
            chatClient.OnMessageOccurred += ChatClient_OnMessageOccurred;
        }
        private async void ChatClient_OnMessageOccurred (object sender, ChatMessageEventModel e) {
            string message = e.message.message[e.message.message.Length - 1].text;
            string user = e.user_name;
            FancyConsole.WriteLine($"{user}: {message}", moduleName);
            var isBeatmap = BeatmapParser.beatmapRegex.Match(message);
            var isSet = BeatmapParser.setRegex.Match(message);
            if (!isBeatmap.Success && !isSet.Success) {
                return;
            }

            BeatmapParser beatmapParser = new BeatmapParser();
            Beatmap beatmap = await beatmapParser.GetBeatmapInfo(message);

            if (beatmap == null) {
                await chatClient.SendMessage("Beatmap not found!");
                FancyConsole.WriteLine("Beatmap not found!", BeatmapParser.moduleName, FancyConsole.LogSeverity.Error);
                return;
            }

            
            string outputOsu = FormatBeatmapOsu(user, beatmap, message);
            string outputChat = FormatBeatmapChat(user, beatmap);
            await chatClient.SendMessage(outputChat);
            osuIRC.SendChatMessage(outputOsu, OsuIRC.channel);
            beatmapParser.Dispose();
        }

        private static string FormatBeatmapOsu (string user, Beatmap beatmap, string link) {
            double difficulty = Math.Round(beatmap.DifficultyRating, 2);
            string formattedBeatmap = $"{user}> [{link} {beatmap.Artist}[{beatmap.Difficulty}]] | {difficulty}* | {beatmap.GameMode}";
            return formattedBeatmap;
        }

        private static string FormatBeatmapChat (string user, Beatmap beatmap) {
            double difficulty = Math.Round(beatmap.DifficultyRating, 2);
            string formattedBeatmap = $"{user}> {beatmap.Artist}[{beatmap.Difficulty}] | {difficulty}* | {beatmap.GameMode}";
            return formattedBeatmap;
        }
    }
}
