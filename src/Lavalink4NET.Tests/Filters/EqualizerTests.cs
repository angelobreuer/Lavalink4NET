namespace Lavalink4NET.Tests.Filters;

using System;
using Lavalink4NET.Filters;
using Xunit;

public sealed class EqualizerTests
{
    [Theory]
    [InlineData(0, -0.25F)]
    [InlineData(1, -0.15F)]
    [InlineData(2, -0.05F)]
    [InlineData(3, 0.05F)]
    [InlineData(4, 0.10F)]
    [InlineData(5, 0.15F)]
    [InlineData(6, 0.20F)]
    [InlineData(7, 0.25F)]
    [InlineData(8, 0.30F)]
    [InlineData(9, 0.35F)]
    [InlineData(10, 0.40F)]
    [InlineData(11, 0.45F)]
    [InlineData(12, 0.50F)]
    [InlineData(13, 0.55F)]
    [InlineData(14, 0.60F)]
    public void TestSetUsingBuilderAndGetUsingIndexer(int index, float value)
    {
        var builder = Equalizer.CreateBuilder();

        builder[index] = value;

        var equalizer = builder.Build();

        Assert.Equal(value, equalizer[index]);
    }

    [Theory]
    [InlineData(0, -0.25F)]
    [InlineData(1, -0.15F)]
    [InlineData(2, -0.05F)]
    [InlineData(3, 0.05F)]
    [InlineData(4, 0.10F)]
    [InlineData(5, 0.15F)]
    [InlineData(6, 0.20F)]
    [InlineData(7, 0.25F)]
    [InlineData(8, 0.30F)]
    [InlineData(9, 0.35F)]
    [InlineData(10, 0.40F)]
    [InlineData(11, 0.45F)]
    [InlineData(12, 0.50F)]
    [InlineData(13, 0.55F)]
    [InlineData(14, 0.60F)]
    public void TestSetUsingIndexerAndGetUsingIndexer(int index, float value)
    {
        var builder = Equalizer.CreateBuilder();
        builder[index] = value;
        Assert.Equal(value, builder[index]);
    }

    [Theory]
    [InlineData(0, -0.35F, -0.25)]
    [InlineData(13, 1.55F, 1.0F)]
    public void TestBuilderClampsValue(int index, float value, float clampedValue)
    {
        var builder = Equalizer.CreateBuilder();
        builder[index] = value;
        Assert.Equal(clampedValue, builder[index]);
    }

    [Fact]
    public void TestBuild()
    {
        var builder = Equalizer.CreateBuilder();
        builder.Band0 = -0.25F;
        builder.Band1 = 0.0F;
        builder.Band2 = 0.05F;
        builder.Band3 = 0.10F;
        builder.Band4 = 0.15F;
        builder.Band5 = 0.20F;
        builder.Band6 = 0.25F;
        builder.Band7 = 0.30F;
        builder.Band8 = 0.35F;
        builder.Band9 = 0.40F;
        builder.Band10 = 0.45F;
        builder.Band11 = 0.50F;
        builder.Band12 = 0.55F;
        builder.Band13 = 0.60F;
        builder.Band14 = 0.65F;

        var equalizer = builder.Build();

        Assert.Equal(builder.Band0, equalizer.Band0);
        Assert.Equal(builder.Band1, equalizer.Band1);
        Assert.Equal(builder.Band2, equalizer.Band2);
        Assert.Equal(builder.Band3, equalizer.Band3);
        Assert.Equal(builder.Band4, equalizer.Band4);
        Assert.Equal(builder.Band5, equalizer.Band5);
        Assert.Equal(builder.Band6, equalizer.Band6);
        Assert.Equal(builder.Band7, equalizer.Band7);
        Assert.Equal(builder.Band8, equalizer.Band8);
        Assert.Equal(builder.Band9, equalizer.Band9);
        Assert.Equal(builder.Band10, equalizer.Band10);
        Assert.Equal(builder.Band11, equalizer.Band11);
        Assert.Equal(builder.Band12, equalizer.Band12);
        Assert.Equal(builder.Band13, equalizer.Band13);
        Assert.Equal(builder.Band14, equalizer.Band14);
    }

    [Fact]
    public void TestBuilderFromEqualizer()
    {
        var equalizer = new Equalizer
        {
            Band0 = -0.25F,
            Band1 = 0.0F,
            Band2 = 0.05F,
            Band3 = 0.10F,
            Band4 = 0.15F,
            Band5 = 0.20F,
            Band6 = 0.25F,
            Band7 = 0.30F,
            Band8 = 0.35F,
            Band9 = 0.40F,
            Band10 = 0.45F,
            Band11 = 0.50F,
            Band12 = 0.55F,
            Band13 = 0.60F,
            Band14 = 0.65F,
        };

        var builder = Equalizer.CreateBuilder(equalizer);

        Assert.Equal(builder.Band0, equalizer.Band0);
        Assert.Equal(builder.Band1, equalizer.Band1);
        Assert.Equal(builder.Band2, equalizer.Band2);
        Assert.Equal(builder.Band3, equalizer.Band3);
        Assert.Equal(builder.Band4, equalizer.Band4);
        Assert.Equal(builder.Band5, equalizer.Band5);
        Assert.Equal(builder.Band6, equalizer.Band6);
        Assert.Equal(builder.Band7, equalizer.Band7);
        Assert.Equal(builder.Band8, equalizer.Band8);
        Assert.Equal(builder.Band9, equalizer.Band9);
        Assert.Equal(builder.Band10, equalizer.Band10);
        Assert.Equal(builder.Band11, equalizer.Band11);
        Assert.Equal(builder.Band12, equalizer.Band12);
        Assert.Equal(builder.Band13, equalizer.Band13);
        Assert.Equal(builder.Band14, equalizer.Band14);
    }

    [Fact]
    public void TestCreateUsingInit()
    {
        var equalizer = new Equalizer
        {
            Band0 = -0.25F,
            Band1 = 0.0F,
            Band2 = 0.05F,
            Band3 = 0.10F,
            Band4 = 0.15F,
            Band5 = 0.20F,
            Band6 = 0.25F,
            Band7 = 0.30F,
            Band8 = 0.35F,
            Band9 = 0.40F,
            Band10 = 0.45F,
            Band11 = 0.50F,
            Band12 = 0.55F,
            Band13 = 0.60F,
            Band14 = 0.65F,
        };

        Assert.Equal(-0.25F, equalizer.Band0);
        Assert.Equal(0.0F, equalizer.Band1);
        Assert.Equal(0.05F, equalizer.Band2);
        Assert.Equal(0.10F, equalizer.Band3);
        Assert.Equal(0.15F, equalizer.Band4);
        Assert.Equal(0.20F, equalizer.Band5);
        Assert.Equal(0.25F, equalizer.Band6);
        Assert.Equal(0.30F, equalizer.Band7);
        Assert.Equal(0.35F, equalizer.Band8);
        Assert.Equal(0.40F, equalizer.Band9);
        Assert.Equal(0.45F, equalizer.Band10);
        Assert.Equal(0.50F, equalizer.Band11);
        Assert.Equal(0.55F, equalizer.Band12);
        Assert.Equal(0.60F, equalizer.Band13);
        Assert.Equal(0.65F, equalizer.Band14);
    }

    [Fact]
    public void TestBandOutOfRange()
    {
        var equalizer = Equalizer.CreateBuilder();
        void Act() => equalizer[15] = 0.0F;
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void TestDefaultEqualizerAllBandsDefault()
    {
        var equalizer = Equalizer.Default;

        Assert.Equal(0.0F, equalizer.Band0);
        Assert.Equal(0.0F, equalizer.Band1);
        Assert.Equal(0.0F, equalizer.Band2);
        Assert.Equal(0.0F, equalizer.Band3);
        Assert.Equal(0.0F, equalizer.Band4);
        Assert.Equal(0.0F, equalizer.Band5);
        Assert.Equal(0.0F, equalizer.Band6);
        Assert.Equal(0.0F, equalizer.Band7);
        Assert.Equal(0.0F, equalizer.Band8);
        Assert.Equal(0.0F, equalizer.Band9);
        Assert.Equal(0.0F, equalizer.Band10);
        Assert.Equal(0.0F, equalizer.Band11);
        Assert.Equal(0.0F, equalizer.Band12);
        Assert.Equal(0.0F, equalizer.Band13);
        Assert.Equal(0.0F, equalizer.Band14);
    }
}
