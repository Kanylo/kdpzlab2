using PoultryFarm.Domain;

namespace PoultryFarm.Components;

public interface IClimateControl 
{ 
    void Regulate(Zone zone); 
}