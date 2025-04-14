using System;

namespace MovieWatching.Database;

public class MovieSession
{
    public Guid Id { get; private set; }
    
    public Movie Movie { get; private set; }
    
    public MovieSessionPlan MovieSessionPlan { get; }
    public Guid MovieSessionPlanId { get; }

    public DateTime When { get; private set; }

    private MovieSession()
    {
        Movie = null!;
        MovieSessionPlan = null!;
    }

    public MovieSession(Movie movie, MovieSessionPlan movieSessionPlan, DateTime when)
    {
        Id = Guid.NewGuid();
        Movie = movie;
        MovieSessionPlan = movieSessionPlan;
        MovieSessionPlanId = movieSessionPlan.Id;
        When = when;
    }
}