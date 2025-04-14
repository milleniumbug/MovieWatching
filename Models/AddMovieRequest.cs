using System;
using System.ComponentModel.DataAnnotations;

namespace MovieWatching.Models;

public class AddMovieRequest
{
    [MinLength(1)]
    [Required]
    public string Name { get; set; }
    
    [Url]
    public string? Url { get; set; }
    
    public Guid MovieSessionPlanId { get; set; }
}