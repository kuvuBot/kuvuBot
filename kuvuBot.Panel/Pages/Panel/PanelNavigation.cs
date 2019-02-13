using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace kuvuBot.Panel.Pages.Panel
{
    public class SidebarItem
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public NavigationItem Page { get; set; }

        public SidebarItem(string text, string icon, string href = null, NavigationItem page = null)
        {
            Text = text;
            Icon = icon;
            Href = href ?? text;
            Page = page ?? new NavigationItem(Href);
        }
    }

    public class NavigationItem
    {
        public NavigationItem Parent { get; set; }
        public string Text { get; set; }
        public string Href { get; set; }

        public NavigationItem(string text, NavigationItem parent = null, string href = null)
        {
            Text = text;
            Parent = parent;
            Href = parent?.Href + href ?? text;
        }
    }

    public static class PanelNavigation
    {
        // Panel/
        public static NavigationItem Guilds => new NavigationItem("My guilds");

        // Panel/Guild/{id}/
        public static NavigationItem GuildManage(string id, string text) => new NavigationItem(text, href: $"/Panel/Guild/{id}/");
        public static NavigationItem Members(NavigationItem guildManage) => new NavigationItem("Members", guildManage);
        public static NavigationItem Configuration(NavigationItem guildManage) => new NavigationItem("Configuration", guildManage);
       

        public static NavigationItem ActivePage(ViewContext viewContext)
        {
            return viewContext.ViewData["ActivePage"] as NavigationItem
                             ?? new NavigationItem(System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName));
        }
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            return string.Equals(ActivePage(viewContext).Text, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
