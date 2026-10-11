using Firebase_Csharp.Domain.Features;
using Firebase_Csharp.Domain.Features.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Firebase_Csharp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send-to-device")]
        public async Task<IActionResult> SendToDevice([FromBody] SendToDeviceRequestDto sendToDeviceRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _notificationService.SendToDeviceAsync(sendToDeviceRequestDto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("send-to-topic")]
        public async Task<IActionResult> SendToTopic([FromBody] SendToTopicRequestDto sendToTopicRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _notificationService.SendToTopicAsync(sendToTopicRequestDto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
