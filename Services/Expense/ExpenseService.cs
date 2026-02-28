using AutoMapper;
using Azure;
using CRUDWithAuth.Data;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Helpers.StaticEnums;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Models.Expense;
using CRUDWithAuth.Services.IServices.Expense;
using CRUDWithAuth.Services.IServices.Shared;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;

namespace CRUDWithAuth.Services.Expense
{/// <summary>
/// Service responsible for handling business logic related to Expense management.
/// Supports creating, retrieving, updating, deleting, and filtering expenses,
/// along with file upload handling and audit tracking.
/// </summary>
    public class ExpenseService : IExpenseService
    {
        private readonly AppDBContext _conn;
        private readonly IHttpContextAccessor _httpConext;
        private readonly IActionContextAccessor _ipAddress;
        private readonly IFileUploadService _fileUploadService;
        private readonly ServerUrls _serverUrls;
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseService"/> class.
        /// </summary>
        /// <param name="conn">Database context for expense persistence.</param>
        /// <param name="httpConext">HTTP context accessor used to retrieve current user information.</param>
        /// <param name="ipAddress">Action context accessor used to obtain request IP address.</param>
        /// <param name="fileUploadService">Service for handling file uploads.</param>
        /// <param name="configuration">Application configuration for database connection string.</param>
        /// <param name="serverUrls">Server URL configuration for constructing media file paths.</param>
        /// <param name="mapper">AutoMapper instance for mapping entities to DTOs.</param>
        public ExpenseService(AppDBContext conn, IHttpContextAccessor httpConext, IActionContextAccessor ipAddress, IFileUploadService fileUploadService, IConfiguration configuration, ServerUrls serverUrls, IMapper mapper)
        {
            _conn = conn;
            _connectionString = configuration.GetConnectionString("DBCon") ?? "";
            _httpConext = httpConext;
            _ipAddress = ipAddress;
            _fileUploadService = fileUploadService;
            _serverUrls = serverUrls;
            _mapper = mapper;
        }
        /// <summary>
        /// Adds a new expense record with optional attachment and audit information.
        /// Generates a unique expense ID and stores file metadata if provided.
        /// </summary>
        /// <param name="requestDTO">Expense request details including category, amount, notes, and optional attachment.</param>
        /// <returns>
        /// A <see cref="ResponseDTO"/> indicating success or failure along with status code and message.
        /// </returns>
        public async Task<ResponseDTO> AddExpense(ExpenseRequestDTO requestDTO)
        {
            var response = new ResponseDTO();
            var addedBy = _httpConext.HttpContext?.User?.Identity?.Name;
            addedBy = addedBy ?? "";
            DateTime dateTime = TimeStamps.UTCTime();
            string _ip = CommonSettings.IPAddress(_ipAddress);
            string expenseGuid = Guid.NewGuid().ToString();
            var maxId = await _conn.ExpenseDetails.MaxAsync(o => (int?)o.CollectionMax) ?? 0;
            var nextId = (maxId + 1).ToString("D4");
            var expenseId = "EXP" + nextId;

            if (requestDTO.Amount <= 0)
            {
                response.ResponseCode = StatusCodes.Status400BadRequest;
                response.IsSuccess = false;
                response.Message = "Amount must be greater than zero";
                return response;
            }
            string stored = "";
            string storedType = "";
            if (requestDTO.Attachment != null)
            {
                string fileName = "";
                Random rnd = new Random();
                var ext = Path.GetExtension(requestDTO.Attachment.FileName);
                var uniqueId = Guid.NewGuid().ToString("N");
                fileName = "proof-" + uniqueId + "_" + Path.GetFileNameWithoutExtension(requestDTO.Attachment.FileName).Replace(" ", "-").Replace(".", "") + "_proof" + ext;
                stored = await _fileUploadService.SaveFileAsync(requestDTO.Attachment, "ProofFiles", fileName);
                string fileExtension = Path.GetExtension(requestDTO.Attachment.FileName).ToLower();
                // Determine file type based on extension
                storedType = fileExtension.ToLower() switch
                {
                    // Images
                    ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".tiff" or ".svg" => "Image",

                    // Videos
                    ".mp4" or ".mov" or ".avi" or ".mkv" or ".wmv" or ".flv" or ".webm" => "Video",

                    // Audio
                    ".mp3" or ".wav" or ".aac" or ".flac" or ".ogg" or ".wma" or ".m4a" => "Audio",

                    // Documents
                    ".pdf" => "PDF",
                    ".doc" or ".docx" => "Word Document",
                    ".xls" or ".xlsx" => "Excel Spreadsheet",
                    ".ppt" or ".pptx" => "PowerPoint Presentation",
                    ".txt" => "Text File",
                    ".rtf" => "Rich Text File",
                    ".odt" => "OpenDocument Text",
                    ".ods" => "OpenDocument Spreadsheet",
                    ".odp" => "OpenDocument Presentation",
                    ".csv" => "CSV File",

                    // Archives
                    ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "Archive",

                    // Executables / Code
                    ".exe" or ".msi" => "Executable",
                    ".bat" or ".sh" => "Script",
                    ".js" => "JavaScript File",
                    ".py" => "Python File",
                    ".java" => "Java File",
                    ".c" or ".cpp" or ".h" or ".hpp" => "C/C++ Source File",
                    ".cs" => "C# Source File",
                    ".php" => "PHP File",
                    ".html" or ".htm" => "HTML File",
                    ".css" => "CSS File",

                    // Default
                    _ => "Unknown"
                };
            }
            ExpenseDetails _expense = new ExpenseDetails();
            _expense.ExpenseGuid = expenseGuid;
            _expense.ExpenseId = expenseId;
            _expense.CategoryName = requestDTO.CategoryName;
            _expense.Amount = requestDTO.Amount;
            _expense.Notes = requestDTO.Notes;
            _expense.Proof = stored;
            _expense.ProofType = storedType;
            _expense.AddedOn = dateTime;
            _expense.AddedBy = addedBy;
            _expense.AddedIP = _ip;
            _expense.UpdatedOn = dateTime;
            _expense.UpdatedBy = addedBy;
            _expense.UpdatedIP = _ip;
            _expense.Status = ExpenseStatuses.Active.ToString();
            _expense.CollectionMax = maxId + 1;
            _expense.CollectionMaxId = nextId;
            await _conn.ExpenseDetails.AddAsync(_expense);
            int _saved = await _conn.SaveChangesAsync();
            if (_saved > 0)
            {
                response.ResponseCode = StatusCodes.Status201Created;
                response.IsSuccess = true;
                response.Message = "Expense Added Successfully";
            }
            else
            {
                response.ResponseCode = StatusCodes.Status500InternalServerError;
                response.IsSuccess = false;
                response.Message = "Failed to add expense";
            }
            return response;

        }
        /// <summary>
        /// Retrieves a paginated and filtered list of expenses using a stored procedure.
        /// Also appends full file URLs for proof attachments.
        /// </summary>
        /// <param name="requestDTO">Filtering parameters including date range, search text, and pagination details.</param>
        /// <returns>A response containing a list of filtered expenses.</returns>
        public async Task<ResponseDTO> GetAllExpenses(ExpenseFilterDTO requestDTO)
        {
            var response = new ResponseDTO();
            var userGuid = _httpConext.HttpContext?.User?.Identity?.Name ?? "";
            requestDTO.PageSize = requestDTO.PageSize < 1 ? 10 : requestDTO.PageSize;
            requestDTO.PageNo = requestDTO.PageNo < 1 ? 1 : requestDTO.PageNo;
            requestDTO.FromDate = requestDTO.FromDate ?? TimeStamps.UTCTime().ToString("dd-MMM-yyyy");
            requestDTO.ToDate = requestDTO.ToDate ?? TimeStamps.UTCTime().AddMonths(-3).ToString("dd-MMM-yyyy");
            requestDTO.SParam = requestDTO.SParam ?? "";
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PageSize", requestDTO.PageSize);
                parameters.Add("@PageNo", requestDTO.PageNo);
                parameters.Add("@FromDate", requestDTO.FromDate);
                parameters.Add("@ToDate", requestDTO.ToDate);
                parameters.Add("@SParam", requestDTO.SParam);

                var expenses = (await connection.QueryAsync<ExpenseResponseDTO>(
                    "dbo.GetExpenseList",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                foreach (var expense in expenses)
                {
                    if (!string.IsNullOrEmpty(expense.Proof))
                    {
                        expense.Proof = _serverUrls.proofMediaFilePath + expense.Proof;
                    }
                }
                response.IsSuccess = true;
                response.Message = "Success";
                response.ResponseCode = StatusCodes.Status200OK;
                response.Result = expenses;

                return response;
            }
        }
        /// <summary>
        /// Retrieves a specific expense by its unique GUID.
        /// </summary>
        /// <param name="expenseGuid">Unique identifier of the expense.</param>
        /// <returns>Expense details if found; otherwise returns not found response.</returns>
        public async Task<ResponseDTO> GetExpense(string expenseGuid)
        {
            var response = new ResponseDTO();
            var userGuid = _httpConext.HttpContext?.User?.Identity?.Name ?? "";
            var expense = await _conn.ExpenseDetails.Where(e => e.ExpenseGuid == expenseGuid && e.Status != ExpenseStatuses.Deleted.ToString()).FirstOrDefaultAsync();
            if (expense == null)
            {
                response.IsSuccess = false;
                response.Message = "Expense not found";
                response.ResponseCode = StatusCodes.Status404NotFound;
                return response;
            }
            else
            {
                if (!string.IsNullOrEmpty(expense.Proof))
                {
                    expense.Proof = _serverUrls.proofMediaFilePath + expense.Proof;
                }
                response.IsSuccess = true;
                response.Message = "Success";
                response.ResponseCode = StatusCodes.Status200OK;
                response.Result = _mapper.Map<ExpenseResponseDTO>(expense);
                return response;
            }
        }
        /// <summary>
        /// Updates an existing expense record and optionally replaces its attachment.
        /// </summary>
        /// <param name="requestDTO">Updated expense details including GUID and optional new attachment.</param>
        /// <returns>Response indicating whether the update was successful.</returns>
        public async Task<ResponseDTO> UpdateExpense(ExpenseUpdateDTO requestDTO)
        {
            var response = new ResponseDTO();
            var expense = await _conn.ExpenseDetails.Where(e => e.ExpenseGuid == requestDTO.ExpenseGuid && e.Status != ExpenseStatuses.Deleted.ToString()).FirstOrDefaultAsync();
            if (expense == null)
            {
                response.IsSuccess = false;
                response.Message = "Expense not found";
                response.ResponseCode = StatusCodes.Status404NotFound;
                return response;
            }
            var addedBy = _httpConext.HttpContext?.User?.Identity?.Name;
            addedBy = addedBy ?? "";
            DateTime dateTime = TimeStamps.UTCTime();
            string _ip = CommonSettings.IPAddress(_ipAddress);
            if (requestDTO.Amount <= 0)
            {
                response.ResponseCode = StatusCodes.Status400BadRequest;
                response.IsSuccess = false;
                response.Message = "Amount must be greater than zero";
                return response;
            }
            string stored = expense.Proof;
            string storedType = expense.ProofType;
            if (requestDTO.Attachment != null)
            {
                string fileName = "";
                Random rnd = new Random();
                var ext = Path.GetExtension(requestDTO.Attachment.FileName);
                var uniqueId = Guid.NewGuid().ToString("N");
                fileName = "proof-" + uniqueId + "_" + Path.GetFileNameWithoutExtension(requestDTO.Attachment.FileName).Replace(" ", "-").Replace(".", "") + "_proof" + ext;
                stored = await _fileUploadService.SaveFileAsync(requestDTO.Attachment, "ProofFiles", fileName);
                string fileExtension = Path.GetExtension(requestDTO.Attachment.FileName).ToLower();
                // Determine file type based on extension
                storedType = fileExtension.ToLower() switch
                {
                    // Images
                    ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".tiff" or ".svg" => "Image",

                    // Videos
                    ".mp4" or ".mov" or ".avi" or ".mkv" or ".wmv" or ".flv" or ".webm" => "Video",

                    // Audio
                    ".mp3" or ".wav" or ".aac" or ".flac" or ".ogg" or ".wma" or ".m4a" => "Audio",

                    // Documents
                    ".pdf" => "PDF",
                    ".doc" or ".docx" => "Word Document",
                    ".xls" or ".xlsx" => "Excel Spreadsheet",
                    ".ppt" or ".pptx" => "PowerPoint Presentation",
                    ".txt" => "Text File",
                    ".rtf" => "Rich Text File",
                    ".odt" => "OpenDocument Text",
                    ".ods" => "OpenDocument Spreadsheet",
                    ".odp" => "OpenDocument Presentation",
                    ".csv" => "CSV File",

                    // Archives
                    ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "Archive",

                    // Executables / Code
                    ".exe" or ".msi" => "Executable",
                    ".bat" or ".sh" => "Script",
                    ".js" => "JavaScript File",
                    ".py" => "Python File",
                    ".java" => "Java File",
                    ".c" or ".cpp" or ".h" or ".hpp" => "C/C++ Source File",
                    ".cs" => "C# Source File",
                    ".php" => "PHP File",
                    ".html" or ".htm" => "HTML File",
                    ".css" => "CSS File",

                    // Default
                    _ => "Unknown"
                };
            }
            expense.CategoryName = requestDTO.CategoryName;
            expense.Amount = requestDTO.Amount;
            expense.Notes = requestDTO.Notes ?? "";
            expense.Proof = stored;
            expense.ProofType = storedType;
            expense.UpdatedOn = dateTime;
            expense.UpdatedBy = addedBy;
            expense.UpdatedIP = _ip;
            _conn.ExpenseDetails.Update(expense);
            int _saved = await _conn.SaveChangesAsync();
            if (_saved > 0)
            {
                response.ResponseCode = StatusCodes.Status200OK;
                response.IsSuccess = true;
                response.Message = "Expense Updated Successfully";
            }
            else
            {
                response.ResponseCode = StatusCodes.Status500InternalServerError;
                response.IsSuccess = false;
                response.Message = "Failed to update expense";
            }
            return response;

        }
        /// <summary>
        /// Soft deletes an expense by marking its status as Deleted.
        /// Maintains audit fields such as updated time, user, and IP address.
        /// </summary>
        /// <param name="expenseGuid">Unique identifier of the expense to delete.</param>
        /// <returns>Response indicating whether the delete operation was successful.</returns>
        public async Task<ResponseDTO> DeleteExpense(string expenseGuid)
        {
            var response = new ResponseDTO();
            var expense = await _conn.ExpenseDetails.Where(e => e.ExpenseGuid == expenseGuid && e.Status != ExpenseStatuses.Deleted.ToString()).FirstOrDefaultAsync();
            if (expense == null)
            {
                response.IsSuccess = false;
                response.Message = "Expense not found";
                response.ResponseCode = StatusCodes.Status404NotFound;
                return response;
            }
            var addedBy = _httpConext.HttpContext?.User?.Identity?.Name;
            addedBy = addedBy ?? "";
            DateTime dateTime = TimeStamps.UTCTime();
            string _ip = CommonSettings.IPAddress(_ipAddress);
            expense.Status = ExpenseStatuses.Deleted.ToString();
            expense.UpdatedOn = dateTime;
            expense.UpdatedBy = addedBy;
            expense.UpdatedIP = _ip;
            _conn.ExpenseDetails.Update(expense);
            int _saved = await _conn.SaveChangesAsync();
            if (_saved > 0)
            {
                response.ResponseCode = StatusCodes.Status200OK;
                response.IsSuccess = true;
                response.Message = "Expense Deleted Successfully";
            }
            else
            {
                response.ResponseCode = StatusCodes.Status500InternalServerError;
                response.IsSuccess = false;
                response.Message = "Failed to delete expense";
            }
            return response;

        }

    }
}
