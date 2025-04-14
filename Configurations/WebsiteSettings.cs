using System;

namespace MovieWatching.Configurations;

public class WebsiteSettings
{
    public const string SectionName = "Website";
    
    public string CanonicalUrl { get; set; } = null!;
    
    public string BasePath => new Uri(CanonicalUrl).AbsolutePath;
}