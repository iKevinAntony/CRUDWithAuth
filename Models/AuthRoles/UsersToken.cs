namespace CRUDWithAuth.Models.AuthRoles
{
    public class UsersToken
    {
        public int Id { get; set; }
        public string UserGuid { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenCreatedOn { get; set; }
        public DateTime TokenValidTill { get; set; }
        public DateTime RefreshTokenExpire { get; set; }
    }
}
