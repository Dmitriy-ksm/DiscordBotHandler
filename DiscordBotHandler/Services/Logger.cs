using DiscordBotHandler.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotHandler.Logger
{
    class Logger : ILogger
    {
        const string directory = "log";
        string _logFile;
        string LogFile {
            get
            {
                if (_logFile == null)
                {
                    _logFile = Path.Combine(directory,
                           (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds + ".log");

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    if (!File.Exists(_logFile))
                        File.Create(_logFile);
                } 
                return _logFile;
            }
        }
        public async Task<Task> LogMessage(string message)
        {
            using (StreamWriter sw = File.AppendText(LogFile))
            {
                await sw.WriteLineAsync(message);
            }
            return Task.CompletedTask;
        }
    }
}
