namespace Passalong.Application.Common;

public sealed record Error(ErrorType Type, string? Message)
{
    public static Error Validation(string message)
    {
        return new Error(ErrorType.Validation, message);
    }

    public static Error Conflict(string message)
    {
        return new Error(ErrorType.Conflict, message);
    }

    public static Error NotFound()
    {
        return new Error(ErrorType.NotFound, null);
    }

    public static Error Forbidden()
    {
        return new Error(ErrorType.Forbidden, null);
    }
}
