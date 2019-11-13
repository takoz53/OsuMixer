using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMixer {
    static class Config {
        public static readonly string moduleName = "Config";
        private static bool osuApi = false;
        private static bool mixerClientID = false;
        private static bool osuIRC = false;
        private static readonly string mixerPath = "Config/mixerClientID.txt";
        private static readonly string osuPath = "Config/osuApiKey.txt";
        private static readonly string ircPath = "Config/ircIDPW.txt";
        private static readonly string configPath = "Config";
        private static void CheckOsu () {
            if (!File.Exists(osuPath)) {
                File.Create(osuPath).Dispose();
                FancyConsole.WriteLine("Fill in the osu Api Key and restart the program", moduleName, FancyConsole.LogSeverity.Warning);
                osuApi = true;
            }
        }
        private static void CheckIRC () {
            if (!File.Exists(ircPath)) {
                File.Create(ircPath).Dispose();
                FancyConsole.WriteLine("For IRC: Line 1 must be ID, Line 2 Server Password, Line 3 Channel", moduleName, FancyConsole.LogSeverity.Warning);
                osuIRC = true;
            }
        }

        private static void CheckMixer () {
            if (!File.Exists(mixerPath)) {
                File.Create(mixerPath).Dispose();
                FancyConsole.WriteLine("Fill in the mixer client ID and restart the program", moduleName, FancyConsole.LogSeverity.Warning);
                mixerClientID = true;
            }
            
        }
        private static void CheckFolder () {
            if (!Directory.Exists(configPath)) {
                Directory.CreateDirectory(configPath);
                Task.Delay(500);
            }
        }
        public static bool DependenciesSet () {
            CheckFolder();
            CheckOsu();
            CheckMixer();
            CheckIRC();
            return osuApi && mixerClientID && osuIRC;
        }
    }
}
