using System.Text.Json.Serialization;

namespace CRUDWithAuth.Helpers
{
    public class UserJwtTokenConfig
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; } = string.Empty;

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; } = string.Empty;

        [JsonPropertyName("audience")]
        public string Audience { get; set; } = string.Empty;

        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; } = 0;

        [JsonPropertyName("refreshTokenExpiration")]
        public int RefreshTokenExpiration { get; set; } = 0;
    }
}
