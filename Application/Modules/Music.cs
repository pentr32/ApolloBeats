using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Application.Services;
using System.Threading.Tasks;

namespace Application.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        public ulong userId;
        public EmbedBuilder builder = new EmbedBuilder();
        private bool isBotConnected;

        #region Dependency Injection
        private MusicService _musicService;

        public Music(MusicService musicService)
        {
            _musicService = musicService;
            isBotConnected = false;
        }
        #endregion Dependency Injection

        [Command("Join")]
        public async Task Join()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            if(isBotConnected == true)
            {
                builder.WithDescription(":x: I'm already connected to a channel.")
                   .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
                return;
            }

            if (user.VoiceChannel == null)
            {
                builder.WithDescription(":x:  You need to connect to a voice channel.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
                return;
            }
            else
            {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                isBotConnected = true;

                builder.WithDescription($":white_check_mark:  I'm now connected to [**{user.VoiceChannel.Name}]**")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("J")]
        public async Task JoinJ()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            if (isBotConnected == true)
            {
                builder.WithDescription(":x: I'm already connected to a channel.")
                   .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
                return;
            }

            if (user.VoiceChannel == null)
            {
                builder.WithDescription(":x:  You need to connect to a voice channel.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
                return;
            }
            else
            {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                isBotConnected = true;

                builder.WithDescription($":white_check_mark:  I'm now connected to [**{user.VoiceChannel.Name}]**")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel == null)
            {
                builder.WithDescription(":x:  Please join the channel the bot is in to make it leave.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);

                builder.WithDescription($":wave:  Bot has now left [**{user.VoiceChannel.Name}**]")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("L")]
        public async Task LeaveL()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                builder.WithDescription(":x:  Please join the channel the bot is in to make it leave.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);

                builder.WithDescription($":wave:  Bot has now left [**{user.VoiceChannel.Name}**]")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("Disconnect")]
        public async Task Disconnect()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                builder.WithDescription(":x:  Please join the channel the bot is in to make it leave.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);

                builder.WithDescription($":wave:  Bot has now left [**{user.VoiceChannel.Name}**]")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("D")]
        public async Task DisconnectD()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                builder.WithDescription(":x:  Please join the channel the bot is in to make it leave.")
                    .WithColor(Color.Red);
                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);

                builder.WithDescription($":wave:  Bot has now left [**{user.VoiceChannel.Name}**]")
                    .WithColor(Color.DarkPurple);
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("Play")]
        public async Task Play([Remainder]string query)
        {
            userId = Context.User.Id;
            builder = await _musicService.PlayAsync(query, Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("P")]
        public async Task PlayP([Remainder]string query)
        {
            userId = Context.User.Id;
            builder = await _musicService.PlayAsync(query, Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Stop")]
        public async Task Stop()
        {
            userId = Context.User.Id;
            builder = await _musicService.StopAsync(Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Skip")]
        public async Task Skip()
        {
            userId = Context.User.Id;
            builder = await _musicService.SkipAsync(Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Volume")]
        public async Task Volume(int newVolume)
        {
            userId = Context.User.Id;
            builder = await _musicService.SetVolumeAsync(newVolume, Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("V")]
        public async Task VolumeV(int newVolume)
        {
            userId = Context.User.Id;
            builder = await _musicService.SetVolumeAsync(newVolume, Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Pause")]
        public async Task Pause()
        {
            userId = Context.User.Id;
            builder = await _musicService.PauseOrResumeAsync(Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Resume")]
        public async Task Resume()
        {
            userId = Context.User.Id;
            builder = await _musicService.ResumeAsync(Context.Guild.Id, userId);
            await ReplyAsync("", false, builder.Build());
        }
    }
}