using CRUDWithAuth.Models.DTO;

namespace CRUDWithAuth.Services.IServices.Expense
{
    /// <summary>
    /// Defines the contract for expense management operations.
    /// Provides methods for creating, retrieving, updating, deleting,
    /// and filtering expense records within the system.
    /// </summary>
    public interface IExpenseService
    {
        /// <summary>
        /// Adds a new expense record with optional attachment and audit details.
        /// </summary>
        /// <param name="requestDTO">Expense data including category, amount, notes, and optional file.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> indicating success or failure of the operation,
        /// along with status code and message.
        /// </returns>
        Task<ResponseDTO> AddExpense(ExpenseRequestDTO requestDTO);
        /// <summary>
        /// Retrieves a filtered and paginated list of expenses based on the provided criteria.
        /// </summary>
        /// <param name="requestDTO">
        /// Filter parameters including date range, search term, page number, and page size.
        /// </param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> containing the filtered list of expenses and operation status.
        /// </returns>
        Task<ResponseDTO> GetAllExpenses(ExpenseFilterDTO requestDTO);
        /// <summary>
        /// Retrieves a specific expense by its unique identifier (GUID).
        /// </summary>
        /// <param name="expenseGuid">The unique GUID of the expense to retrieve.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> containing the expense details if found;
        /// otherwise returns a not found response.
        /// </returns>
        Task<ResponseDTO> GetExpense(string expenseGuid);
        /// <summary>
        /// Updates an existing expense record and optionally replaces its attachment.
        /// </summary>
        /// <param name="requestDTO">Updated expense information including the expense GUID.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> indicating whether the update operation was successful.
        /// </returns>
        Task<ResponseDTO> UpdateExpense(ExpenseUpdateDTO requestDTO);
        /// <summary>
        /// Deletes (soft delete) an expense record by marking its status as deleted.
        /// </summary>
        /// <param name="expenseGuid">The unique GUID of the expense to delete.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> indicating whether the delete operation was successful.
        /// </returns>
        Task<ResponseDTO> DeleteExpense(string expenseGuid);
    }
}
