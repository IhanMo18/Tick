using Models.Users;

namespace Models.Profiles.ProfileDomainService;

public static class ProfileDomainService
{
    // Lógica de negocio pura, sin I/O
    public static Profile CreateInitialProfile(User user, ProfileDraft draft)
    {
        var dob = DateBirth.Create(draft.Dob);
        var alias = Alias.Create(draft.Alias);

        
        var prefs = Preferences.Create(draft.Preferences, AgeRange.Create(draft.MinAge, draft.MaxAge));

        return Profile.Create(
            id: Guid.NewGuid(),
            ownerId: user.Id,
            aliasRaw: alias.Value,
            dob: dob,
            city: ApproximateCity.Create(draft.City),
            bio: draft.Bio,
            preferences: prefs,
            showCity: true,
            hideOnline: false,
            avatarRoot: null
        );
    }
}