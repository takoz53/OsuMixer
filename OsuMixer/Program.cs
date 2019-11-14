using System;
using System.Threading.Tasks;

namespace OsuMixer
{
    class Program
    {
        static void Main() => MainAsync().GetAwaiter().GetResult();
        private static readonly string moduleName = "Main";
        private static MixerChatBase chatBase;
        public static async Task MainAsync()
        {
            if (Config.DependenciesSet()) {
                FancyConsole.WriteLine("Press Enter to close Program.", moduleName);
                Console.ReadLine();
                return;
            }
            FancyConsole.WriteLine("Setting up Mixer Connection", moduleName);
            chatBase = new MixerChatBase();
            

            

            Console.ReadLine();
        }

    }
}
