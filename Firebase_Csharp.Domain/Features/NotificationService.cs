using System.Text.RegularExpressions;
using Firebase_Csharp.Domain.Features.DTO;
using Firebase_Csharp.Shared;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Firebase_Csharp.Domain.Features
{
    public class NotificationService(ILogger<NotificationService> logger) : INotificationService
    {
        private readonly ILogger<NotificationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private static readonly Regex TopicRegex = new(@"^(/topics/)?([a-zA-Z0-9-_.~%]+)$", RegexOptions.Compiled);

        #region Send To Device
        public async Task<Result<NotificationResponseDto>> SendToDeviceAsync(SendToDeviceRequestDto request)
        {

            if (request is null) return Result<NotificationResponseDto>.Failure("The request cannot be null!");

            _logger.LogInformation(
                "Preparing notification for DeviceToken/FID: {DeviceToken} | Title: {Title}",
                request.DeviceToken,
                request.Title);

            if (string.IsNullOrWhiteSpace(request.DeviceToken))
            {
                _logger.LogWarning("Validation failed: DeviceToken is empty.");
                return Result<NotificationResponseDto>.Failure("Device token is required.");
            }

            var message = new Message
            {
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body
                }
            };

            // Detects whether the input is an FID or a standard FCM registration token
            if (request.DeviceToken.Length <= 30 && !request.DeviceToken.Contains(':'))
            {
                message.Fid = request.DeviceToken;
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete (FCM client registration tokens use Token)
                message.Token = request.DeviceToken;
#pragma warning restore CS0618
            }

            try
            {
                _logger.LogInformation("Sending message payload to Firebase Cloud Messaging server....");
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);
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
                return Result<NotificationResponseDto>.Failure($"Firebase error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid message argument: {Message}", ex.Message);
                return Result<NotificationResponseDto>.Failure($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending notification: {Message}", ex.Message);
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
        #endregion

        #region Send To Topic
        public async Task<Result<NotificationResponseDto>> SendToTopicAsync(SendToTopicRequestDto request)
        {
            if (request is null) return Result<NotificationResponseDto>.Failure("The request cannot be null");

            _logger.LogInformation(
                "Preparing notification for Topic: {Topic} | Title: {Title}",
                request.Topic,
                request.Title);

            if (string.IsNullOrWhiteSpace(request.Topic))
            {
                _logger.LogWarning("Validation failed: Topic is empty.");
                return Result<NotificationResponseDto>.Failure("Topic is required.");
            }

            if (!TopicRegex.IsMatch(request.Topic))
            {
                _logger.LogWarning("Validation failed: Topic contains invalid characters: {Topic}", request.Topic);
                return Result<NotificationResponseDto>.Failure("Malformed topic name. Topic cannot contain spaces or special characters other than '-', '_', '.', '~', '%'.");
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
                string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message).ConfigureAwait(false);
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
                return Result<NotificationResponseDto>.Failure($"Firebase error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid topic argument: {Message}", ex.Message);
                return Result<NotificationResponseDto>.Failure($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending topic notification: {Message}", ex.Message);
                return Result<NotificationResponseDto>.Failure($"Error sending notification: {ex.Message}");
            }
        }
        #endregion
    }
}
