using System;
using System.Collections.Generic;

namespace Service
{
    public class Log
    {
        public enum LogType
        {
            Banner,
            Message,
            Error
        }

        private static readonly Dictionary<LogType, ConsoleColor> _cc = new Dictionary<LogType, ConsoleColor> {
            { LogType.Message, ConsoleColor.White },
            { LogType.Banner, ConsoleColor.DarkGray },
            { LogType.Error, ConsoleColor.Red }
        };

        public static void Prt(string text, LogType logType)
        {
            var typeStr = logType == LogType.Banner ? string.Empty : 
                $"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff")}] [{logType}]: ";
            Console.ForegroundColor = _cc[logType];
            Console.WriteLine($"{typeStr}{text}");
        }
    }
}
