using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MovieWatching.Models;

namespace MovieWatching.Database;

public class User
{
    public ulong Id { get; private set; }
    
    [MaxLength(128)]
    public string Name { get; set; }

    private List<Movie> _movies;
    public IReadOnlyCollection<Movie> Movies => _movies;
    
    private List<MovieVote> _votes;
    public IReadOnlyCollection<MovieVote> Votes => _votes;
    
    public UserOrganizer? Organizer { get; set; }

    private User()
    {
        _movies = null!;
        Name = null!;
        _votes = null!;
    }
    
    public User(ulong id, string name)
    {
        Id = id;
        Name = name;
        _movies = new List<Movie>();
        _votes = new List<MovieVote>();
        Organizer = null;
    }

    public Result AddMovie(Movie movie)
    {
        if (Movies.Count >= 10)
        {
            return Result.TooManyMovies;
        }
        
        _movies.Add(movie);
        return Result.Ok;
    }

    public Result RemoveMovie(Guid movieId)
    {
        var index = _movies.FindIndex(movie => movie.Id == movieId);
        if (index == -1)
        {
            return Result.NotFound;
        }
        
        _movies.RemoveAt(index);
        return Result.Ok;
    }
}