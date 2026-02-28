using System.Text.Json.Serialization;

namespace CRUDWithAuth.Models.DTO
{
    public class RefreshTokenRequestDTO
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
