using System;
using System.Collections.Generic;

namespace MovieWatching.Models;

public class MovieSessionOrganizationModel
{
    public IEnumerable<(string id, string name)> Voters { get; set; }
    
    public Guid MovieSessionPlanId { get; set; }
}