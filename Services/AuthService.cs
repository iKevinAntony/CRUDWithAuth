using CRUDWithAuth.Data;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Helpers.StaticEnums;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Services.IServices;
using CRUDWithAuth.Services.IServices.AuthRoles;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace CRUDWithAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDBContext _conn;
        private readonly IActionContextAccessor _ipAddress;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthService(AppDBContext conn, IActionContextAccessor ipAddress,IJwtAuthManager jwtAuthManager)
        {
            _conn = conn;
            _ipAddress= ipAddress;
            _jwtAuthManager = jwtAuthManager;
        }
        public async Task<ResponseDTO> Login(LoginRequest requestDTO)
        {
            var result = new ResponseDTO();
            var encryptedPassword = CommonSettings.Encrypt(requestDTO.Password);
            var user = await _conn.UserLogin
                .Where(a => a.LoginId == requestDTO.UserId && a.Pwd == encryptedPassword)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                result.IsSuccess = false;
                result.ResponseCode = 404;
                result.Message = "Invalid user name or password";
                return result;
            }

            var claims = new[]
                            {
                                new Claim(ClaimTypes.Name, user.UserGuid ?? string.Empty),
                                new Claim(ClaimTypes.Sid, CommonSettings.IPAddress(_ipAddress)),
                                new Claim(ClaimTypes.Role, "User"),
                                new Claim("UserType",ClaimUserTypes.CAUSER.ToString())
                            };

            // Generate JWT token
            var jwtResult = _jwtAuthManager.GenerateUserokens(user.UserGuid, claims, _conn, "");
            var tokenDetails = new LoginResponseDTO
            {
                Status = user.Status ?? "Invalid",
                Role = RoleTypes.User.ToString(),
                UserName = user.UserGuid,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };

            result.Result = tokenDetails;
            result.IsSuccess = true;
            result.Message = "Logged in successfully";
            result.ResponseCode = 200;

            return result;
        }
    }
}
