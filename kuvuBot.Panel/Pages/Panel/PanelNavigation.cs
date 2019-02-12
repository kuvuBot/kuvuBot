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

        public NavigationItem(string text, NavigationItem parent = null)
        {
            Text = text;
            Parent = parent;
        }
    }

    public static class PanelNavigation
    {
        public static NavigationItem Dashboard => new NavigationItem("Dashboard");
        public static NavigationItem GuildManage => new NavigationItem("Guild");

        public static SidebarItem[] GuildSidebar => new[]
        {
            new SidebarItem("Information", "fa fa-server", "", GuildManage),
            new SidebarItem("Members", "fas fa-users"),
            new SidebarItem("Configuration", "fas fa-cogs"),
        };

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
