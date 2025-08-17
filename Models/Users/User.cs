// Models.User/User.cs

using Models.Events;
using Models.Profiles;
using Models.Shared;

namespace Models.Users;

public sealed class User : AggregateRoot
{
    public Guid Id { get; private set; }      
    public Email Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public UserRole UserRole { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastAccessAt { get; private set; }

    private User() { }

    private User(Guid id, Email email, UserRole role)
    {
        Id = id;
        Email = email;
        UserRole = role;
        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static User CreateFromIdentity(Guid identityUserId, Email email, UserRole role,ProfileDraft profileDraft)
    {
        var u = new User(identityUserId, email, role);
        u.Raise(new UserCreated(u.Id, u.Email.Value, u.UserRole));
        return u;
    }

    public void MarkEmailConfirmed()
    {
        if (EmailConfirmed) return;
        EmailConfirmed = true;
        Raise(new UserEmailConfirmed(Id));
    }

    public void MarkLoggedIn()
    {
        LastAccessAt = DateTime.UtcNow;
        Raise(new UserLoggedIn(Id, LastAccessAt.Value));
    }
}

public enum UserRole { Admin = 1, User = 2, Owner = 3 }