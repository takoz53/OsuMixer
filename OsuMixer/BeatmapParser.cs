using OsuSharp;
using OsuSharp.Endpoints;
using OsuSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpOsu;
using CSharpOsu.Module;
using System.IO;

namespace OsuMixer
{
    //Either Remove CSharpOsu because no async funcs, or remove OsuSharp because no way to get beatmap sets
    class BeatmapParser {
        private IOsuApi Instance { get; }
        public static readonly string moduleName = "Beatmap Parser";
        //OsuClient OsuClient;
        public BeatmapParser () {
            IOsuSharpConfiguration IConfig = new OsuSharpConfiguration() {
                ApiKey = File.ReadAllText("osuApiKey.txt"),
                LogLevel = OsuSharp.Enums.LoggingLevel.Info,
                ModsSeparator = "|"
            };
            Instance = new OsuApi(IConfig);
            //OsuClient = new OsuClient(apiKey);
        }


        /*public OsuBeatmap newGetInfo (string link) {
            bool isSet = false;
            Regex setRegex = new Regex(@"^(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/([0-9]{1,19})$", RegexOptions.Compiled);
            Regex beatmapRegex = new Regex(@"(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/[0-9]{1,19}#(osu|fruits|mania|taiko)\/([0-9]{1,19})", RegexOptions.Compiled);
            System.Text.RegularExpressions.Match beatmapMatch = beatmapRegex.Match(link);
            System.Text.RegularExpressions.Match setMatch = setRegex.Match(link);
            OsuBeatmap beatmap = null;

            if (!beatmapMatch.Success && !setMatch.Success) {
                return null;
            } else if (setMatch.Success) {
                isSet = true;
            }
            //It is a specific Beatmap.
            if (!isSet) {
                long beatmapID = Convert.ToInt64(beatmapMatch.Groups[3].Value);
                FancyConsole.WriteLine($"Beatmap with mode found!\nID: {beatmapID}");

                try {
                    beatmap = OsuClient.GetBeatmap(beatmapID, false).First();
                } catch {
                    return null;
                }
            } else { //It is a beatmapset. To be edited. No real way to get which Gamemode it is.
                long beatmapID = Convert.ToInt64(setMatch.Groups[2].Value);
                FancyConsole.WriteLine($"Beatmapset found!\nSet ID: {beatmapID}");
                try {
                    beatmap = OsuClient.GetBeatmap(beatmapID, true).First();
                } catch {
                    return null;
                }
            }
            return beatmap;
        }*/

        public async Task<Beatmap> GetBeatmapInfo (string link) {
            Beatmap beatmap = null;
            bool isSet = false;
            Regex setRegex = new Regex(@"^(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/([0-9]{1,19})$");
            Regex beatmapRegex = new Regex(@"(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/[0-9]{1,19}#(osu|fruits|mania|taiko)\/([0-9]{1,19})");
            System.Text.RegularExpressions.Match beatmapMatch = beatmapRegex.Match(link);
            System.Text.RegularExpressions.Match setMatch = setRegex.Match(link);

            if (!beatmapMatch.Success && !setMatch.Success) {
                return null;
            } else if (setMatch.Success) {
                isSet = true;
            }
            //It is a specific Beatmap.
            if (!isSet) {
                string mode = beatmapMatch.Groups[2].Value;
                long beatmapID = Convert.ToInt64(beatmapMatch.Groups[3].Value);
                FancyConsole.WriteLine($"Beatmap with Mode: {mode}, ID: {beatmapID} found.", moduleName);

                try {
                    if (mode == "osu") {
                        beatmap = await Instance.GetBeatmapAsync(beatmapID);
                    } else if (mode == "taiko") {
                        beatmap = await Instance.GetBeatmapAsync(beatmapID, OsuSharp.Enums.BeatmapType.ByDifficulty, OsuSharp.Enums.GameMode.Taiko);
                    } else if (mode == "mania") {
                        beatmap = await Instance.GetBeatmapAsync(beatmapID, OsuSharp.Enums.BeatmapType.ByDifficulty, OsuSharp.Enums.GameMode.Mania);
                    } else if (mode == "fruits") {
                        beatmap = await Instance.GetBeatmapAsync(beatmapID, OsuSharp.Enums.BeatmapType.ByDifficulty, OsuSharp.Enums.GameMode.Catch);
                    }
                } catch {
                    return null;
                }
            } else { //It is a beatmapset. To be edited. No real way to get which Gamemode it is.
                long beatmapID = Convert.ToInt64(setMatch.Groups[2].Value);
                FancyConsole.WriteLine($"Beatmapset found with Set ID: {beatmapID}", moduleName);
                try {
                    beatmap = Instance.GetBeatmap(beatmapID, OsuSharp.Enums.BeatmapType.ByBeatmap, OsuSharp.Enums.GameMode.Standard);
                } catch {
                    return null;
                }
            }
            return beatmap;
        }
    }
}
