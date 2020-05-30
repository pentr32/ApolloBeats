using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace Application.Services
{
    public class MusicService
    {
        #region Dependency Injection
        private readonly LavaRestClient _lavaRestClient;
        private readonly LavaSocketClient _lavaSocketClient;
        private readonly DiscordSocketClient _client;
        private readonly LogService _logService;

        public EmbedBuilder builderService = new EmbedBuilder();

        public MusicService(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient, LogService logService)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
            _logService = logService;
        }
        #endregion Dependency Injection

        // Method that intialize the the properties from 'Dependency Injection' region by help from methods down below.
        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            return Task.CompletedTask;
        }

        #region Connect/ Leave/ ClientReady Lavalink asynchron
        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
           => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client);
        }
        #endregion Connect/ Leave/ ClientReady Lavalink asynchron

        /// <summary>
        /// Used to play the track asynchron.
        /// </summary>
        /// <param name="query">Track's name</param>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns>EmbedBuilder</returns>
        public async Task<EmbedBuilder> PlayAsync(string query, ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);                                                                 // Get the player from Lavalink by Guild's Id.
            var results = await _lavaRestClient.SearchYouTubeAsync(query);                                                      // Search the track on YouTube.

            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)                              // If the track couldn't be found, will return 'No matches found' message as embedBuilder.
            {
                builderService.WithDescription(":x:  No matches found.")                                                        // Add the description to embedBuilder with discord emoji in front of message.
                    .WithColor(Color.Red);
                return builderService;
            }

            var track = results.Tracks.FirstOrDefault();                                                                        // Find the track from LavaLink.

            if (_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);                                                                                   // Add the next track to queue if a player is already playing a track.

                builderService.WithTitle("Queued")
                    .WithDescription($":dvd:  [{track.Title}]({track.Uri}) has been added to the queue. [<@{userId}>]")         // Add the description to the embedBuilder.
                    .WithColor(Color.Gold);
                return builderService;
            }
            else
            {
                builderService.WithTitle("Now Playing")
                    .WithDescription($":arrow_forward:  [{track.Title}]({track.Uri}) [<@{userId}>]")
                    .WithColor(Color.DarkPurple);
                await _player.PlayAsync(track);                                                                                 // Play the wished track.
                return builderService;
            }
        }

        /// <summary>
        /// Used to stop the track asynchron.
        /// </summary>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns>EmbedBuilder</returns>
        public async Task<EmbedBuilder> StopAsync(ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player != null)
            {
                builderService.WithDescription($":pause_button:  Music Playback **Stopped**. [<@{userId}>]")
                    .WithColor(Color.DarkPurple);

                await _player.StopAsync();                                                                                         // Stop the player.
                return builderService;


            }

            builderService.WithDescription(":x:  Error with Player")
                .WithColor(Color.Red);
            return builderService;
        }

        /// <summary>
        /// Used to skip the track asynchron.
        /// </summary>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns>EmbedBuilder</returns>
        public async Task<EmbedBuilder> SkipAsync(ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player != null || _player.Queue.Items.Count() > -1)
            {
                await _player.SkipAsync();                                                                                      // Skip the current track.

                builderService.WithTitle("Skiped")
                    .WithDescription($":track_next:  Now Playing: [{_player.CurrentTrack.Title}]({_player.CurrentTrack.Uri}) [<@{userId}>]")
                    .WithColor(Color.Orange);
                return builderService;
            }

            builderService.WithDescription(":x:  Nothing in **queue**.")
                .WithColor(Color.Red);
            return builderService;
        }

        /// <summary>
        /// Used to set the player's volume asynchron.
        /// </summary>
        /// <param name="vol">New volume</param>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns>EmbedBuilder</returns>
        public async Task<EmbedBuilder> SetVolumeAsync(int vol, ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player != null)
            {
                if (vol > 150 || vol <= 2)
                {
                    builderService.WithDescription(":x:  Please use a number between **2 - 150**.")
                        .WithColor(Color.Red);
                    return builderService;
                }

                await _player.SetVolumeAsync(vol);                                                                              // Set the player's volume.

                builderService.WithTitle("Volume")
                    .WithDescription($":loud_sound:  Volume set to: **{vol}** [<@{userId}>]")
                    .WithColor(Color.DarkPurple);
                return builderService;
            }

            builderService.WithDescription(":x:  Player isn't **playing**.")
                .WithColor(Color.Red);
            return builderService;
        }

        /// <summary>
        /// Used to pause or resume the track asynchron.
        /// </summary>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns></returns>
        public async Task<EmbedBuilder> PauseOrResumeAsync(ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player != null)
            {
                if (!_player.IsPaused)                                                                                           // Checks if the player is paused.
                {
                    await _player.PauseAsync();                                                                                  // Pause the player.
                    builderService.WithDescription($":pause_button:  Player is **Paused**. [<@{userId}>]")
                        .WithColor(Color.DarkPurple);
                    return builderService;
                }
                else
                {
                    await _player.ResumeAsync();                                                                                 // Resume the player.
                    builderService.WithDescription($":arrow_forward:  Player **Resumed**. [<@{userId}>]")
                        .WithColor(Color.DarkPurple);
                    return builderService;
                }
            }

            builderService.WithDescription(":x:  Player isn't **playing**.")
                .WithColor(Color.Red);
            return builderService;

        }

        /// <summary>
        /// Used to resume the track asynchron
        /// </summary>
        /// <param name="guildId">Guild's Id</param>
        /// <param name="userId">User's Id</param>
        /// <returns></returns>
        public async Task<EmbedBuilder> ResumeAsync(ulong guildId, ulong userId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player != null)
            {
                builderService.WithDescription(":x:  Player is not **Paused**.")
                    .WithColor(Color.Red);
                return builderService;
            }

            if (_player.IsPaused)
            {
                await _player.ResumeAsync();
                builderService.WithDescription($":arrow_forward:  Player **Resumed**. [<@{userId}>]")
                    .WithColor(Color.DarkPurple);
                return builderService;
            }

            builderService.WithDescription(":x:  Player isn't **playing**.")
                .WithColor(Color.Red);
            return builderService;

        }

        // Method that checks when the track is finished will play the next one if there is any track in queue, otherwise the player will stop.
        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;

            if (player.Queue.TryDequeue(out var nextTrack) && nextTrack != null)
            {
                var nextTrackInfo = (LavaTrack)nextTrack;
                await player.PlayAsync(nextTrackInfo);

                builderService.WithTitle("Now Playing")
                    .WithDescription($":arrow_forward:  [{nextTrackInfo.Title}]({nextTrackInfo.Uri})")
                    .WithColor(Color.DarkPurple);
                await player.TextChannel.SendMessageAsync("", false, builderService.Build());              
            }
            else
            {
                builderService
                    .WithTitle("")
                    .WithDescription(":x: There are no more tracks in the **queue**.")
                    .WithColor(Color.Red);
                await player.TextChannel.SendMessageAsync("", false, builderService.Build());
            }

        }

        // Method that adds log messages to logs from DataLayer.Logs folder.
        private async Task LogAsync(LogMessage logMessage)
        {
            await _logService.LogAsync(logMessage);
        }
    }
}