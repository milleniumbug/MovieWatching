using System;
using System.Collections.Generic;

namespace MovieWatching.Models;

public class StartSessionRequest
{
    public IEnumerable<ulong> Voter { get; set; }
    
    public Guid MovieSessionPlanId { get; set; }
}