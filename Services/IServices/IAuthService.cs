using CRUDWithAuth.Models.DTO;

namespace CRUDWithAuth.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDTO> Login(LoginRequest requestDTO);
    }
}
