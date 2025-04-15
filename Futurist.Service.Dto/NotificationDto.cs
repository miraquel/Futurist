namespace Futurist.Service.Dto;

public class NotificationDto<T>
{
    public string Method { get; set; } = string.Empty;
    public T? Data { get; set; } = default!;
}