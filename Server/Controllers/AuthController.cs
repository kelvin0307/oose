using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseApiController
{
    /// <summary>
    /// Login a teacher by email for identification purposes
    /// </summary>
    /// <param name="loginRequest">Login request containing teacher email</param>
    /// <returns>Teacher information if login is successful</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginRequest)
    {
        if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        var response = await authService.LoginTeacherByEmail(loginRequest.Email);

        if (!response.Success)
            return HandleResponse(response);

        // Create claims and sign in using cookie authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, response.Result.Id.ToString()),
            new Claim(ClaimTypes.Name, response.Result.FirstName + " " + response.Result.LastName),
            new Claim(ClaimTypes.Email, response.Result.Email),
            new Claim("TeacherCode", response.Result.TeacherCode)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return Ok(response.Result);
    }

    /// <summary>
    /// Login a teacher by email using query parameter
    /// </summary>
    /// <param name="email">Teacher email address</param>
    /// <returns>Teacher information if login is successful</returns>
    [AllowAnonymous]
    [HttpPost("login-by-email")]
    public async Task<IActionResult> LoginByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        var response = await authService.LoginTeacherByEmail(email);

        if (!response.Success)
            return HandleResponse(response);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, response.Result.Id.ToString()),
            new Claim(ClaimTypes.Name, response.Result.FirstName + " " + response.Result.LastName),
            new Claim(ClaimTypes.Email, response.Result.Email),
            new Claim("TeacherCode", response.Result.TeacherCode)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return Ok(response.Result);
    }

    /// <summary>
    /// Logout the currently signed-in teacher
    /// </summary>
    /// <returns>No content response</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}