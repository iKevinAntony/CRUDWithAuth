using CRUDWithAuth.Data;
using CRUDWithAuth.Models.AuthRoles;
using System.Security.Claims;

namespace CRUDWithAuth.Services.IServices.AuthRoles
{/// <summary>
/// Defines the contract for managing JWT authentication and refresh token operations.
/// Responsible for generating access tokens, issuing refresh tokens,
/// and refreshing expired tokens securely.
/// </summary>
    public interface IJwtAuthManager
    {/// <summary>
     /// Generates a new JWT access token and refresh token for the specified user.
     /// Also updates or inserts token information in the data store.
     /// </summary>
     /// <param name="uGuid">Unique identifier of the user.</param>
     /// <param name="claims">Claims to be embedded within the JWT token.</param>
     /// <param name="_conn">Database context used to persist token details.</param>
     /// <param name="token">Existing access token (used during refresh or reissue scenarios).</param>
     /// <returns>
     /// A <see cref="JwtAuthResult"/> containing the generated access token,
     /// refresh token, and associated user information.
     /// </returns>
        JwtAuthResult GenerateUserokens(string uGuid, Claim[] claims, AppDBContext _conn, string token);
        /// <summary>
        /// Refreshes an expired JWT access token using a valid refresh token.
        /// Validates the refresh token and regenerates a new access and refresh token pair.
        /// </summary>
        /// <param name="refreshToken">The refresh token issued during login.</param>
        /// <param name="accessToken">The expired or existing access token used to recover claims.</param>
        /// <param name="_conn">Database context used to update token records.</param>
        /// <returns>
        /// A <see cref="JwtAuthResult"/> containing the refreshed access token
        /// and a newly issued refresh token.
        /// </returns>
        JwtAuthResult UserRefresh(string refreshToken, string accessToken, AppDBContext _conn);
    }
}
