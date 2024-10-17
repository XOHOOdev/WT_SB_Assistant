namespace WtSbAssistant.Core.Dto
{
    public class Result<TObj>(TObj? value = default, bool? success = null, Exception? exception = null, string? message = null)
    {
        public TObj? Value { get; set; } = value;

        public bool Success { get; set; } = success ?? exception == null && value != null;

        public Exception? Exception { get; set; } = exception;

        public string? Message { get; set; } = message;
    }
}
