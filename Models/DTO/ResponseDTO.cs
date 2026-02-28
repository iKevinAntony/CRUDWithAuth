namespace CRUDWithAuth.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int ResponseCode { get; set; }
    }
}
