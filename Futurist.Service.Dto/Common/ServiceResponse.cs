using System.Text.Json.Serialization;

namespace Futurist.Service.Dto.Common;

public class ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = [];
    public bool IsSuccess => Errors.Count == 0;
    public string ErrorMessage => string.Join(", ", Errors);
}

public class ServiceResponse<T> : ServiceResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }
}