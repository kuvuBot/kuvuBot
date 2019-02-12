using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public DiscordRestClient Client;
        public BotContext _BotContext;
        public async Task OnGetAsync()
        {
            _BotContext = new BotContext();
            Client = await HttpContext.GetRestClient();
        }
    }
}