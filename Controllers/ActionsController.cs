using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWatching.Database;
using MovieWatching.Extensions;
using MovieWatching.Models;

namespace MovieWatching.Controllers;

[Authorize]
public class ActionsController : Controller
{
    [HttpPost]
    public async Task<ActionResult> AddMovie(
        [FromServices] DatabaseContext databaseContext,
        [FromForm] AddMovieRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }

        var movieSessionPlan = await databaseContext.GetSessionPlan(request.MovieSessionPlanId) ?? throw new ArgumentException();

        if (this.ModelState.IsValid == false)
        {
            var errors = ModelState.SelectMany(x => x.Value?.Errors ?? []);
            
            var errorMessage = string.Join(';', errors.Select(x => x.ErrorMessage));
            
            return PartialView("MovieList", await databaseContext.GetMovieSessionModel(user, movieSessionPlan, errorMessage));
        }

        var movie = new Movie(
            request.Name,
            request.Url != null ? new Uri(request.Url) : null);
        databaseContext.Movies.Add(movie);
        var result = user.AddMovie(movie);
        if (result == Result.Ok)
        {
            await databaseContext.SaveChangesAsync();

            return PartialView("MovieList", await databaseContext.GetMovieSessionModel(user, movieSessionPlan, ""));
        }
        else if(result == Result.TooManyMovies)
        {
            return PartialView("MovieList", await databaseContext.GetMovieSessionModel(user, movieSessionPlan, "You can't add more than 10 movies"));
        }
        else
        {
            return UnprocessableEntity();
        }
    }
    
    [HttpPost]
    public async Task<ActionResult> VoteMovie(
        [FromServices] DatabaseContext databaseContext,
        VoteMovieRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }
        
        var movieSessionPlan = await databaseContext.GetSessionPlan(request.SessionPlanId);

        if (this.ModelState.IsValid == false)
        {
            return UnprocessableEntity();
        }
        
        if (movieSessionPlan == null)
        {
            return NotFound();
        }
        
        if (movieSessionPlan.Session != null)
        {
            return UnprocessableEntity();
        }
        
        var movie = await databaseContext.Movies.FirstOrDefaultAsync(movie => movie.Id == request.MovieId);

        if (movie == null)
        {
            return NotFound();
        }

        if (await databaseContext.MovieVotes.AnyAsync(movieVote =>
                movieVote.MovieId == request.MovieId &&
                movieVote.MovieSessionPlanId == request.SessionPlanId &&
                movieVote.VoterId == user.Id))
        {
            return UnprocessableEntity();
        }
        
        var vote = new MovieVote(user, movie, movieSessionPlan);
        databaseContext.MovieVotes.Add(vote);
        await databaseContext.SaveChangesAsync();
        return PartialView("MovieEntry", await databaseContext.GetMovieModel(user, movie, movieSessionPlan, ""));
    }
    
    [HttpPost]
    public async Task<ActionResult> RevokeVote(
        [FromServices] DatabaseContext databaseContext,
        RevokeVoteMovieRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }
        
        var movieSessionPlan = await databaseContext.GetSessionPlan(request.SessionPlanId);

        if (this.ModelState.IsValid == false)
        {
            return UnprocessableEntity();
        }
        
        if (movieSessionPlan == null)
        {
            return NotFound();
        }
        
        if (movieSessionPlan.Session != null)
        {
            return UnprocessableEntity();
        }
        
        var movie = await databaseContext.Movies.FirstOrDefaultAsync(movie => movie.Id == request.MovieId);

        if (movie == null)
        {
            return NotFound();
        }

        var vote = await databaseContext.MovieVotes.FirstOrDefaultAsync(movieVote =>
            movieVote.MovieId == request.MovieId &&
            movieVote.MovieSessionPlanId == request.SessionPlanId &&
            movieVote.VoterId == user.Id);
        if (vote == null)
        {
            return UnprocessableEntity();
        }
        
        databaseContext.MovieVotes.Remove(vote);
        await databaseContext.SaveChangesAsync();
        return PartialView("MovieEntry", await databaseContext.GetMovieModel(user, movie, movieSessionPlan, ""));
    }

    [HttpPost]
    public async Task<ActionResult> StartSession(
        [FromServices] DatabaseContext databaseContext,
        [FromForm] StartSessionRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }

        var movieSessionPlan = await databaseContext.GetSessionPlan(request.MovieSessionPlanId);

        if (movieSessionPlan == null)
        {
            return NotFound();
        }
        
        if (user.Organizer?.Id != movieSessionPlan.OrganizerId)
        {
            return Forbid();
        }
        
        var movie = await databaseContext.PickMovie(movieSessionPlan, request.Voter);

        var movieSession = new MovieSession(movie, movieSessionPlan, DateTime.UtcNow);
        databaseContext.MovieSessions.Add(movieSession);
        await databaseContext.SaveChangesAsync();
        return Redirect("/");
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteMovie(
        [FromServices] DatabaseContext databaseContext,
        [FromQuery] DeleteMovieRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }
        
        var result = user.RemoveMovie(request.MovieId);
        if (result == Result.Ok)
        {
            await databaseContext.SaveChangesAsync();

            return Ok();
        }
        else if(result == Result.NotFound)
        {
            return NoContent();
        }
        else
        {
            return UnprocessableEntity();
        }
    }

    [HttpPost]
    public async Task<ActionResult> StartSessionPlanning(
        [FromServices] DatabaseContext databaseContext,
        [FromForm] StartSessionPlanningRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user?.Organizer == null)
        {
            return Forbid();
        }

        var movieSessionPlan = new MovieSessionPlan(
            user.Organizer,
            request.Name,
            DateTime.UtcNow);
        databaseContext.MovieSessionPlans.Add(movieSessionPlan);
        await databaseContext.SaveChangesAsync();
        return Redirect("/");
    }
    
    /*[HttpPost]
    public async Task<ActionResult> BecomeOrganizer(
        [FromServices] DatabaseContext databaseContext,
        [FromForm] BecomeOrganizerRequest request)
    {
        var user = await HttpContext.EnsureUser(databaseContext);

        if (user == null)
        {
            return Forbid();
        }

        var organizer = await databaseContext.UserOrganizers
            .FirstOrDefaultAsync(userOrganizer => userOrganizer.UserId == user.Id);
        if (organizer == null)
        {
            var userOrganizer = new UserOrganizer(user);
            databaseContext.UserOrganizers.Add(userOrganizer);

            await databaseContext.SaveChangesAsync();
        }

        return Ok();
    }*/
}