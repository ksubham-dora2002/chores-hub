using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ChoresHub.Application.DTOs;
using ChoresHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace ChoresHub.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;


        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDTO>> GetNotification([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid notification ID.");

            var result = await _notificationService.GetNotificationByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpGet()]
        public async Task<ActionResult<IList<NotificationDTO>>> GetAllNotifications([FromQuery] int pageSize)
        {
            var result = await _notificationService.GetAllNotificationsAsync(pageSize);

            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }
        [HttpGet("all-by-email")]
        public async Task<ActionResult<IList<NotificationDTO>>> GetAllNotificationsByUserEmail([FromQuery] int pageSize)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized("User email not found in cookies.");

            var result = await _notificationService.GetAllNotificationsByUserEmailAsync(userEmail, pageSize);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid notification ID.");

            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);
            return NoContent();
        }
        [HttpPost()]
        public async Task<ActionResult<NotificationDTO>> CreateNotification([FromBody] CreateNotificationDTO notificationDTO)
        {
            if (notificationDTO == null) return BadRequest("Notification can not be empty!");

            var result = await _notificationService.CreateNotificationAsync(notificationDTO);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpPut("update")]
        public async Task<ActionResult<NotificationDTO>> UpdateNotification([FromBody] NotificationDTO notificationDTO)
        {
            if (notificationDTO == null) return BadRequest("Notification can not be empty!");

            var result = await _notificationService.UpdateNotificationAsync(notificationDTO);
            if (!result.IsSuccess) return NotFound(result.Error);
            
            return Ok(result.Value);
        }

    }
}