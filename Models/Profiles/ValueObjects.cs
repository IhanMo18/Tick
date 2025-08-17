namespace Models.Profiles;

public readonly record struct Alias
{
    public string Value { get; }
    private Alias(string value) => Value = value;

    public static Alias Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Alias requerido.");
        var v = value.Trim();
        if (v.Length is < 3 or > 20) throw new ArgumentException("Alias debe tener 3–20 caracteres.");
        return new Alias(v);
    }
    public override string ToString() => Value;
}

public readonly record struct DateBirth
{
    public DateOnly Value { get; }
    private DateBirth(DateOnly value) => Value = value;

    public static DateBirth Create(DateOnly value, DateOnly? hoy = null)
    {
        var refDate = hoy ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var edad = CalcularEdad(value, refDate);
        if (edad < 13 || edad > 70) throw new ArgumentException("Edad permitida: 13–70.");
        return new DateBirth(value);
    }

    public int Edad(DateOnly? hoy = null) =>
        CalcularEdad(Value, hoy ?? DateOnly.FromDateTime(DateTime.UtcNow));

    private static int CalcularEdad(DateOnly fecha, DateOnly refDate)
    {
        var a = refDate.Year - fecha.Year;
        if (refDate < fecha.AddYears(a)) a--;
        return a;
    }
}



public readonly record struct ApproximateCity
{
    public string Value { get; }
    private ApproximateCity(string value) => Value = value;
    public static ApproximateCity Create(string? raw)
    {
        var v = raw?.Trim();
        if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("City requerida.");
        if (v.Length > 40) throw new ArgumentException("City demasiado larga.");
        return new ApproximateCity(v);
    }
    public override string ToString() => Value;
}


public sealed record AgeRange
{
    public int Min { get; init; }
    public int Max { get; init; }

    private AgeRange() { }                
    private AgeRange(int min, int max) { Min = min; Max = max; }

    public static AgeRange Create(int min, int max)
    {
        if (min < 13 || max > 70) throw new ArgumentException("Range permitido 13–70.");
        if (min > max) throw new ArgumentException("min <= max.");
        return new AgeRange(min, max);
    }
}

public enum PreferenceType
{
    Friendship = 1,
    Couple = 2,
    JustATime = 3
}

public sealed class Preferences
{
    private readonly HashSet<PreferenceType> _types;
    public IReadOnlyCollection<PreferenceType> Types => _types;

    public AgeRange Range { get; }
    public bool CommonInterestsOnly { get; }
    public bool WeeklySuggestions { get; }
    
    public Preferences() { }

    private Preferences(HashSet<PreferenceType> types, AgeRange range, bool only, bool weekly)
    {
        _types = types;
        Range = range;
        CommonInterestsOnly = only;
        WeeklySuggestions = weekly;
    }

    public static Preferences Create(
        IEnumerable<PreferenceType>? types,
        AgeRange range,
        bool commonInterestsOnly = false,
        bool weeklySuggestions = true)
    {
        var set = new HashSet<PreferenceType>(types ?? Array.Empty<PreferenceType>());
        if (set.Count == 0) set.Add(PreferenceType.Friendship);
        return new Preferences(set, range, commonInterestsOnly, weeklySuggestions);
    }
}

