using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Fun
{
    public class ShipCommand : BaseCommandModule
    {
        [Aliases("paruj")]
        [Command("ship"), LocalizedDescription("ship.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Ship(CommandContext ctx, [Description("User to ship")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            
            var target2 = ctx.Guild.Members.Values.ToList()[new Random().Next(ctx.Guild.Members.Count)];
            
            var httpClient = new System.Net.Http.HttpClient();

            var bytesTarget = await httpClient.GetByteArrayAsync(target.AvatarUrl);
            var bytesTarget2 = await httpClient.GetByteArrayAsync(target2.AvatarUrl);
            var bytesEmoji = await httpClient.GetByteArrayAsync("https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/twitter/180/heavy-black-heart_2764.png");

            // wrap the bytes in a stream
            var stream = new MemoryStream(bytesTarget);
            var streamTarget2 = new MemoryStream(bytesTarget2);
            var streamEmoji = new MemoryStream(bytesEmoji);

            // decode the bitmap stream
            var bitmap = SKBitmap.Decode(stream);
            var bitmapTarget2 = SKBitmap.Decode(streamTarget2);
            var bitmapEmoji = SKBitmap.Decode(streamEmoji);

            var dimensions = new SKImageInfo(250, 90);
            using (var surface = SKSurface.Create(dimensions))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Transparent);

                // Text styling
                var paint = new SKPaint
                {
                    Color = SKColors.SlateGray,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    TextSize = 16,
                    TextAlign = SKTextAlign.Center,
                    Typeface = SKTypeface.FromFamilyName("Roboto")
                };

                // Draw text and images
                canvas.DrawText($"{target.Name()}", 57, 80, paint);
                canvas.DrawText($"{target2.Name()}", 193, 80, paint);
                canvas.DrawBitmap(bitmap, SKRect.Create(25,0,64,64));
                canvas.DrawBitmap(bitmapEmoji, SKRect.Create(109,18,32,32));
                canvas.DrawBitmap(bitmapTarget2, SKRect.Create(160,0,64,64));

                // Generate and send the image
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 50))
                {
                    await ctx.RespondWithFileAsync(fileData: data.AsStream(), fileName: $"ship.png");
                }
            }
        }
    }
}