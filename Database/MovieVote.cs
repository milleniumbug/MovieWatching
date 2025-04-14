using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MovieWatching.Database;

public class MovieVote
{
    public Guid Id { get; private set; }
    
    public User Voter { get; private set; }
    public ulong VoterId { get; private set; }
    
    public Movie Movie { get; private set; }
    public Guid MovieId { get; private set; }
    
    public MovieSessionPlan MovieSessionPlan { get; private set; }
    public Guid MovieSessionPlanId { get; private set; }

    [UsedImplicitly]
    private MovieVote()
    {
        Voter = null!;
        Movie = null!;
        MovieSessionPlan = null!;
    }

    public MovieVote(User voter, Movie movie, MovieSessionPlan sessionPlan)
    {
        Id = Guid.NewGuid();
        Voter = voter ?? throw new ArgumentNullException(nameof(voter));
        Movie = movie ?? throw new ArgumentNullException(nameof(movie));
        MovieSessionPlan = sessionPlan ?? throw new ArgumentNullException(nameof(sessionPlan));
    }
}