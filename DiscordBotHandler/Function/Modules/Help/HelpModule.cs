using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Linq;

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
        public Task Help([Summary("Module Name")] string moduleName = null)
        {
            string moduleInfo = string.Empty;
            if (moduleName != null && moduleName.Trim().Length > 0)
            {
                var module = _cmS.Modules.First(i => i.Name == moduleName);
                if (module != null)
                {
                    foreach (var command in module.Commands)
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
                }
                else
                {
                    moduleInfo = $"Not Found {moduleName}";
                }
            }
            else
            {
                foreach (var item in _cmS.Modules)
                {
                    moduleInfo += item.Name + /*" - " + item.Summary +*/ Environment.NewLine;
                    moduleInfo += Environment.NewLine;
                }
            }
            ReplyAsync(moduleInfo);
            return Task.CompletedTask;
        }
    }
}
