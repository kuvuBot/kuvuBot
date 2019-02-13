using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using kuvuBot.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    public class IndexModel : BaseGuildModel
    {
        public override async Task<ActionResult> OnGetAsync(string id)
        {
            if (!Request.GetDisplayUrl().EndsWith("/"))
                Response.Redirect(Request.GetEncodedUrl() + "/");

            var result = await base.OnGetAsync(id);
            ViewData["ActivePage"] = GuildManage;
            return result;
        }
    }
}