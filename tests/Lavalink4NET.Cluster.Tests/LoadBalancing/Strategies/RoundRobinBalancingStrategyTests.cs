namespace Lavalink4NET.Cluster.Tests.LoadBalancing.Strategies;

using System.Collections.Immutable;
using System.Threading.Tasks;
using Lavalink4NET.Cluster.LoadBalancing.Strategies;
using Lavalink4NET.Cluster.Nodes;
using Microsoft.Extensions.Options;
using Moq;

public sealed class RoundRobinBalancingStrategyTests
{
    [Fact]
    public async Task TestScoreWithThreeNodesAsync()
    {
        // Arrange
        var node1 = Mock.Of<ILavalinkNode>();
        var node2 = Mock.Of<ILavalinkNode>();
        var node3 = Mock.Of<ILavalinkNode>();

        var options = new RoundRobinBalancingStrategyOptions();
        var strategy = new RoundRobinBalancingStrategy(Options.Create(options));

        // Act
        var balanceResult = await strategy
            .ScoreAsync(ImmutableArray.Create(node1, node2, node3))
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(3D / 3.0, balanceResult.Nodes[0].Score);
        Assert.Equal(2D / 3.0, balanceResult.Nodes[1].Score);
        Assert.Equal(1D / 3.0, balanceResult.Nodes[2].Score);
    }

    [Fact]
    public async Task TestScoreWithThreeNodesShiftedAsync()
    {
        // Arrange
        var node1 = Mock.Of<ILavalinkNode>();
        var node2 = Mock.Of<ILavalinkNode>();
        var node3 = Mock.Of<ILavalinkNode>();

        var options = new RoundRobinBalancingStrategyOptions();
        var strategy = new RoundRobinBalancingStrategy(Options.Create(options));

        await strategy
            .ScoreAsync(ImmutableArray.Create(node1, node2, node3))
            .ConfigureAwait(false);

        // Act
        var balanceResult = await strategy
            .ScoreAsync(ImmutableArray.Create(node1, node2, node3))
            .ConfigureAwait(false);

        // Assert
        Assert.Equal(1D / 3.0, balanceResult.Nodes[0].Score);
        Assert.Equal(3D / 3.0, balanceResult.Nodes[1].Score);
        Assert.Equal(2D / 3.0, balanceResult.Nodes[2].Score);
    }
}
