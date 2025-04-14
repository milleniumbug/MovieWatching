using System;

namespace MovieWatching.Models;

public class MoviePlanSummaryModel
{
    public string UserName { get; set; }
    public string SessionName { get; set; }
    public string? SelectedMovie { get; set; }
    public Guid Id { get; set; }
}