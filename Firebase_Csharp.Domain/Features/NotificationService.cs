using Firebase_Csharp.Domain.Features.DTO;
using Firebase_Csharp.Shared;
using FirebaseAdmin.Messaging;

namespace Firebase_Csharp.Domain.Features
{
    public class NotificationService : INotificationService
    {
        #region Send To Device
        public async Task<Result<NotificationResponseDto>> SendToDeviceAsync(SendToDeviceRequestDto request)
        {
            if (string.IsNullOrEmpty(request.DeviceToken))
            {
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
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Result<NotificationResponseDto>.Success(new NotificationResponseDto { MessageId = messageId });
            }
            catch (FirebaseMessagingException ex)
            {
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
#endregion

        #region Send To Topic
        public async Task<Result<NotificationResponseDto>> SendToTopicAsync(SendToTopicRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Topic))
            {
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
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Result<NotificationResponseDto>.Success(new NotificationResponseDto { MessageId = messageId });
            }
            catch (FirebaseMessagingException ex)
            {
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
        #endregion
    }
}
