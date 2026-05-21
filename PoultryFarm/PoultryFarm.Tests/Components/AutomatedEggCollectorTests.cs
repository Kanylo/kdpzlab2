using FluentAssertions;
using Moq;
using PoultryFarm.Components;
using PoultryFarm.Domain;
using Xunit;

namespace PoultryFarm.Tests.Components;

public class AutomatedEggCollectorTests
{
    [Fact]
    public void Collect_ShouldAddEggs_WhenAgeAndHealthAreSufficient()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Layers, 100, mockClimate.Object);
        
        // Force conditions
        zone.CurrentFlock.AgeInDays = 21;
        zone.CurrentFlock.HealthIndex = 51;
        zone.CurrentFlock.EggsCollected = 0;

        var collector = new AutomatedEggCollector();

        // Act
        collector.Collect(zone);

        // Assert
        zone.CurrentFlock.EggsCollected.Should().Be(5); // 100 * 0.05
    }

    [Fact]
    public void Collect_ShouldNotAddEggs_WhenAgeIsTooLow()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Layers, 100, mockClimate.Object);
        
        // Force conditions
        zone.CurrentFlock.AgeInDays = 20; // Needs to be > 20
        zone.CurrentFlock.HealthIndex = 100;
        zone.CurrentFlock.EggsCollected = 0;

        var collector = new AutomatedEggCollector();

        // Act
        collector.Collect(zone);

        // Assert
        zone.CurrentFlock.EggsCollected.Should().Be(0);
    }

    [Fact]
    public void Collect_ShouldNotAddEggs_WhenHealthIsTooLow()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Layers, 100, mockClimate.Object);
        
        // Force conditions
        zone.CurrentFlock.AgeInDays = 25; 
        zone.CurrentFlock.HealthIndex = 50; // Needs to be > 50
        zone.CurrentFlock.EggsCollected = 0;

        var collector = new AutomatedEggCollector();

        // Act
        collector.Collect(zone);

        // Assert
        zone.CurrentFlock.EggsCollected.Should().Be(0);
    }
}
