using DiscordBotHandler.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotHandler.Logger
{
    class Logger: ILogger
    {
        string log_file;
        public Logger()
        {
            string directory = "log";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string log_file_name = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds + ".log";
            log_file = Path.Combine(directory, log_file_name);
            if (!File.Exists(log_file))
            {
                File.Create(log_file);
            }
            /*string directory = ConfigurationManager.AppSettings["LogPath"];
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string log_file_name = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds + ".log";
            log_file = Path.Combine(directory, log_file_name);
            if (!File.Exists(log_file))
            {
                File.Create(log_file);
            }*/
        }
        public async Task<Task> LogMessage(string message)
        {
            //Console.WriteLine(message);
            using (StreamWriter sw = File.AppendText(log_file))
            {
                await sw.WriteLineAsync(message);
            }
            return Task.CompletedTask;
        }
    }
}
