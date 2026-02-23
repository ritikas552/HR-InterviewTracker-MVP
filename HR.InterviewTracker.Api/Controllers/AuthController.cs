using HR.InterviewTracker.Application.DTOs;
using HR.InterviewTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.InterviewTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return Unauthorized(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Ok(new
        {
            success = true,
            data = new { userId, email, role, name }
        });
    }

    // POST: api/auth/create-user
    [HttpPost("create-user")]
    [Authorize(Roles = "Admin")] // Only Admin can create users
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        try
        {
            var response = await _authService.CreateUserAsync(request);
            return Ok(new
            {
                success = true,
                message = "User created successfully",
                data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User creation failed");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // GET: api/auth/users
    [HttpGet("users")]
    [Authorize(Roles = "Admin")] // Only Admin can view all users
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(new
            {
                success = true,
                data = users
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // GET: api/auth/users/{id}
    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // DELETE: api/auth/users/{id}
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        try
        {
            var result = await _authService.DeactivateUserAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate user");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}