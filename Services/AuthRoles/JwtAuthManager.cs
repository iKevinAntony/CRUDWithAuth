using CRUDWithAuth.Data;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Models.AuthRoles;
using CRUDWithAuth.Services.IServices.AuthRoles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRUDWithAuth.Services.AuthRoles
{
    /// <summary>
    /// Manages JWT authentication and refresh token lifecycle for users.
    /// Responsible for generating access tokens, issuing refresh tokens,
    /// validating tokens, and refreshing expired JWT tokens securely.
    /// </summary>
    public class JwtAuthManager : IJwtAuthManager
    {
        /// <summary>
        /// Gets a read-only dictionary containing all active refresh tokens mapped by token string.
        /// </summary>
        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;
        private readonly UserJwtTokenConfig _userJwtTokenConfig;
        private readonly byte[] _usersecret;
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthManager"/> class.
        /// </summary>
        /// <param name="userJwtTokenConfig">Configuration containing issuer, audience, secret, and expiration settings.</param>
        public JwtAuthManager(UserJwtTokenConfig userJwtTokenConfig)
        {
            _userJwtTokenConfig = userJwtTokenConfig;
            _usersecret = Encoding.ASCII.GetBytes(userJwtTokenConfig.Secret);
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
        }
        #region User
        /// <summary>
        /// Generates JWT and refresh tokens for  user.
        /// </summary>
        /// <param name="uGuid">User GUID.</param>
        /// <param name="claims">Claims to include in the token.</param>
        /// <param name="_conn">Database context.</param>
        /// <param name="token">Existing token (optional).</param>
        public JwtAuthResult GenerateUserokens(string uGuid, Claim[] claims, AppDBContext _conn, string token)
        {
            var apiUsers = _conn.UsersToken.Where(x => x.Token == token).FirstOrDefault();
            DateTime timeNow = TimeStamps.UTCTime();
            DateTime tokenValidTill = timeNow.AddMinutes(_userJwtTokenConfig.AccessTokenExpiration);
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _userJwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _userJwtTokenConfig.Audience : string.Empty,
                claims,
                expires: tokenValidTill,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_usersecret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                UserName = uGuid,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = tokenValidTill.AddMinutes(_userJwtTokenConfig.RefreshTokenExpiration)
            };


            if (apiUsers != null)
            {
                _conn.Entry(apiUsers).State = EntityState.Detached;
                UsersToken aPI = new UsersToken();
                aPI.Id = apiUsers.Id;
                _conn.Attach(aPI);
                aPI.Token = accessToken;
                aPI.RefreshToken = refreshToken.TokenString;
                aPI.RefreshTokenExpire = refreshToken.ExpireAt;
                aPI.TokenCreatedOn = timeNow;
                aPI.TokenValidTill = tokenValidTill;
                _conn.SaveChanges();
            }
            else
            {
                UsersToken aPI = new UsersToken();
                aPI.UserGuid = uGuid;
                aPI.Token = accessToken;
                aPI.RefreshToken = refreshToken.TokenString;
                aPI.RefreshTokenExpire = refreshToken.ExpireAt;
                aPI.TokenCreatedOn = timeNow;
                aPI.TokenValidTill = tokenValidTill;
                _conn.Add(aPI);
                _conn.SaveChanges();
            }

            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, __) => refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = uGuid,
            };
        }

        /// <summary>
        /// Refreshes access token using a valid refresh token for User.
        /// </summary>
        public JwtAuthResult UserRefresh(string refreshToken, string accessToken, AppDBContext _conn)
        {
            DateTime timeNow = TimeStamps.UTCTime();
            var (principal, jwtToken) = DecodeUserJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = principal.Identity?.Name;
            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < timeNow)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return GenerateUserokens(userName, principal.Claims.ToArray(), _conn, accessToken); // need to recover the original claims
        }

        /// <summary>
        /// Decodes and validates the JWT token for Customer.
        /// </summary>
        public (ClaimsPrincipal, JwtSecurityToken) DecodeUserJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _userJwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_usersecret),
                        ValidAudience = _userJwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }
        #endregion
        /// <summary>
        /// Generates a secure random refresh token string using a cryptographic random number generator.
        /// </summary>
        /// <returns>A base64-encoded random string representing the refresh token.</returns>
        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
