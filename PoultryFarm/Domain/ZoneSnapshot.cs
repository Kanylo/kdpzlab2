using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoultryFarm.Domain;

public record ZoneSnapshot(
    int Id, 
    ZonePurpose Purpose, 
    int Count, 
    double AgeInDays, 
    double Health, 
    double Food, 
    double Water, 
    int Eggs, 
    string FeedType);