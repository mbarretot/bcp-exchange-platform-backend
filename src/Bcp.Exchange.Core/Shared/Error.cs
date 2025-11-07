namespace Bcp.Exchange.Core.Shared;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error Create(string code, string message) => new(code, message);
}
