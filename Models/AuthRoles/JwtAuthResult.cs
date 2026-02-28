using System.Text.Json.Serialization;

namespace CRUDWithAuth.Models.AuthRoles
{
    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public RefreshToken RefreshToken { get; set; }

        [JsonPropertyName("UserName")]
        public string UserName { get; set; }
    }
}
