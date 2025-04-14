using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MovieWatching.Database;

public class MovieSessionPlan
{
    public Guid Id { get; private set; }
    
    [MaxLength(128)]
    public string Name { get; private set; }
    
    public DateTime Start { get; private set; }
    
    public UserOrganizer Organizer { get; private set; }
    public Guid OrganizerId { get; private set; }
    
    public MovieSession? Session { get; private set; }
    
    public IEnumerable<MovieVote> Votes { get; private set; }

    [UsedImplicitly]
    private MovieSessionPlan()
    {
        Name = null!;
        Votes = null!;
        Organizer = null!;
    }

    public MovieSessionPlan(UserOrganizer organizer, string name, DateTime start)
    {
        Id = Guid.NewGuid();
        Name = name;
        Start = start;
        Votes = new List<MovieVote>();
        Organizer = organizer;
    }

    public void Finalize(MovieSession session)
    {
        Session = session;
    }
}