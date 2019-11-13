using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;
namespace OsuMixer {
    class OsuIRC {
        private static IrcClient ircClient;
        private static readonly string moduleName = "IRC Client";
        private static readonly string serverIP = "irc.ppy.sh";
        private static readonly int port = 6667;
        private static string userName;
        private static string serverPass;
        public static string channel;
        public OsuIRC () {
            string[] information = File.ReadAllLines(Config.ircPath);
            try {
                userName = information[0];
                serverPass = information[1];
                channel = information[2];
                ircClient = new IrcClient();
            } catch {
                FancyConsole.WriteLine("Could not get either username, password or channel!", moduleName, FancyConsole.LogSeverity.Error);
                return;
            }

            Connect();
            new Thread (new ThreadStart(Listen)).Start();
        }

        //Not sure which connect function I need to use so I connect with everything at once lmao.
        private void Connect () {
            ircClient.Connect(serverIP, port);
            ircClient.Login(userName, userName, 1, userName, serverPass);
            ircClient.RfcPass(serverPass);
            ircClient.RfcUser(userName, 1, userName);
            ircClient.RfcJoin(channel);
        }
        private void Listen () {
            ircClient.OnConnected += IrcClient_OnConnected;
            ircClient.OnDisconnected += IrcClient_OnDisconnected;
            ircClient.Listen();
        }

        private void IrcClient_OnDisconnected (object sender, EventArgs e) {
            FancyConsole.WriteLine("Disconnected! Trying to reconnect.", moduleName, FancyConsole.LogSeverity.Error);
            Connect();
        }

        private void IrcClient_OnConnected (object sender, EventArgs e) {
            FancyConsole.WriteLine("Connected.", moduleName);
        }

        public void SendChatMessage(string message, string channel) {
            ircClient.SendMessage(SendType.Message, channel, message);
        }
    }
}
