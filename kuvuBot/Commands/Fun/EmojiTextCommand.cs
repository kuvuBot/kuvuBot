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
        [Command("emojitext"), Description("Generates a emoji text"), Aliases("emtext")]
        public async Task EmojiText(CommandContext ctx, [RemainingText] string message)
        {
            string formatted = "";
            string[] nums = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            int index = 0;
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
                index++;
            }
            await ctx.RespondAsync(formatted);
        }
    }
}
