using System;
using FluentAssertions;
using Moq;
using PoultryFarm.Components;
using PoultryFarm.Domain;
using Xunit;

namespace PoultryFarm.Tests.Domain;

public class ZoneTests
{
    [Fact]
    public void Zone_Initialization_ShouldSetInitialValuesCorrectly()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        int birdCount = 100;

        // Act
        var zone = new Zone(1, ZonePurpose.Broilers, birdCount, mockClimate.Object);

        // Assert
        zone.Id.Should().Be(1);
        zone.Purpose.Should().Be(ZonePurpose.Broilers);
        zone.CurrentFlock.Count.Should().Be(birdCount);
        zone.CurrentFlock.AgeInDays.Should().Be(1);
        zone.FoodKg.Should().Be(birdCount * 0.5);
        zone.WaterLiters.Should().Be(birdCount * 1.0);
        zone.FeedingStrategy.Should().BeOfType<StarterFeedingStrategy>();
    }

    [Fact]
    public void Tick_ShouldDecreaseFoodAndWater_WhenEnoughResourcesAvailable()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Broilers, 100, mockClimate.Object);
        double initialFood = zone.FoodKg;
        double initialWater = zone.WaterLiters;
        double daysPassed = 1;

        // Act
        zone.Tick(daysPassed);

        // Assert
        zone.CurrentFlock.AgeInDays.Should().Be(2); // Initial is 1, passed 1
        zone.FoodKg.Should().Be(initialFood - (100 * 0.1 * daysPassed));
        zone.WaterLiters.Should().Be(initialWater - (100 * 0.25 * daysPassed));
        zone.CurrentFlock.HealthIndex.Should().Be(100); // Max is 100
        mockClimate.Verify(c => c.Regulate(zone), Times.Once);
    }

    [Fact]
    public void Tick_ShouldDecreaseHealth_WhenResourcesAreInsufficient()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Broilers, 100, mockClimate.Object);
        
        // Let's exhaust resources manually (wait, we can't easily set them. We will just Tick enough times to exhaust them).
        // Initial food: 100 * 0.5 = 50kg. Consumes 10kg/day. Takes 5 days to exhaust.
        // Initial water: 100 * 1.0 = 100L. Consumes 25L/day. Takes 4 days to exhaust.
        
        zone.Tick(4); // Water exhausts here. Food is 10kg.

        double currentHealth = zone.CurrentFlock.HealthIndex; // Should be 100

        // Act - tick 1 day with no water
        zone.Tick(1);

        // Assert
        zone.CurrentFlock.HealthIndex.Should().BeLessThan(currentHealth);
    }

    [Fact]
    public void RefillResources_ShouldIncreaseFoodAndWater()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Broilers, 100, mockClimate.Object);
        double initialFood = zone.FoodKg;
        double initialWater = zone.WaterLiters;

        // Act
        zone.RefillResources(10, 20);

        // Assert
        zone.FoodKg.Should().Be(initialFood + 10);
        zone.WaterLiters.Should().Be(initialWater + 20);
    }

    [Fact]
    public void Tick_ShouldChangeFeedingStrategy_WhenAgeThresholdCrossed()
    {
        // Arrange
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Broilers, 100, mockClimate.Object); // Starts at Age 1 (Starter)
        
        // Act - Tick 10 days to make age 11
        // We will refill resources so health doesn't drop to 0, though strategy update doesn't depend on health.
        zone.RefillResources(1000, 1000); 
        zone.Tick(10);

        // Assert
        zone.CurrentFlock.AgeInDays.Should().Be(11);
        zone.FeedingStrategy.Should().BeOfType<GrowerFeedingStrategy>();
    }
}
