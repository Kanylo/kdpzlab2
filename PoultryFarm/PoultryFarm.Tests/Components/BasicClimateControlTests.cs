using Moq;
using PoultryFarm.Components;
using PoultryFarm.Domain;
using Xunit;

namespace PoultryFarm.Tests.Components;

public class BasicClimateControlTests
{
    [Fact]
    public void Regulate_ShouldNotThrowExceptions()
    {
        // Arrange
        var control = new BasicClimateControl();
        var mockClimate = new Mock<IClimateControl>();
        var zone = new Zone(1, ZonePurpose.Broilers, 100, mockClimate.Object);

        // Act
        var exception = Record.Exception(() => control.Regulate(zone));

        // Assert
        Assert.Null(exception);
    }
}
