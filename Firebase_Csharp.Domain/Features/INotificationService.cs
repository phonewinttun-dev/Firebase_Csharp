using Firebase_Csharp.Domain.Features.DTO;
using Firebase_Csharp.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firebase_Csharp.Domain.Features
{
    public interface INotificationService
    {
        Task<Result<NotificationResponseDto>> SendToDeviceAsync(SendToDeviceRequestDto request);
        Task<Result<NotificationResponseDto>> SendToTopicAsync(SendToTopicRequestDto request);
    }
}
