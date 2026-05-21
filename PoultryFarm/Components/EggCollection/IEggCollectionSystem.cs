using PoultryFarm.Domain;

namespace PoultryFarm.Components;

public interface IEggCollectionSystem 
{ 
    void Collect(Zone zone); 
}