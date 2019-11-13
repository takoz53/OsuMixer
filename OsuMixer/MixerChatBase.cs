using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.OAuth;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using OsuSharp.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            try { connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes).Result; } catch (RestServiceRequestException e) {

            }
            osuIRC = new OsuIRC();
            osuIRC.SendChatMessage("takoBot has been activated. Send me maps.", OsuIRC.channel);
            UserModel user = connection.Users.GetCurrentUser().Result;
            ExpandedChannelModel channel = connection.Channels.GetChannel(user.username).Result;

            chatClient = ChatClient.CreateFromChannel(connection, channel).Result;
            chatClient.Connect();
            if (chatClient.Connected) {
                new Thread(new ThreadStart(ListenMessages)).Start();
                chatClient.SendMessage("Connection established.");
            }
        }

        private void ListenMessages () {
            chatClient.OnMessageOccurred += ChatClient_OnMessageOccurred;
        }
        private async void ChatClient_OnMessageOccurred (object sender, ChatMessageEventModel e) {
            var isBeatmap = BeatmapParser.beatmapRegex.Match(e.message.ToString());
            var isSet = BeatmapParser.setRegex.Match(e.message.ToString());
            if (!isBeatmap.Success || !isSet.Success) {
                return;
            }

            BeatmapParser beatmapParser = new BeatmapParser();
            Beatmap beatmap = await beatmapParser.GetBeatmapInfo(e.message.ToString());

            if (beatmap == null) {
                await chatClient.SendMessage("Beatmap not found! Is it a Beatmapset?");
                FancyConsole.WriteLine("No beatmap available", BeatmapParser.moduleName, FancyConsole.LogSeverity.Error);
                return;
            }

            string output = FormatBeatmap(e.user_name, beatmap);
            osuIRC.SendChatMessage(output, OsuIRC.channel);
        }

        private static string FormatBeatmap (string user, Beatmap beatmap) {
            double difficulty = Math.Round(beatmap.DifficultyRating, 2);
            string formattedBeatmap = $"{user}> {beatmap.Artist}[{beatmap.Difficulty}] | {difficulty}* | {beatmap.GameMode}";
            return formattedBeatmap;
        }
    }
}
