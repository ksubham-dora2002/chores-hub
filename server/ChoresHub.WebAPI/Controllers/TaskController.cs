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
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService _taskService = taskService;

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> GetTask(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid task ID.");

            var result = await _taskService.GetTaskByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpGet()]
        public async Task<ActionResult<IList<TaskDTO>>> GetAllTasks([FromQuery] int pageSize)
        {
            var result = await _taskService.GetAllTasksAsync(pageSize);

            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }
        [HttpGet("all-by-email")]
        public async Task<ActionResult<IList<TaskDTO>>> GetAllTasksByUserEmail([FromQuery] int pageSize)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized("User email not found in cookies.");

            var result = await _taskService.GetAllTasksByUserEmailAsync(userEmail, pageSize);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid task ID.");

            var result = await _taskService.DeleteTaskAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost()]
        public async Task<ActionResult<TaskDTO>> CreateTask([FromBody] CreateTaskDTO taskDTO)
        {
            if (taskDTO == null) return BadRequest("Task can not be empty!");

            var result = await _taskService.CreateTaskAsync(taskDTO);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpPut("update")]
        public async Task<ActionResult<TaskDTO>> UpdateTask([FromBody] TaskDTO taskDTO)
        {
            if (taskDTO == null) return BadRequest("Task can not be empty!");

            var result = await _taskService.UpdateTaskAsync(taskDTO);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }


    }
}