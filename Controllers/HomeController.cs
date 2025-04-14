/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWatching.Database;
using MovieWatching.Extensions;
using MovieWatching.Models;

namespace MovieWatching.Controllers;

public class HomeController : Controller
{
    [HttpGet("~/")]
    public async Task<ActionResult> Index(
        [FromServices] DatabaseContext databaseContext)
    {
        var user = await HttpContext.EnsureUser(databaseContext);
        if (user == null)
        {
            return View(null as MoviePlanListModel);
        }
        else
        {
            var sessionPlans = await databaseContext.MovieSessionPlans
                .OrderBy(sessionPlan => sessionPlan.Session != null ? 1 : 0)
                .ThenByDescending(sessionPlan => sessionPlan.Start)
                .Select(sessionPlan => new MoviePlanSummaryModel()
                {
                    Id = sessionPlan.Id,
                    UserName = sessionPlan.Organizer.User.Name,
                    SessionName = sessionPlan.Name,
                    SelectedMovie = sessionPlan.Session != null ? sessionPlan.Session.Movie.Name : null
                })
                .ToListAsync();

            return View(new MoviePlanListModel()
            {
                User = user,
                UserName = user.Name,
                Plans = sessionPlans,
                MoviesNewOrganizationPlan = await CreateNewMoviesOrganizationPlan(databaseContext, user),
            });
        }
    }
    
    [HttpGet("~/SessionPlan/{sessionPlanId}")]
    public async Task<ActionResult> SessionPlan(
        [FromServices] DatabaseContext databaseContext,
        [FromRoute] Guid sessionPlanId)
    {
        var user = await HttpContext.EnsureUser(databaseContext);
        if (user == null)
        {
            return View(null as MovieSessionPlanModel);
        }
        else
        {
            var sessionPlan = await databaseContext.GetSessionPlan(sessionPlanId);
            return View(new MovieSessionPlanModel()
            {
                MoviesVote = sessionPlan != null
                    ? await databaseContext.GetMovieSessionModel(user, sessionPlan, "")
                    : null,
                MoviesOrganizationPlan = await CreateMoviesOrganizationPlan(databaseContext, user, sessionPlan),
                VotedMovie = await CreateVotedMovieModel(databaseContext, user, sessionPlan),
                IsOrganizer = user.Organizer != null,
                UserName = HttpContext.User!.Identity!.Name!
            });
        }
    }

    private async Task<MovieNewSessionOrganizationModel?> CreateNewMoviesOrganizationPlan(DatabaseContext databaseContext, User user)
    {
        if (user.Organizer == null)
        {
            return null;
        }

        return new MovieNewSessionOrganizationModel()
        {

        };
    }

    private async Task<VotedMovieModel?> CreateVotedMovieModel(
        DatabaseContext databaseContext,
        User user,
        MovieSessionPlan? sessionPlan)
    {
        if (sessionPlan != null)
        {
            return null;
        }
        
        var movieSession = await databaseContext.MovieSessions
            .OrderByDescending(movieSession => movieSession.When)
            .Select(movieSession => new VotedMovieModel()
            {
                When = movieSession.When,
                MovieName = movieSession.Movie.Name,
                MovieUrl = movieSession.Movie.Url,
            })
            .FirstOrDefaultAsync();
        
        return movieSession;
    }

    private static async Task<MovieSessionOrganizationModel?> CreateMoviesOrganizationPlan(
        DatabaseContext databaseContext, User user, MovieSessionPlan? sessionPlan)
    {
        if (user.Organizer != null && sessionPlan?.Organizer.UserId == user.Id)
        {
            return new MovieSessionOrganizationModel()
            {
                MovieSessionPlanId = sessionPlan.Id,
                Voters = (await databaseContext.MovieVotes
                    .Where(movieVote => sessionPlan.Id == movieVote.MovieSessionPlanId)
                    .Select(movieVote => movieVote.Voter)
                    .Distinct()
                    .Select(u => new { u.Id, u.Name })
                    .ToListAsync())
                    .Select(u => (u.Id.ToString(), u.Name))
                    .ToList(),
            };
        }
        else
        {
            return null;
        }
    }
}