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
        private static readonly string mixerPath = "Config/mixerClientID.txt";
        private static readonly string osuPath = "Config/osuApiKey.txt";
        private static readonly string configPath = "Config";
        private static void CheckOsu () {
            if (!File.Exists(osuPath)) {
                File.Create(osuPath);
                FancyConsole.WriteLine("Fill in the osu Api Key and restart the program", moduleName, FancyConsole.LogSeverity.Warning);
                osuApi = true;
            }
            
        }
        
        private static void CheckMixer () {
            if (!File.Exists(mixerPath)) {
                File.Create(mixerPath);
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
            return osuApi && mixerClientID;
        }
    }
}
