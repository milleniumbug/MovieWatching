using System;

namespace MovieWatching.Models;

public class VotedMovieModel
{
    public string MovieName { get; set; }
    
    public DateTime When { get; set; }
    
    public Uri? MovieUrl { get; set; }
}