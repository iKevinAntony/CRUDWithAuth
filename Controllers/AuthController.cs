using CRUDWithAuth.Data;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Helpers.StaticEnums;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Services.IServices;
using CRUDWithAuth.Services.IServices.AuthRoles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static CRUDWithAuth.Helpers.ApiException;

namespace CRUDWithAuth.Controllers
{/// <summary>
/// Provides authentication endpoints for the CRUDWithAuth system.
/// This controller handles user login and JWT refresh token operations
/// to ensure secure access to protected resources.
/// </summary>
/// <remarks>
/// All users share a single role model. Authentication is performed using JWT,
/// and refresh tokens are used to renew expired access tokens securely.
/// </remarks>
    [Route("api/v1/auth")]
    [ApiController]
    [TypeFilter(typeof(CaptureExceptions))]
    [ApiExplorerSettings(GroupName = "user")]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _conn;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="conn">Database context used for authentication and token validation.</param>
        /// <param name="authService">Service responsible for handling login operations.</param>
        /// <param name="logger">Logger instance for tracking authentication events.</param>
        /// <param name="jwtAuthManager">Manager responsible for generating and refreshing JWT tokens.</param>
        public AuthController(AppDBContext conn, IAuthService authService, ILogger<AuthController> logger, IJwtAuthManager jwtAuthManager)
        {
            _conn = conn;
            _authService = authService;
            _logger = logger;
            _jwtAuthManager = jwtAuthManager;
        }
        /// <summary>
        /// Authenticates the user and generates JWT access and refresh tokens.
        /// </summary>
        /// <param name="request">Login credentials containing username and password.</param>
        /// <returns>
        /// Returns a JWT access token and refresh token if authentication succeeds;
        /// otherwise returns appropriate error responses.
        /// </returns>
        /// <response code="200">Login successful and tokens generated.</response>
        /// <response code="404">User does not exist.</response>
        /// <response code="409">User conflict or invalid state.</response>
        /// <response code="500">Internal server error.</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ServerResult.ThrowMissingJSON();
                }
                var result = await _authService.Login(request);
                if (result == null)
                {
                    ServerResult.ThrowServerError("There is some problem now. Please try after some time.");
                    return StatusCode(500, result);
                }
                else if (!result.IsSuccess)
                {
                    if (result.ResponseCode == 409)
                    {
                        ServerResult.ThrowAlreadyExists(result.Message);
                    }
                    else if (result.ResponseCode == 404)
                    {
                        ServerResult.ThrowDoesNotExist(result.Message);
                    }
                    else
                    {
                        ServerResult.ThrowServerError(result.Message);
                    }
                    return StatusCode(500, result);
                }
                else
                {
                    return StatusCode(200, result);
                }
            }
            catch (Exception ex)
            {
                ServerResult.ThrowServerError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Refreshes the expired JWT access token using a valid refresh token.
        /// </summary>
        /// <param name="request">DTO containing the refresh token issued during login.</param>
        /// <returns>
        /// Returns a new access token and refresh token pair if validation succeeds.
        /// </returns>
        /// <remarks>
        /// This endpoint requires authorization and validates the refresh token
        /// against stored records to prevent token misuse.
        /// </remarks>
        /// <response code="200">Token refreshed successfully.</response>
        /// <response code="401">Invalid or expired refresh token.</response>
        [Authorize(Policy = "UserOnly")]
        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("CustomerScheme", "access_token");

                var jwtResult = _jwtAuthManager.UserRefresh(request.RefreshToken, accessToken, _conn);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResponseDTO
                {
                    UserName = jwtResult.UserName,
                    Role = RoleTypes.User.ToString(),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    Status = "Active"
                });
            }
            catch (SecurityTokenException e)
            {
                ServerResult.ThrowInvalidToken();
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
    }
}
