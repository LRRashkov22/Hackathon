using System.Diagnostics.CodeAnalysis;

namespace Passalong.Application.Common;

public class Result
{
    protected Result(Error? error)
    {
        Error = error;
    }

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static Result Success()
    {
        return new Result(null);
    }

    public static implicit operator Result(Error error)
    {
        return new Result(error);
    }
}
