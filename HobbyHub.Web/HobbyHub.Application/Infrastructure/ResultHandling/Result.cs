using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;

namespace HobbyHub.Application.Infrastructure.ResultHandling;

public class Result<T>
{
    private Result(bool isSuccess, Error? error, T? outcome)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Outcome = outcome;
    }
    
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public T? Outcome { get; }
    public bool IsSuccess { get; }
    public Error? Error { get; }

    public static Result<T> Success(T outcome) => new(true, Error.None, outcome);

    public static Result<T> Failure(Error error) => new(false, error);
}