using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDWithAuth.Models.Expense
{
    public class ExpenseDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ExpenseGuid { get; set; } = string.Empty;
        public string ExpenseId { get; set; } = string.Empty;   
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;   
        public string Proof { get; set; } = string.Empty;   
        public string ProofType { get; set; } = string.Empty;   
        public string AddedBy { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
        public string AddedIP { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedOn { get; set; }
        public string UpdatedIP { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CollectionMax { get; set; } = 0;
        public string CollectionMaxId { get; set; } = string.Empty;

    }
}
