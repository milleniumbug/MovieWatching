using System;
using Microsoft.AspNetCore.Mvc;

namespace MovieWatching.Models;

public class VoteMovieRequest
{
    [FromQuery]
    public Guid MovieId { get; set; }
    
    [FromQuery]
    public Guid SessionPlanId { get; set; }
}