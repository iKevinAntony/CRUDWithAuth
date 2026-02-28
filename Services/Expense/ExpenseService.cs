using CRUDWithAuth.Data;
using CRUDWithAuth.Helpers;
using CRUDWithAuth.Helpers.StaticEnums;
using CRUDWithAuth.Models.DTO;
using CRUDWithAuth.Models.Expense;
using CRUDWithAuth.Services.IServices.Expense;
using CRUDWithAuth.Services.IServices.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRUDWithAuth.Services.Expense
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDBContext _conn;
        private readonly IHttpContextAccessor _httpConext;
        private readonly IActionContextAccessor _ipAddress;
        private readonly IFileUploadService _fileUploadService;
        public ExpenseService(AppDBContext conn, IHttpContextAccessor httpConext, IActionContextAccessor ipAddress, IFileUploadService fileUploadService)
        {
            _conn = conn;
            _httpConext = httpConext;
            _ipAddress = ipAddress;
            _fileUploadService = fileUploadService;
        }
        public async Task<ResponseDTO> AddExpense(ExpenseRequestDTO requestDTO)
        {
            var result = new ResponseDTO();
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
                result.ResponseCode = StatusCodes.Status400BadRequest;
                result.IsSuccess = false;
                result.Message = "Amount must be greater than zero";
                return result;
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
                result.ResponseCode = StatusCodes.Status201Created;
                result.IsSuccess = true;
                result.Message = "Expense Added Successfully";
            }
            else
            {
                result.ResponseCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                result.Message = "Failed to add expense";
            }
            return result;

        }
    }
}
