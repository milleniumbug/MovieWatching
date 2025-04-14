using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieWatching.Extensions;
using MovieWatching.Models;

namespace MovieWatching.Database;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Movie> Movies { get; set; }
    
    public DbSet<UserOrganizer> UserOrganizers { get; set; }
    
    public DbSet<MovieSession> MovieSessions { get; set; }
    
    public DbSet<MovieSessionPlan> MovieSessionPlans { get; set; }
    
    public DbSet<MovieVote> MovieVotes { get; set; }

    public DatabaseContext()
    {
    }

    public DatabaseContext(
        DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public async Task<IEnumerable<User>> GetOrganizers()
    {
        return await UserOrganizers
            .Select(u => u.User)
            .ToListAsync();
    }

    public async Task<User> EnsureUser(ulong id, string name)
    {
        var user = await Users
            .Include(user => user.Movies.Where(movie => !movie.MovieSessions!.Any()))
            .Include(user => user.Organizer)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            user.Name = name;
            return user;
        }

        user = new User(id, name);
        this.Users.Add(user);
        return user;
    }

    public async Task<MovieSessionPlan?> GetCurrentSessionPlan(User? user = null)
    {
        IQueryable<MovieSessionPlan> sessionPlans = MovieSessionPlans;

        if (user != null)
        {
            sessionPlans = sessionPlans
                .Where(sessionPlan => sessionPlan.Organizer.User.Id == user.Id);
        }
        
        return await sessionPlans
            .Include(sessionPlan => sessionPlan.Organizer)
            .FirstOrDefaultAsync(
                movieSessionPlan => movieSessionPlan.Session == null);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        {
            var builder = modelBuilder.Entity<User>();
            builder
                .HasMany(user => user.Movies)
                .WithOne(movie => movie.AddedBy);
        }
        {
            var builder = modelBuilder.Entity<MovieSession>();
            builder
                .HasOne(session => session.MovieSessionPlan)
                .WithOne(sessionPlan => sessionPlan.Session)
                .HasForeignKey<MovieSession>(session => session.MovieSessionPlanId);
        }
        {
            var builder = modelBuilder.Entity<MovieVote>();
            builder
                .HasOne(movieVote => movieVote.Voter)
                .WithMany(voter => voter.Votes)
                .HasForeignKey(movieVote => movieVote.VoterId);
            
            builder
                .HasOne(movieVote => movieVote.MovieSessionPlan)
                .WithMany(movieSessionPlan => movieSessionPlan.Votes)
                .HasForeignKey(movieVote => movieVote.MovieSessionPlanId);

            builder
                .HasOne(movieVote => movieVote.Movie)
                .WithMany(movie => movie.Votes)
                .HasForeignKey(movieVote => movieVote.MovieId);
            
            builder
                .HasIndex(movieVote => new { movieVote.MovieSessionPlanId, movieVote.MovieId, movieVote.VoterId })
                .IsUnique(true);
            
        }
    }

    public async Task<Models.MovieVoteSessionModel> GetMovieSessionModel(User user, MovieSessionPlan sessionPlan, string errorMessage)
    {
        return new Models.MovieVoteSessionModel()
        {
            Movies = await GetMovies(user, sessionPlan, query => query.Where(movie => !movie.MovieSessions!.Any())),
            ErrorMessage = errorMessage,
            MovieSessionName = sessionPlan.Name,
            MovieSessionPlanId = sessionPlan.Id,
        };
    }

    private async Task<List<MovieEntry>> GetMovies(User user, MovieSessionPlan? sessionPlan, Func<IQueryable<Movie>, IQueryable<Movie>>? query = null)
    {
        query ??= x => x;
        
        var movies =
            await query(this.Movies)
                .Select(movie => new MovieEntry()
                {
                    Id = movie.Id,
                    Name = movie.Name,
                    MovieSessionPlanId = sessionPlan != null ? sessionPlan.Id : null,
                    Url = movie.Url != null ? movie.Url.ToString() : null,
                    UserAdded = movie.AddedBy.Id == user.Id,
                })
                .ToListAsync();
        
        var movieCounts = 
            await this.MovieVotes
                .Where(movieVote => movieVote.MovieSessionPlanId == sessionPlan!.Id)
                .GroupBy(movieVote => movieVote.MovieId)
                .Select(grp => new
                {
                    MovieId = grp.Key,
                    VoteCount = grp.Count(),
                })
                .ToDictionaryAsync(grp => grp.MovieId, grp => grp.VoteCount);

        var userVoted =
            (await this.MovieVotes
                .Where(movieVote => movieVote.VoterId == user.Id && movieVote.MovieSessionPlanId == sessionPlan!.Id)
                .Select(movieVote => movieVote.MovieId)
                .ToListAsync())
            .ToHashSet();

        foreach (var movie in movies)
        {
            movie.VoteCount = movieCounts.GetValueOrDefault(movie.Id, 0);
            movie.UserVoted = userVoted.Contains(movie.Id);
        }

        return movies;
    }

    public async Task<MovieEntry> GetMovieModel(User user, Movie movie, MovieSessionPlan? sessionPlan, string errorMessage)
    {
        var entries = await GetMovies(
            user,
            sessionPlan,
            query => query.Where(m => m.Id == movie.Id).Take(1));
        
        return entries.Single();
    }

    public async Task<MovieSessionPlan?> GetSessionPlan(Guid sessionPlanId)
    {
        return await this.MovieSessionPlans
            .Include(movieSessionPlan => movieSessionPlan.Session)
            .FirstOrDefaultAsync(movieSessionPlan => movieSessionPlan.Id == sessionPlanId);
    }

    public async Task<Movie> PickMovie(MovieSessionPlan movieSessionPlan, IEnumerable<ulong> users)
    {
        var movies =
            await this.MovieVotes
                .Where(movieVote => movieVote.MovieSessionPlanId == movieSessionPlan.Id)
                .Where(movieVote => users.Any(user => user == movieVote.VoterId))
                .GroupBy(movieVote => movieVote.Movie)
                .Select(grp => new
                {
                    Movie = grp.Key,
                    VoteCount = grp.Count(),
                })
                .OrderByDescending(grp => grp.VoteCount)
                .ToListAsync();

        var movieVotesMaxCount = movies.First().VoteCount;
        var votedMovies = movies.Where(movie => movie.VoteCount == movieVotesMaxCount).ToList();
        RandomExt.Shuffle(votedMovies);
        return votedMovies.First().Movie;
    }
}