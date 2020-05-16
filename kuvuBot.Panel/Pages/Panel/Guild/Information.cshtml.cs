using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    public class IndexModel : BaseGuildModel
    {
        public async Task<ActionResult> OnGetAsync(string id)
        {
            var result = await MakeResult(id);
            ViewData["ActivePage"] = GuildManage;
            return result;
        }
    }
}