using System;

namespace MovieWatching.Models;

public class AddNewMovieEntryModel
{
    public string? Error { get; set; }
    
    public Guid MovieSessionPlanId { get; set; }
}