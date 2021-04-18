using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Help
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _cmS;
        public HelpModule(CommandService cmS)
        {
            _cmS = cmS;
        }
        [Command("help")]
        [Summary("Command list helper")]
        public Task Help([Summary("The name of the module for which you need help")] string moduleName = null)
        {
            if (moduleName == null)
            {
                foreach (var item in _cmS.Modules)
                {
                    string moduleInfo = item.Name + (!string.IsNullOrEmpty(item.Summary) ? " - " + item.Summary : "") + Environment.NewLine;
                    moduleInfo += Environment.NewLine;
                    ReplyAsync(moduleInfo);
                }
            }
            else
            {
                var item = _cmS.Modules.FirstOrDefault(m => m.Name.ToLower() == moduleName.ToLower());
                if(item == null)
                {
                    ReplyAsync("Module not found");
                }
                string moduleInfo = item.Name + (!string.IsNullOrEmpty(item.Summary) ? " - " + item.Summary : "") + Environment.NewLine;
                foreach (var command in item.Commands)
                {
                    moduleInfo += "  " + command.Name + ":" + Environment.NewLine;
                    moduleInfo += "    " + command.Summary + Environment.NewLine;
                    if (command.Parameters.Count > 0)
                    {
                        moduleInfo += "    Params:" + Environment.NewLine;
                    }
                    foreach (var _params in command.Parameters)
                    {
                        moduleInfo += "    " + _params.Name + (!string.IsNullOrEmpty(_params.Summary) ? " - " + _params.Summary : "") + Environment.NewLine;
                    }
                }
                moduleInfo += Environment.NewLine;
                ReplyAsync(moduleInfo);
            }
            return Task.CompletedTask;
        }
    }
}
