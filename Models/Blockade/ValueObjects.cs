namespace Models.Blockade;

public readonly record struct Reason
{
    
    public string Value { get; }
    
    private Reason(string value)=>Value=value;
    
    
    public static Reason Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Razon requerida.");
        var r = value.Trim();
        if (r.Length is < 3 or > 200) throw new ArgumentException("Razon debe tener 3–200 caracteres.");
        return new Reason(r);
    }
    public override string ToString() => Value;
}