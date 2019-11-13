using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMixer {
    static class FancyConsole {
        public enum LogSeverity {
            Normal,
            Error,
            Warning
        }
        public static void WriteLine (string message, string module = "", LogSeverity logSeverity = LogSeverity.Normal) {
            if (logSeverity == LogSeverity.Normal) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("{" + module + "}[" + DateTime.Now.ToShortTimeString() + "]: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            } else if (logSeverity == LogSeverity.Error) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{" + module + "}[" + DateTime.Now.ToShortTimeString() + "] - ERROR: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            } else if (logSeverity == LogSeverity.Warning) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("{" + module + "}[" + DateTime.Now.ToShortTimeString() + "] - WARNING: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            }
        }
    }
}
