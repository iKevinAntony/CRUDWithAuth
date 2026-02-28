namespace CRUDWithAuth.Models.DTO
{
    public class LoginResponseDTO
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
    }
}
