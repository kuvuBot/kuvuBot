using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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
