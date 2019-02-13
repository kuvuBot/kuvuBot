using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    public class ConfigurationModel : BaseGuildModel
    {
        public override Task<ActionResult> OnGetAsync(string id)
        {
            if (Request.GetDisplayUrl().EndsWith("/"))
                Response.Redirect(Request.GetEncodedUrl().TrimEnd('/'));
            return base.OnGetAsync(id);
        }
    }
}