namespace Lavalink4NET.Rest.Entities.Usage;

using System;
using Lavalink4NET.Protocol.Models.Usage;

public sealed record class LavalinkServerStatistics(
    int ConnectedPlayers,
    int PlayingPlayers,
    TimeSpan Uptime,
    ServerMemoryUsageStatistics MemoryUsage,
    ServerProcessorUsageStatistics ProcessorUsage,
    ServerFrameStatistics? FrameStatistics)
{
    internal static LavalinkServerStatistics FromModel(LavalinkServerStatisticsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var memoryUsage = new ServerMemoryUsageStatistics(
            FreeMemory: model.MemoryUsage.FreeMemory,
            UsedMemory: model.MemoryUsage.UsedMemory,
            AllocatedMemory: model.MemoryUsage.AllocatedMemory,
            ReservableMemory: model.MemoryUsage.ReservableMemory);

        var processorUsage = new ServerProcessorUsageStatistics(
            CoreCount: model.ProcessorUsage.CoreCount,
            SystemLoad: model.ProcessorUsage.SystemLoad,
            LavalinkLoad: model.ProcessorUsage.LavalinkLoad);

        var frameStatistics = model.FrameStatistics is null ? null : new ServerFrameStatistics(
            SentFrames: model.FrameStatistics.SentFrames,
            NulledFrames: model.FrameStatistics.NulledFrames,
            DeficitFrames: model.FrameStatistics.DeficitFrames) as ServerFrameStatistics?;

        return new LavalinkServerStatistics(
            ConnectedPlayers: model.ConnectedPlayers,
            PlayingPlayers: model.PlayingPlayers,
            Uptime: model.Uptime,
            MemoryUsage: memoryUsage,
            ProcessorUsage: processorUsage,
            FrameStatistics: frameStatistics);
    }
}
