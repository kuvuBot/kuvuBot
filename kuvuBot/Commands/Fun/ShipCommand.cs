using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using SkiaSharp;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;

namespace kuvuBot.Commands.Fun
{
    public class ShipCommand : BaseCommandModule
    {
        private SKBitmap HeartEmoji { get; } = SKBitmap.Decode(Assembly.GetExecutingAssembly().GetManifestResourceStream("kuvuBot.Assets.heart.png"));

        [Aliases("paruj")]
        [Command("ship"), LocalizedDescription("ship.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Ship(CommandContext ctx, [Description("User to ship")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();

            var randomMember = ctx.Guild.Members.Values.ToList()[new Random().Next(ctx.Guild.Members.Count)];

            using var httpClient = new HttpClient();

            using var targetAvatar = SKBitmap.Decode(await httpClient.GetStreamAsync(target.AvatarUrl));
            using var randomAvatar = SKBitmap.Decode(await httpClient.GetStreamAsync(randomMember.AvatarUrl));

            var dimensions = new SKImageInfo(375, 135);
            using var surface = SKSurface.Create(dimensions);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            // Text styling
            var paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextSize = 22,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Roboto")
            };

            // Draw text and images
            canvas.DrawBitmap(targetAvatar, SKRect.Create(37.5f, 0, 96, 96));
            canvas.DrawText($"{target.Name()}", 96f / 2f + 37.5f, 96 + 24, paint);

            canvas.DrawBitmap(HeartEmoji, SKRect.Create(37.5f + 96f + 30f, 27F, 48f, 48f));

            canvas.DrawBitmap(randomAvatar, SKRect.Create(37.5f + 96f + 30 + 48 + 28.5f, 0, 96, 96));
            canvas.DrawText($"{randomMember.Name()}", 96f / 2f + 37.5f + 96f + 30 + 48 + 28.5f, 96 + 24, paint);

            // Generate and send the image
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Webp, 100);
            await ctx.RespondWithFileAsync(fileData: data.AsStream(), fileName: "ship.webp");
        }
    }
}