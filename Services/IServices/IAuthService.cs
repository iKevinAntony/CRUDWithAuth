using CRUDWithAuth.Models.DTO;

namespace CRUDWithAuth.Services.IServices
{
    /// <summary>
    /// Defines the contract for user authentication operations.
    /// Provides methods for validating user credentials and issuing authentication responses.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// Validates the user and returns an authentication response containing token details if successful.
        /// </summary>
        /// <param name="requestDTO">Login request containing username and password.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> indicating whether authentication was successful,
        /// along with relevant status code, message, and token information.
        /// </returns>
        Task<ResponseDTO> Login(LoginRequest requestDTO);
    }
}
