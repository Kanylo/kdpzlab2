using FluentAssertions;
using PoultryFarm.Components;
using Xunit;

namespace PoultryFarm.Tests.Components;

public class FeedingStrategyTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void StarterFeedingStrategy_ShouldBeApplicable_UpTo10Days(double age, bool expected)
    {
        var strategy = new StarterFeedingStrategy();
        strategy.IsApplicable(age).Should().Be(expected);
    }

    [Fact]
    public void StarterFeedingStrategy_ShouldReturnCorrectName()
    {
        var strategy = new StarterFeedingStrategy();
        strategy.GetName().Should().Be("Стартовий (1-10 дн)");
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(11, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    public void GrowerFeedingStrategy_ShouldBeApplicable_Between11And20Days(double age, bool expected)
    {
        var strategy = new GrowerFeedingStrategy();
        strategy.IsApplicable(age).Should().Be(expected);
    }

    [Fact]
    public void GrowerFeedingStrategy_ShouldReturnCorrectName()
    {
        var strategy = new GrowerFeedingStrategy();
        strategy.GetName().Should().Be("Ростовий (11-20 дн)");
    }

    [Theory]
    [InlineData(20, false)]
    [InlineData(21, true)]
    [InlineData(100, true)]
    public void FinisherFeedingStrategy_ShouldBeApplicable_After20Days(double age, bool expected)
    {
        var strategy = new FinisherFeedingStrategy();
        strategy.IsApplicable(age).Should().Be(expected);
    }

    [Fact]
    public void FinisherFeedingStrategy_ShouldReturnCorrectName()
    {
        var strategy = new FinisherFeedingStrategy();
        strategy.GetName().Should().Be("Фінішний (21+ дн)");
    }
}
