namespace SubmiSoonProject.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SubmiSoonProject.DTOs.Auth;
    using SubmiSoonProject.DTOs.Common;
    using SubmiSoonProject.Exceptions;
    using SubmiSoonProject.Services;

    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.AuthenticateUser(request);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "USER_NOT_FOUND",
                        Message = ex.Message
                    }
                });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INVALID_CREDENTIALS",
                        Message = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "An unexpected error occurred"
                    }
                });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true, message = "Logged out successfully" });
        }

    }
}