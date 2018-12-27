using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Colorful;
using DSharpPlus.Entities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace kuvuBot.Commands.Fun
{
    public class EmojiTextCommand : BaseCommandModule
    {
        string[] nums = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        [Command("emojitext"), Description("Generates a emoji text"), Aliases("emtext")]
        public async Task EmojiText(CommandContext ctx, [RemainingText] string message)
        {
            string formatted = "";
            foreach (var c in message)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    formatted += $":regional_indicator_{char.ToLower(c)}:";
                }
                else if (c >= '0' && c <= '9')
                {
                    formatted += $":{nums[int.Parse(c.ToString())]}:";
                }
                else switch (c)
                    {
                        case '?':
                            formatted += ":grey_question:";
                            break;
                        case '!':
                            formatted += ":grey_exclamation:";
                            break;
                        case '.':
                            formatted += ":record_button:";
                            break;
                        default:
                            formatted += c;
                            break;
                    }
            }
            await ctx.RespondAsync(formatted);
        }

        [Command("emojireaction"), Description("Generates a emoji text"), Aliases("react", "emreaction", "emreact")]
        public async Task EmojiReaction(CommandContext ctx, ulong dmessageId, [RemainingText] string message)
        {
            DiscordMessage dmessage = dmessageId == 0 ? ctx.Message : await ctx.Channel.GetMessageAsync(dmessageId);
            if (dmessageId != 0) await ctx.Message.DeleteAsync();
            foreach (var c in message)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    await dmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, $":regional_indicator_{char.ToLower(c)}:"));
                }
                else if (c >= '0' && c <= '9')
                {
                    await dmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, $":{nums[int.Parse(c.ToString())]}:"));
                }
                else switch (c)
                    {
                        case '?':
                            await dmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":grey_question:"));
                            break;
                        case '!':
                            await dmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":grey_exclamation:"));
                            break;
                        case '.':
                            await dmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":record_button:"));
                            break;
                    }
            }
        }
    }
}
