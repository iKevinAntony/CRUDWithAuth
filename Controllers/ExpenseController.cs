using CRUDWithAuth.Helpers;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Services.IServices.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CRUDWithAuth.Helpers.ApiException;

namespace CRUDWithAuth.Controllers
{
    [Route("api/v1/expense")]
    [ApiController]
    [Authorize(Policy = "UserOnly")]
    [TypeFilter(typeof(CaptureExceptions))]
    [ApiExplorerSettings(GroupName = "user")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }
        [HttpPost]
        public async Task<ActionResult> AddExpense([FromForm] ExpenseRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                ServerResult.ThrowMissingJSON();
            }
            var result = await _expenseService.AddExpense(request);
            if (result == null)
            {
                ServerResult.ThrowServerError("There is some problem now. Please try after some time.");
                return StatusCode(500, result);
            }
            else if (!result.IsSuccess)
            {
                if (result.ResponseCode == 409)
                {
                    ServerResult.ThrowAlreadyExists(result.Message);
                }
                else if (result.ResponseCode == 404)
                {
                    ServerResult.ThrowDoesNotExist(result.Message);
                }
                else
                {
                    ServerResult.ThrowServerError(result.Message);
                }
                return StatusCode(500, result);
            }
            else
            {
                return StatusCode(201, result);
            }
        }

        [HttpPost]
        [Route("filters")]
        public async Task<ActionResult> GetAllExpenses([FromBody] ExpenseFilterDTO requestDTO)
        {
            var result = await _expenseService.GetAllExpenses(requestDTO);
            if (result == null)
            {
                ServerResult.ThrowServerError("There is some problem now. Please try after some time.");
                return StatusCode(500, result);
            }
            else if (!result.IsSuccess)
            {
                if (result.ResponseCode == 409)
                {
                    ServerResult.ThrowAlreadyExists(result.Message);
                }
                else if (result.ResponseCode == 404)
                {
                    ServerResult.ThrowDoesNotExist(result.Message);
                }
                else
                {
                    ServerResult.ThrowServerError(result.Message);
                }
                return StatusCode(500, result);
            }
            else
            {
                return StatusCode(200, result);
            }
        }
    }
}
