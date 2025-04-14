using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using MovieWatching.Database;

namespace MovieWatching.Models;

public class MovieSessionPlanModel
{
    public MovieVoteSessionModel? MoviesVote { get; set; }
    
    public MovieSessionOrganizationModel? MoviesOrganizationPlan { get; set; }
    
    public string UserName { get; set; }
    
    public bool IsOrganizer { get; set; }
    
    public VotedMovieModel? VotedMovie { get; set; }
}