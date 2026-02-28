using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDWithAuth.Models.Auth
{
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserGuid { get; set; }
        public string? LoginId { get; set; }
        public string? Pwd { get; set; }
        public string? LastLoggedIn { get; set; }
        public string? LastLoggedIp { get; set; }
        public DateTime? AddedOn { get; set; }
        public string AddedIP { get; set; }
        public string? ResetId { get; set; }
        public string? ResetOn { get; set; }
        public string? ResetIp { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? UpdatedIp { get; set; }
        public string Status { get; set; }
    }
}
