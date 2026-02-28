using CRUDWithAuth.Models.DTO;

namespace CRUDWithAuth.Services.IServices.Expense
{
    public interface IExpenseService
    {
        Task<ResponseDTO> AddExpense(ExpenseRequestDTO requestDTO);
        Task<ResponseDTO> GetAllExpenses(ExpenseFilterDTO requestDTO);
        Task<ResponseDTO> GetExpense(string expenseGuid);
        Task<ResponseDTO> UpdateExpense(ExpenseUpdateDTO requestDTO);
        Task<ResponseDTO> DeleteExpense(string expenseGuid);
    }
}
