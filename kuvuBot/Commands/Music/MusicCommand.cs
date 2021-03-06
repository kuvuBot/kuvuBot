﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using kuvuBot.Core.Commands;

namespace kuvuBot.Commands.Music
{
    [RequireLavalink]
    [Hidden]
    public class MusicCommand : BaseCommandModule
    {
        public class RequireLavalinkAttribute : CheckBaseAttribute
        {
            public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            {
                return Task.FromResult(Lavalink != null);
            }
        }

        public static LavalinkNodeConnection Lavalink { get; set; }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string query)
        {
            query.RequireRemainingText();
            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                await ctx.RespondAsync("You need to be in a voice channel.");
                return;
            }

            await chn.ConnectAsync(Lavalink);
            var loadResult = await Lavalink.Rest.GetTracksAsync(query);
            LavalinkTrack track = loadResult.Tracks.First();

            var connection = Lavalink.GetConnection(ctx.Guild);
            await connection.PlayAsync(track);
            await ctx.RespondAsync($"👌, playing {track.Uri}");
        }

        [Command("volume")]
        public async Task Volume(CommandContext ctx, int volume)
        {
            volume = Math.Clamp(volume, 0, 1000);

            var connection = Lavalink.GetConnection(ctx.Guild);
            await connection.SetVolumeAsync(volume);
            await ctx.RespondAsync($"👌, set volume to {volume}");
        }

        [Command("stop")]
        public async Task Stop(CommandContext ctx)
        {
            var connection = Lavalink.GetConnection(ctx.Guild);
            await connection.StopAsync();
            await ctx.RespondAsync($"👌");
        }

        [Command("queue")]
        public async Task Queue(CommandContext ctx)
        {
            var connection = Lavalink.GetConnection(ctx.Guild);
            await ctx.RespondAsync($"{connection.CurrentState.CurrentTrack.Uri}");
        }
    }
}
