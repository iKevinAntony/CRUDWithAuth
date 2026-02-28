namespace CRUDWithAuth.Services.IServices.Shared
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Defines the contract for file upload operations.
        /// Provides functionality to save files to a specified storage location
        /// and return the stored file path or name.
        /// </summary>
        Task<string> SaveFileAsync(IFormFile profilePic, string folderName, string reqFileName);
    }
}
