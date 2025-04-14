using System;

namespace MovieWatching.Models;

public class MovieEntry
{
    public bool UserVoted { get; set; }
    
    public bool UserAdded { get; set; }
    
    public int VoteCount { get; set; }
    
    public Guid? MovieSessionPlanId { get; set; }
    
    public string? Url { get; set; }
    
    public string Name { get; set; }
    
    public Guid Id { get; set; }
}