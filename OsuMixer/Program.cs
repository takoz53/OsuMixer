using CSharpOsu.Module;
using OsuSharp.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMixer
{
    class Program
    {
        static void Main() => MainAsync().GetAwaiter().GetResult();
        private static readonly string moduleName = "Main";
        public static async Task MainAsync()
        {
            if (Config.DependenciesSet()) {
                FancyConsole.WriteLine("Press Enter to close Program.", moduleName);
                Console.ReadLine();
                return;
            }
            FancyConsole.WriteLine("Starting Beatmap Parser", moduleName);
            BeatmapParser beatmapParser = new BeatmapParser();
            FancyConsole.WriteLine("Setting up Mixer Connection", moduleName);
            //MixerChatBase chatBase = new MixerChatBase();
            FancyConsole.WriteLine("Starting IRC.", moduleName);
            OsuIRC osuIRC = new OsuIRC();
            osuIRC.SendChatMessage("Test.", "takoz53");
            Beatmap beatmap = await beatmapParser.GetBeatmapInfo("https://osu.ppy.sh/beatmapsets/757291#osu/1707563");
            if(beatmap == null)
            {
              //chatBase.SendChatMessage("Beatmap not found! Is it a Beatmapset?");
                FancyConsole.WriteLine("No beatmap available", BeatmapParser.moduleName, FancyConsole.LogSeverity.Error);
                Console.ReadLine();
                return;
            }

            string result = FormatBeatmap(beatmap);
            FancyConsole.WriteLine(result, BeatmapParser.moduleName);
            Console.ReadLine();
        }


        private static string FormatBeatmap(Beatmap beatmap)
        {
            double difficulty = Math.Round(beatmap.DifficultyRating, 2);
            string formattedBeatmap = $"takoz53: {beatmap.Artist}[{beatmap.Difficulty}] | {difficulty}* NoMod | {beatmap.GameMode}";
            return formattedBeatmap;
        }
    }
}
