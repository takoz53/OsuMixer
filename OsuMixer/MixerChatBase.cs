using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.OAuth;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMixer
{
    class MixerChatBase {
        public readonly string moduleName = "Mixer Chat";
        private readonly MixerConnection connection;
        private static ChatClient chatClient;
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
            var clientID = File.ReadAllText("mixerClientID.txt");
            connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes).Result;
            UserModel user = connection.Users.GetCurrentUser().Result;
            ExpandedChannelModel channel = connection.Channels.GetChannel(user.username).Result;

            chatClient = ChatClient.CreateFromChannel(connection, channel).Result;
            chatClient.Connect();
            if (chatClient.Connected) {
                chatClient.SendMessage("I connected uwu");
            } else {
                FancyConsole.WriteLine("Task failed successfully owo", moduleName, FancyConsole.LogSeverity.Error);
            }
        }

        public void SendChatMessage (string message) {
            chatClient.SendMessage(message);
        }
    }
}
