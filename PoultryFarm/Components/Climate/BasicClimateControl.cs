using PoultryFarm.Domain;

namespace PoultryFarm.Components;

public class BasicClimateControl : IClimateControl
{
    public void Regulate(Zone zone) 
    { 
        // Логіка симуляції підтримки температури/вологості (можна розширити в майбутньому)
    }
}