using System.ComponentModel.DataAnnotations;

namespace CRUDWithAuth.Models.DTO
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
