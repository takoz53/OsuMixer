using OsuSharp.Entities;
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
            FancyConsole.WriteLine("Setting up Mixer Connection", moduleName);
            MixerChatBase chatBase = new MixerChatBase();

            FancyConsole.WriteLine("Starting IRC.", moduleName);

            

            Console.ReadLine();
        }

    }
}
