using Firebase_Csharp.Domain.Features.DTO;
using Firebase_Csharp.Shared;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Firebase_Csharp.Domain.Features
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        #region Send To Device
        public async Task<Result<NotificationResponseDto>> SendToDeviceAsync(SendToDeviceRequestDto request)
        {
            _logger.LogInformation(
                "Preparing notification for DeviceToken/FID: {DeviceToken} | Title: {Title}",
                request.DeviceToken,
                request.Title);

            if (string.IsNullOrEmpty(request.DeviceToken))
            {
                _logger.LogWarning("Validation failed: DeviceToken is empty.");
                return Result<NotificationResponseDto>.Failure("Device token is required.");
            }

            var message = new Message
            {
                Fid = request.DeviceToken, // Fid = Firebase Installation ID
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body
                }
            };

            try
            {
                _logger.LogInformation("Sending message payload to Firebase Cloud Messaging server....");
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation(
                    "Successfully sent notification! Firebase MessageId: {MessageId}",
                    messageId);

                return Result<NotificationResponseDto>.Success(new NotificationResponseDto { MessageId = messageId });
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(
                    ex,
                    "Firebase rejected request. ErrorCode: {ErrorCode}, MessagingErrorCode: {MessagingErrorCode}, Message: {Message}",
                    ex.ErrorCode,
                    ex.MessagingErrorCode,
                    ex.Message);
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
        #endregion

        #region Send To Topic
        public async Task<Result<NotificationResponseDto>> SendToTopicAsync(SendToTopicRequestDto request)
        {
            _logger.LogInformation(
                "Preparing notification for Topic: {Topic} | Title: {Title}",
                request.Topic,
                request.Title);

            if (string.IsNullOrEmpty(request.Topic))
            {
                _logger.LogWarning("Validation failed: Topic is empty.");
                return Result<NotificationResponseDto>.Failure("Topic is required.");
            }

            var message = new Message
            {
                Topic = request.Topic,
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body
                }
            };

            try
            {
                _logger.LogInformation("Sending message payload to Firebase Cloud Messaging server....");
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation(
                    "Successfully sent topic notification! Firebase MessageId: {MessageId}",
                    messageId);

                return Result<NotificationResponseDto>.Success(new NotificationResponseDto { MessageId = messageId });
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(
                    ex,
                    "Firebase rejected topic request. ErrorCode: {ErrorCode}, MessagingErrorCode: {MessagingErrorCode}, Message: {Message}",
                    ex.ErrorCode,
                    ex.MessagingErrorCode,
                    ex.Message);
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
        #endregion
    }
}
