using CRUDWithAuth.Helpers;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Services.IServices.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static CRUDWithAuth.Helpers.ApiException;

namespace CRUDWithAuth.Controllers
{
    /// <summary>
    /// Provides endpoints for managing expenses in the CRUDWithAuth system.
    /// This controller supports creating, retrieving, updating, deleting,
    /// and filtering expenses with optional file uploads.
    /// </summary>
    /// <remarks>
    /// All endpoints require authenticated users under the single-role authorization model.
    /// File uploads (such as receipts or bills) are supported for expense records.
    /// </remarks>
    [Route("api/v1/expense")]
    [ApiController]
    [Authorize(Policy = "UserOnly")]
    [TypeFilter(typeof(CaptureExceptions))]
    [ApiExplorerSettings(GroupName = "user")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseController"/> class.
        /// </summary>
        /// <param name="expenseService">Service responsible for handling expense business logic.</param>
        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }
        /// <summary>
        /// Creates a new expense with optional file attachment (receipt/bill).
        /// </summary>
        /// <param name="request">Expense details along with optional file upload.</param>
        /// <returns>Returns the created expense details if successful.</returns>
        /// <response code="201">Expense created successfully.</response>
        /// <response code="409">Expense already exists or conflict occurred.</response>
        /// <response code="404">Referenced resource not found.</response>
        /// <response code="500">Internal server error.</response>
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
        /// <summary>
        /// Retrieves a filtered and paginated list of expenses.
        /// </summary>
        /// <param name="requestDTO">Filtering criteria including date range, category, and pagination details.</param>
        /// <returns>Returns a filtered list of expenses.</returns>
        /// <response code="200">Expenses retrieved successfully.</response>
        /// <response code="404">No matching expenses found.</response>
        /// <response code="500">Internal server error.</response>
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
        /// <summary>
        /// Retrieves a specific expense by its unique identifier.
        /// </summary>
        /// <param name="expenseGuid">Unique identifier of the expense.</param>
        /// <returns>Returns the expense details if found.</returns>
        /// <response code="200">Expense retrieved successfully.</response>
        /// <response code="404">Expense not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        public async Task<ActionResult> GetExpense([FromQuery][Required] string expenseGuid)
        {
            if (!ModelState.IsValid)
            {
                ServerResult.ThrowMissingJSON();
            }
            var result = await _expenseService.GetExpense(expenseGuid);
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
        /// <summary>
        /// Updates an existing expense record with new details and optional file attachment.
        /// </summary>
        /// <param name="request">Updated expense information.</param>
        /// <returns>Returns the updated expense details if successful.</returns>
        /// <response code="200">Expense updated successfully.</response>
        /// <response code="404">Expense not found.</response>
        /// <response code="409">Conflict occurred while updating.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut]
        public async Task<ActionResult> UpdateExpense([FromForm] ExpenseUpdateDTO request)
        {
            if (!ModelState.IsValid)
            {
                ServerResult.ThrowMissingJSON();
            }
            var result = await _expenseService.UpdateExpense(request);
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
        /// <summary>
        /// Deletes an expense record based on the provided unique identifier.
        /// </summary>
        /// <param name="expenseGuid">Unique identifier of the expense to be deleted.</param>
        /// <returns>Returns a success response if the expense is deleted.</returns>
        /// <response code="200">Expense deleted successfully.</response>
        /// <response code="404">Expense not found.</response>
        /// <response code="500">Internal server error.</response>

        [HttpDelete]
        public async Task<ActionResult> DeleteExpense([FromQuery][Required] string expenseGuid)
        {
            if (!ModelState.IsValid)
            {
                ServerResult.ThrowMissingJSON();
            }
            var result = await _expenseService.DeleteExpense(expenseGuid);
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


    }
}
