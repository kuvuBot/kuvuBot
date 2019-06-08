using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using kuvuBot.Commands.Moderation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    public class ConfigurationModel : BaseGuildModel
    {
        public async Task<ActionResult> OnGetAsync(string id)
        {
            var result = await MakeResult(id);
            ViewData["ActivePage"] = PanelNavigation.Configuration(GuildManage);
            return result;
        }

        public async Task<IActionResult> OnPostSaveAsync([FromRoute] string id, [FromForm] Dictionary<string, string> config)
        {
            if (await MakeResult(id) is UnauthorizedResult)
            {
                return Unauthorized();
            }

            foreach (var (key, value) in config)
            {
                var option = ConfigCommandGroup.Option.Options.FirstOrDefault(x => x.Name == key);
                if (option != null)
                {
                    await option.SetValue(Guild, value, false);
                }
            }

            await BotContext.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}