using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace kuvuBot.Panel.Pages
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/login"), HttpPost("~/login")]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties {RedirectUri = "/"}, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("~/logout"), HttpPost("~/logout")]
        public IActionResult Logout()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}