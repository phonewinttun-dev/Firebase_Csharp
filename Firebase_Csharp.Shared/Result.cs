using System.Text.Json.Serialization;

namespace Firebase_Csharp.Shared
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; }
        public bool IsFailure => !IsSuccess;

        [JsonConstructor]
        public Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message = "Success") => new(true, message);
        public static Result Failure(string message) => new(false, message);
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }
        [JsonConstructor]
        public Result(bool isSuccess, string message, T? value) : base(isSuccess, message)
        {
            Value = value;
        }
        public static Result<T> Success(T value, string message = "Success") => new(true, message, value);
        public static new Result<T> Failure(string message) => new(false, message, default);
    }
}
