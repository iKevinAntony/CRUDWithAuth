using System.ComponentModel.DataAnnotations;

namespace CRUDWithAuth.Models.DTO
{
    public class ExpenseRequestDTO
    {
        /// <summary>
        /// Gets or sets the category name of the expense.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount of the expense.
        /// </summary>
        [Required]
        public decimal Amount { get; set; } = 0;
        /// <summary>
        /// Gets or sets the notes of the expense.
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Gets or sets the Proof of the expense.
        /// </summary>
        public IFormFile Attachment { get; set; }
    }
}
