using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;

namespace kuvuBot.Commands.Attributes
{
    public enum RankCheckMode { Minimum, Equal, Maximum }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RequireGlobalRankAttribute : CheckBaseAttribute
    {
        public KuvuGlobalRank Rank { get; }

        public RankCheckMode CheckMode { get; }

        public RequireGlobalRankAttribute(KuvuGlobalRank rank, RankCheckMode checkMode = RankCheckMode.Minimum)
        {
            this.CheckMode = checkMode;
            this.Rank = rank;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null || ctx.Member == null)
                return false;

            var globalUser = await ctx.User.GetGlobalUser();

            switch (this.CheckMode)
            {
                case RankCheckMode.Equal:
                    return globalUser.GlobalRank == Rank;

                case RankCheckMode.Maximum:
                    return globalUser.GlobalRank <= Rank;

                case RankCheckMode.Minimum:
                    return globalUser.GlobalRank >= Rank;
            }
            return false;
        }
    }
}
