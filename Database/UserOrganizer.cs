using System;

namespace MovieWatching.Database;

public class UserOrganizer
{
    public Guid Id { get; private set; }
    
    public ulong UserId { get; private set; }
    public User User { get; private set; }
    
    private UserOrganizer()
    {
        User = null!;
    }

    public UserOrganizer(User user)
    {
        UserId = user.Id;
        User = user;
    }
}