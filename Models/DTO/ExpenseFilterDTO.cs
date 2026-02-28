namespace CRUDWithAuth.Models.DTO
{
    public class ExpenseFilterDTO
    {
        public int PageSize { get; set; }= 10;
        public int PageNo { get; set; } = 1;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string SParam { get; set; } = string.Empty;
    }
}
