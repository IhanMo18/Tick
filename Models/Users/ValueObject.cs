using System.Net.Mail;

namespace Models.Users;

public readonly record struct Email
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        var trimmed = value.Trim();
        try { _ = new MailAddress(trimmed); }
        catch { throw new ArgumentException("Formato de email inválido."); }
        return new Email(trimmed);
    }

    public override string ToString() => Value;
}