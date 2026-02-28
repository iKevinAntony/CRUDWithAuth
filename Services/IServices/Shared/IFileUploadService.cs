namespace CRUDWithAuth.Services.IServices.Shared
{
    public interface IFileUploadService
    {
        Task<string> SaveFileAsync(IFormFile profilePic, string folderName, string reqFileName);
    }
}
