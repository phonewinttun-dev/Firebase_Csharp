namespace Firebase_Csharp.Domain.Features.DTO
{
    public class NotificationResponseDto
    {
        public string MessageId { get; set; } = string.Empty;
    }

    public class SendToDeviceRequestDto
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class SendToTopicRequestDto
    {
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

}
