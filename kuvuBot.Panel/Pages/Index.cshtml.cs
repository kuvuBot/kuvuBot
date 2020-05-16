using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages
{
    public class IndexModel : PageModel
    {
        public string Lang { get; set; }
        public void OnGet()
        {
            Lang = Request.Cookies["lang"] ?? "en";
        }
    }
}
