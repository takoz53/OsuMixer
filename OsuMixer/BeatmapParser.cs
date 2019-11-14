using OsuSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace OsuMixer
{
    //Either Remove CSharpOsu because no async funcs, or remove OsuSharp because no way to get beatmap sets
    class BeatmapParser : IDisposable {
        private OsuClient Client { get; set; }
        public static readonly string moduleName = "Beatmap Parser";
        public static readonly Regex setRegex = new Regex(@"^(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/([0-9]{1,19})\/?$");
        public static readonly Regex beatmapRegex = new Regex(@"(https:\/\/|http:\/\/)?osu.ppy.sh\/beatmapsets\/[0-9]{1,19}#(osu|fruits|mania|taiko)\/([0-9]{1,19})\/?");
        //OsuClient OsuClient;
        public BeatmapParser () {
            var config = new OsuSharpConfiguration() {
                ApiKey = File.ReadAllText(Config.osuPath),
                ModeSeparator = "|"
            };
            Client = new OsuClient(config);
        }
        public async Task<Beatmap> GetBeatmapInfo (string link) {
            Beatmap beatmap;
            bool isSet = false;
            Match beatmapMatch = beatmapRegex.Match(link);
            Match setMatch = setRegex.Match(link);

            if (!beatmapMatch.Success && !setMatch.Success) {
                return null;
            } else if (setMatch.Success) {
                isSet = true;
            }
            //It is a specific Beatmap.
            if (!isSet) {
                long beatmapID = Convert.ToInt64(beatmapMatch.Groups[3].Value);
                FancyConsole.WriteLine($"Beatmap with ID: {beatmapID} found.", moduleName);

                try {
                    beatmap = await Client.GetBeatmapByIdAsync(beatmapID);
                } catch (OsuSharpException e) {
                    return null;
                }
            } else { //It is a beatmapset.
                long beatmapID = Convert.ToInt64(setMatch.Groups[2].Value);
                FancyConsole.WriteLine($"Beatmapset found with Set ID: {beatmapID}", moduleName);
                try {
                    var beatmaps = await Client.GetBeatmapsetAsync(beatmapID);
                    beatmap = beatmaps.OrderByDescending(x => x.DifficultyRating).First();
                } catch (OsuSharpException e) {
                    return null;
                }
            }
            return beatmap;
        }

        public void Dispose () {
            Client = null;
        }
    }
}
