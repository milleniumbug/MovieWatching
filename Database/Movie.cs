using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MovieWatching.Database;

public class Movie
{
    public Guid Id { get; private set; }
    
    [MaxLength(512)]
    public string Name { get; private set; }
    
    [TypeConverter(typeof(StringToUriConverter))]
    [MaxLength(512)]
    public Uri? Url { get; private set; }  
    
    public DateTime AddedOn { get; private set; }
    
    public User AddedBy { get; private set; }
    
    public IEnumerable<MovieVote> Votes { get; private set; }
    
    public IEnumerable<MovieSession>? MovieSessions { get; private set; }
    
    [UsedImplicitly]
    private Movie()
    {
        Name = null!;
        Votes = null!;
    }

    public Movie(string name, Uri? url)
    {
        Id = Guid.NewGuid();
        AddedOn = DateTime.UtcNow;
        Url = url;
        Name = name;
        Votes = new List<MovieVote>();
    }
}