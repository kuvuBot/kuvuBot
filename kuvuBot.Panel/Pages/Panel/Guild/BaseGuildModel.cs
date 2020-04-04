using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using kuvuBot.Commands;
using kuvuBot.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    [Authorize]
    public class BaseGuildModel : PageModel
    {
        public DiscordRestClient RestClient;
        public DiscordShardedClient BotClient;
        public BotContext BotContext;

        public bool Global = false;
        public GlobalUser GlobalUser;

        public KuvuGuild Guild;
        public DiscordGuild DiscordGuild;
        public DiscordGuild RestGuild;

        public NavigationItem GuildManage;

        public virtual async Task<ActionResult> MakeResult([FromQuery] string id)
        {
            BotContext = new BotContext();
            RestClient = await HttpContext.GetRestClient();
            BotClient = kuvuBot.Program.Client;

            if (ulong.TryParse(id, out var guildId))
            {
                Guild = BotContext.Guilds.FirstOrDefault(x => x.GuildId == guildId);
                if (Guild != null)
                {
                    DiscordGuild = await BotClient.GetGuildAsync(Guild.GuildId);
                    RestGuild = RestClient.Guilds.Values.FirstOrDefault(x=>x.Id == Guild.GuildId);
                }
                if (Guild == null || DiscordGuild == null || RestGuild == null || !RestGuild.Permissions.HasValue || !RestGuild.Permissions.Value.HasPermission(Permissions.ManageGuild))
                {
                    return Unauthorized();
                }
            }
            else if (id == "global")
            {
                GlobalUser = await RestClient.CurrentUser.GetGlobalUser(BotContext);
                if (GlobalUser.GlobalRank >= KuvuGlobalRank.Admin)
                {
                    Global = true;
                }
                else
                {
                    return Unauthorized();
                }
            }

            GuildManage = PanelNavigation.GuildManage(id, DiscordGuild.Name);
            ViewData.AddToSidebar(new[]
            {
                new SidebarHeader("Guild management"),
                new SidebarItem("Information", "fas fa-info-circle", "/Panel/Guild/Information", GuildManage),
                new SidebarItem("Members", "fas fa-users", "/Panel/Guild/Members", page: PanelNavigation.Members(GuildManage)),
                new SidebarItem("Configuration", "fas fa-cogs", "/Panel/Guild/Configuration", page: PanelNavigation.Configuration(GuildManage)),
            });
            return Page();
        }
    }
}
