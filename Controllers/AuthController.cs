using CRUDWithAuth.Helpers;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CRUDWithAuth.Helpers.ApiException;

namespace CRUDWithAuth.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    [TypeFilter(typeof(CaptureExceptions))]
    [ApiExplorerSettings(GroupName = "user")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
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
    }
}
