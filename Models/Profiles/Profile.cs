using Models.Events;
using Models.Shared;
using Prefs = Models.Profiles.Preferences; 
namespace Models.Profiles;


public sealed class Profile : AggregateRoot
{
    public Guid Id { get; private set; }
    
    public Guid OwnerId { get; private set; }
    public Alias Alias { get; private set; }
    public DateBirth DateBirth { get; private set; }
    public ApproximateCity City { get; private set; }
    public string? Bio { get; private set; }
    public Prefs Preferences { get; private set; }

    public bool ShowCity { get; private set; }
    public bool HideOnline { get; private set; }
    public string? AvatarRoot { get; private set; }

    public ProfileStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    private Profile() { }

    private Profile(Guid id, Alias alias, DateBirth dob, ApproximateCity city,
        string? bio, Prefs preferences,                   
        bool showCity, bool hideOnline, string? avatarRoot)
    {
        Id = id;
        Alias = alias;
        DateBirth = dob;
        City = city;
        Bio = SetBio(bio);
        Status = ProfileStatus.Active;
        Preferences = preferences;
        ShowCity = showCity;
        HideOnline = hideOnline;
        AvatarRoot = avatarRoot;
        CreatedAt = DateTime.UtcNow; 
        UpdatedAt = DateTime.UtcNow;
    }

    public static Profile Create(
        Guid id,
        Guid ownerId,
        string aliasRaw,
        DateBirth dob,
        ApproximateCity city,
        string? bio,
        Prefs preferences,        
        bool showCity,
        bool hideOnline,
        string? avatarRoot)
    {
        var alias = Alias.Create(aliasRaw);
        var p = new Profile(id, alias, dob, city, bio, preferences, showCity, hideOnline, avatarRoot)
        {
            OwnerId = ownerId
        };
        p.Raise(new ProfileCreated(p.OwnerId,p.Id, alias.Value,dob,city,bio,preferences,showCity,hideOnline,avatarRoot));
        return p;
    }

    public void UpdateBio(string? bio)
    {
        Bio = SetBio(bio);
        Raise(new BioActualizada(Id, DateTime.UtcNow,Bio));
    }

    public void UpdatePreferences(Prefs prefs)    
    {
        Preferences = prefs;
        Raise(new PreferencesUpdated(Id, DateTime.UtcNow));
    }

    private static string? SetBio(string? bio)
    {
        if (bio == null) return null;
        var trimmed = bio.Trim();
        if (trimmed.Length > 200) throw new ArgumentException("Bio > 200 chars.");
        return trimmed;
    }

    public void ChangeStatus(ProfileStatus newStatus, string? reason = null)
    {
        if (newStatus == Status) return;

        if (Status == ProfileStatus.Banned && newStatus != ProfileStatus.Banned)
            throw new InvalidOperationException("Un perfil baneado no puede reactivarse directamente.");

        var old = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        Raise(new ProfileStatusChanged(Id, old, newStatus, reason));
    }
    
}

public enum ProfileStatus
{
    Banned=1,
    Disabled=2, 
    Active=3
}

public sealed record ProfileDraft(
    string Alias,
    IEnumerable<PreferenceType> Preferences,    
    DateOnly Dob,
    string City,
    string? Bio,
    int MinAge, 
    int MaxAge
);

