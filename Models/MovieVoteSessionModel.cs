using System;
using System.Collections.Generic;
using MovieWatching.Database;

namespace MovieWatching.Models;

public class MovieVoteSessionModel
{
    public IEnumerable<MovieEntry> Movies { get; set; }
    
    public string ErrorMessage { get; set; }
    
    public string MovieSessionName { get; set; }
    
    public Guid MovieSessionPlanId { get; set; }
}