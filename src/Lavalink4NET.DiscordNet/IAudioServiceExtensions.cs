namespace Lavalink4NET.DiscordNet;

using System.Threading.Tasks;
using Discord;
using Lavalink4NET.Player;

/// <summary>
///     A set of different extension methods for the <see cref="IAudioService"/> class.
/// </summary>
public static class IAudioServiceExtensions
{
    /// <summary>
    ///     Gets the audio player for the specified <paramref name="guild"/>.
    /// </summary>
    /// <typeparam name="TPlayer">the type of the player to use</typeparam>
    /// <param name="audioService">the audio service</param>
    /// <param name="guild">the guild to get the player for</param>
    /// <returns>the player for the guild</returns>
    public static TPlayer? GetPlayer<TPlayer>(this IAudioService audioService, IGuild guild) where TPlayer : LavalinkPlayer
        => audioService.GetPlayer<TPlayer>(guild.Id);

    /// <summary>
    ///     Gets the audio player for the specified <paramref name="guild"/>.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="guild">the guild to get the player for</param>
    /// <returns>the player for the guild</returns>
    public static LavalinkPlayer? GetPlayer(this IAudioService audioService, IGuild guild)
        => audioService.GetPlayer(guild.Id);

    /// <summary>
    ///     Gets a value indicating whether a player is created for the specified <paramref name="guild"/>.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="guild">the guild to create the player for</param>
    /// <returns>
    ///     a value indicating whether a player is created for the specified <paramref name="guild"/>
    /// </returns>
    public static bool HasPlayer(this IAudioService audioService, IGuild guild)
        => audioService.HasPlayer(guild.Id);

    /// <summary>
    ///     Joins the specified <paramref name="audioService"/> asynchronously.
    /// </summary>
    /// <typeparam name="TPlayer">the type of the player to create</typeparam>
    /// <param name="audioService">the audio service</param>
    /// <param name="voiceChannel">the voice channel to join</param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the audio player</para>
    /// </returns>
    public static Task<TPlayer> JoinAsync<TPlayer>(this IAudioService audioService, IVoiceChannel voiceChannel,
        bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer, new()
        => audioService.JoinAsync<TPlayer>(voiceChannel.GuildId, voiceChannel.Id, selfDeaf, selfMute);

    /// <summary>
    ///     Joins the specified <paramref name="audioService"/> asynchronously.
    /// </summary>
    /// <typeparam name="TPlayer">the type of the player to create</typeparam>
    /// <param name="audioService">the audio service</param>
    /// <param name="playerFactory">the player factory</param>
    /// <param name="voiceChannel">the voice channel to join</param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the audio player</para>
    /// </returns>
    public static Task<TPlayer> JoinAsync<TPlayer>(
        this IAudioService audioService, PlayerFactory<TPlayer> playerFactory, IVoiceChannel voiceChannel,
        bool selfDeaf = false, bool selfMute = false) where TPlayer : LavalinkPlayer
        => audioService.JoinAsync(playerFactory, voiceChannel.GuildId, voiceChannel.Id, selfDeaf, selfMute);

    /// <summary>
    ///     Joins the specified <paramref name="audioService"/> asynchronously.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="voiceChannel">the voice channel to join</param>
    /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
    /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
    /// <returns>
    ///     a task that represents the asynchronous operation
    ///     <para>the audio player</para>
    /// </returns>
    public static Task JoinAsync(this IAudioService audioService, IVoiceChannel voiceChannel,
        bool selfDeaf = false, bool selfMute = false)
        => audioService.JoinAsync(voiceChannel.GuildId, voiceChannel.Id, selfDeaf, selfMute);
}
