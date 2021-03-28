using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
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
        public Task Help()
        {
            foreach(var item in _cmS.Modules)
            {
                string moduleInfo = item.Name + /*" - " + item.Summary +*/ Environment.NewLine;
                foreach(var command in item.Commands)
                {
                    moduleInfo +="  " + command.Name + ":" + Environment.NewLine;
                    moduleInfo+="    "+command.Summary+ Environment.NewLine;
                    if (command.Parameters.Count > 0)
                    {
                        moduleInfo += "    Params:" + Environment.NewLine;
                    }
                    foreach(var _params in command.Parameters)
                    {
                        moduleInfo += "    " + _params.Name + (!string.IsNullOrEmpty(_params.Summary) ? " - "+_params.Summary : "") + Environment.NewLine;
                    }
                }
                moduleInfo += Environment.NewLine;
                ReplyAsync(moduleInfo);
            }
            return Task.CompletedTask;
        }
    }
}
