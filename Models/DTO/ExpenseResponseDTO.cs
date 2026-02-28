namespace CRUDWithAuth.Models.DTO
{
    public class ExpenseResponseDTO
    {
        public int RowNumber { get; set; }
        public int TotalCount { get; set; }
        public string ExpenseGuid { get; set; } = string.Empty;
        public string ExpenseId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;
        public string Proof { get; set; } = string.Empty;
        public string ProofType { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
