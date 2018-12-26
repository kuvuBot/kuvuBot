using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using kuvuBot.Lang;

namespace kuvuBot.Commands
{
    public static class CommandUtils
    {
        public async static Task<bool> HasPermission(this CommandContext ctx, Permissions permission, bool respond = true)
        {
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(permission))
            {
                return true;
            }
            if (respond)
                await ctx.RespondAsync(await ctx.Lang("global.nopermission"));
            return false;
        }
    }
}
