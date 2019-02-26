using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using kuvuBot.Panel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    public class MembersModel : BaseGuildModel
    {
        public int PaginationPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task<ActionResult> OnGetAsync(string id)
        {
            if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"].FirstOrDefault(), out var page))
                PaginationPage = page;
            if (Request.Query.ContainsKey("pageSize") && int.TryParse(Request.Query["pageSize"].FirstOrDefault(), out var pageSize))
                PageSize = pageSize;

            var result = await MakeResult(id);
            ViewData["ActivePage"] = PanelNavigation.Members(GuildManage);
            return result;
        }

        public async Task<IActionResult> OnPostResetAllLevelsAsync([FromRoute] string id)
        {
            if (await MakeResult(id) is UnauthorizedResult)
            {
                return Unauthorized();
            }

            foreach(var kuvuUser in _BotContext.Users.Where(x=>x.Guild.GuildId == DGuild.Id))
            {
                kuvuUser.Exp = 0;
                kuvuUser.LastExpMessage = null;
            }
            await _BotContext.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetLevelAsync([FromRoute] string id, [FromForm] int userId)
        {
            if (await MakeResult(id) is UnauthorizedResult)
            {
                return Unauthorized();
            }

            var kuvuUser = _BotContext.Users.Find(userId);
            if (kuvuUser.Guild.GuildId != DGuild.Id)
                return Unauthorized();
            kuvuUser.Exp = 0;
            kuvuUser.LastExpMessage = null;
            await _BotContext.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}