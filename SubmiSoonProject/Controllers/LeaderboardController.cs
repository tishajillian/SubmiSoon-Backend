using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.Services;

namespace SubmiSoonProject.Controllers
{
    [ApiController]
    [Route("")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [Authorize]
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? sort_by = null,
            [FromQuery] string? sort_order = "desc")
        {
            if (page < 1)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Page must be at least 1"
                });
            }

            if (size < 1 || size > 100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Size must be between 1 and 100"
                });
            }

            if (!string.IsNullOrEmpty(sort_order) &&
                sort_order.ToLower() != "asc" &&
                sort_order.ToLower() != "desc")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Sort order must be 'asc' or 'desc'"
                });
            }

            try
            {
                var result = await _leaderboardService.GetLeaderboardAsync(page, size, sort_by, sort_order);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while fetching the leaderboard"
                });
            }
        }
    }
}
