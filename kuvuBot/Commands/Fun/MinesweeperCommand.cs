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
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Fun
{
    public enum Difficulty { Easy, Normal, Hard, Insane }
    class Field
    {
        public int x;
        public int y;
        public bool isMine = false;

        public Field(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Gameboard
    {
        public List<Field> Fields;
        public Difficulty Difficulty;

        public Field GetFieldAt(int x, int y)
        {
            return Fields.FirstOrDefault(p => p.x == x && p.y == y);
        }

        public void Generate()
        {
            Fields = new List<Field>();
            int size = 0;
            int mines = 0;

            switch (Difficulty)
            {
                case Difficulty.Easy:
                    size = 5;
                    mines = new Random().Next(1,3);
                    break;
                case Difficulty.Normal:
                    size = 10;
                    mines = new Random().Next(8, 10);
                    break;
                case Difficulty.Hard:
                    size = 13;
                    mines = new Random().Next(11, 13);
                    break;
                case Difficulty.Insane:
                    size = 14;
                    mines = new Random().Next(20, 22);
                    break;
            }


            for (int y = 1; y <= size; y++)
            {
                for (int x = 1; x <= size; x++)
                {
                    Fields.Add(new Field(x, y));
                }
            }

            for (int i = 1; i <= mines; i++)
            {
                GetFieldAt(new Random().Next(1, size), new Random().Next(1, size)).isMine = true;
            }
        }

        public string ToDiscordMessage(bool spoiler = true)
        {
            StringBuilder builder = new StringBuilder();

            if(Difficulty < Difficulty.Hard)
            {
                builder.Append($"Mines {Fields.Count(p => p.isMine)}, Fields {Fields.Count()}\n");
            }

            var lastY = 1;
            foreach (Field field in Fields)
            {
                if (lastY != field.y)
                {
                    builder.Append("\n");
                }
                if (field.isMine)
                {
                    builder.Append(spoiler ? $"||💣||" : "💣");
                }
                else
                {
                    int minesAround = 0;

                    var sequence = new List<(int, int)>
                    {
                        (0, 1), // up
                        (0, -1), // down
                        (1, 0), // right
                        (-1, 0), // left
                        //cross
                        (-1, 1),
                        (1, 1),
                        (1, -1),
                        (-1, -1),
                    };

                    foreach (var xy in sequence)
                    {
                        var p = GetFieldAt(field.x + xy.Item1, field.y + xy.Item2);
                        if (p != null && p.isMine)
                            minesAround++;
                    }

                    builder.Append(spoiler ? $"||:{EmojiTextCommand.nums[minesAround]}:||" : $":{EmojiTextCommand.nums[minesAround]}:");
                }
                lastY = field.y;
            }
            return builder.ToString();
        }

        public Gameboard(Difficulty difficulty)
        {
            this.Difficulty = difficulty;
        }
    }

    public class MinesweeperCommand : BaseCommandModule
    {
        class MessageOwner
        {
            public DiscordUser User;
            public DiscordMessage Message;
            public Gameboard Gameboard;

            public MessageOwner(DiscordUser user, DiscordMessage message, Gameboard gameboard)
            {
                User = user;
                Message = message;
                Gameboard = gameboard;
            }
        }

        static List<MessageOwner> MessageOwners = new List<MessageOwner>();

        static readonly DiscordEmoji ReGenerateEmoji = DiscordEmoji.FromUnicode("🔁");
        static readonly DiscordEmoji RevealEmoji = DiscordEmoji.FromUnicode("💡");

        [Command("minesweeper"), Aliases("saper"), LocalizedDescription("minesweeper.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task MineSweeper(CommandContext ctx, string difficultyName = "normal")
        {
            var parsed = Enum.TryParse<Difficulty>(difficultyName, out var result);
            Difficulty difficulty = parsed ? result : Difficulty.Normal;
            var board = new Gameboard(difficulty);
            board.Generate();

            var message = await ctx.RespondAsync(board.ToDiscordMessage());

            MessageOwners.Add(new MessageOwner(ctx.User, message, board));
            await message.CreateReactionAsync(ReGenerateEmoji);
            await message.CreateReactionAsync(RevealEmoji);
        }

        public static async Task Client_MessageReactionAdded(DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
        {
            var messageOwner = MessageOwners.FirstOrDefault(x => x.User == e.User && x.Message == e.Message);
            if (messageOwner != null)
            {
                var board = messageOwner.Gameboard;
                if (e.Emoji == ReGenerateEmoji)
                {
                    board.Generate();
                    await e.Message.ModifyAsync(board.ToDiscordMessage());
                }
                else if (e.Emoji == RevealEmoji)
                {
                    await e.Message.ModifyAsync(board.ToDiscordMessage(false));
                }

                await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                e.Handled = true;
            }
        }
    }
}
