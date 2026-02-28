using CRUDWithAuth.Models.DTO;

namespace CRUDWithAuth.Services.IServices.Expense
{
    public interface IExpenseService
    {
        Task<ResponseDTO> AddExpense(ExpenseRequestDTO requestDTO);
    }
}
