using CRUDWithAuth.Data;
using CRUDWithAuth.Models.AuthRoles;
using System.Security.Claims;

namespace CRUDWithAuth.Services.IServices.AuthRoles
{
    public interface IJwtAuthManager
    {
        JwtAuthResult GenerateUserokens(string uGuid, Claim[] claims, AppDBContext _conn, string token);
    }
}
