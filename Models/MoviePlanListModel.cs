using System.Collections.Generic;
using MovieWatching.Database;

namespace MovieWatching.Models;

public class MoviePlanListModel
{
    public User User { get; set; }
    
    public string UserName { get; set; }
    
    public MovieNewSessionOrganizationModel? MoviesNewOrganizationPlan { get; set; }
    
    public IReadOnlyList<MoviePlanSummaryModel> Plans { get; set; }
}