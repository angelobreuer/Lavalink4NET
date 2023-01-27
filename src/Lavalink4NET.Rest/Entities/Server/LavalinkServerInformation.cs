namespace Lavalink4NET.Rest.Entities.Server;

using System;
using System.Collections.Immutable;
using Lavalink4NET.Protocol.Tests.Models;

public sealed record class LavalinkServerInformation(
    LavalinkServerVersion Version,
    DateTimeOffset BuiltAt,
    LavalinkCommitInformation CommitInformation,
    string JavaVersion,
    string LavaplayerVersion,
    ImmutableArray<string> AvailableSourceManagers,
    ImmutableArray<string> AvailableFilters,
    ImmutableArray<LavalinkPluginInformation> Plugins)
{
    internal static LavalinkServerInformation FromModel(LavalinkServerInformationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var version = new LavalinkServerVersion(
            Version: new Version(model.Version.Major, model.Version.Minor, model.Version.Patch),
            PreRelease: model.Version.PreRelease);

        var commitInformation = new LavalinkCommitInformation(
            BranchName: model.CommitInformation.BranchName,
            CommitId: model.CommitInformation.CommitId,
            CommittedAt: model.CommitInformation.CommittedAt);

        var plugins = model.Plugins.Select(x => new LavalinkPluginInformation(
            Name: x.Name,
            Version: x.Version));

        return new LavalinkServerInformation(
            Version: version,
            BuiltAt: model.BuiltAt,
            CommitInformation: commitInformation,
            JavaVersion: model.JavaVersion,
            LavaplayerVersion: model.LavaplayerVersion,
            AvailableSourceManagers: model.AvailableSourceManagers,
            AvailableFilters: model.AvailableFilters,
            Plugins: plugins.ToImmutableArray());
    }
}
