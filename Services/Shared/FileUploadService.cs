using CRUDWithAuth.Helpers;
using CRUDWithAuth.Services.IServices.Shared;
using Serilog;

namespace CRUDWithAuth.Services.Shared
{
    /// <summary>
    /// Provides file upload and management services, including saving, deleting, 
    /// and validating uploaded images.
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadService"/> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment, used to access the root path for file storage.</param>
        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Saves an uploaded file asynchronously to the specified folder.
        /// </summary>
        /// <param name="profilePic">The uploaded file.</param>
        /// <param name="folderName">The name of the folder where the file will be saved.</param>
        /// <returns>
        /// The generated file name on the server, or <c>null</c> if an exception occurs.
        /// </returns>
        public async Task<string> SaveFileAsync(IFormFile profilePic, string folderName, string reqFileName)
        {
            try
            {
                string filePath = GetFilePath(folderName);
                EnsureDirectoryExists(filePath);
                string imagePath = Path.Combine(filePath, reqFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                {
                    await profilePic.CopyToAsync(stream);
                }
                return reqFileName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception occurred in {nameof(FileUploadService)}");
                return "Failed";
            }
        }

        /// <summary>
        /// Deletes an image file from the server by file name.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        public void DeleteImage(string fileName)
        {
            try
            {
                string filePath = GetFilePath();
                string fullPath = Path.Combine(filePath, fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    Log.Information($"Deleted image: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error deleting image file: {fileName}");
            }
        }

        /// <summary>
        /// Checks if the uploaded thumbnail image has a valid format and size.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <returns>
        /// Returns <c>"Format"</c> if the file type is not supported, 
        /// <c>"Size"</c> if the file is too large (currently disabled), 
        /// or an empty string if valid.
        /// </returns>
        public string CheckThumbFormatAndSize(IFormFile file)
        {
            #region ThumbImage
            string thumbImg = "";

            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!(fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" ||
                  fileExtension == ".gif" || fileExtension == ".webp"))
            {
                return "Format";
            }
            //if (file.Length > 1050000)
            //{
            //    return "Size";
            //}

            #endregion

            return thumbImg;
        }

        /// <summary>
        /// Gets the absolute file path for the specified folder inside the web root.
        /// </summary>
        /// <param name="folderName">The folder name. Optional.</param>
        /// <returns>The full path of the folder, or an empty string if an error occurs.</returns>
        private string GetFilePath(string folderName = "")
        {
            try
            {
                return Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception occurred in {nameof(FileUploadService)}");
                return "";
            }
        }

        /// <summary>
        /// Ensures that the given directory path exists, creating it if necessary.
        /// </summary>
        /// <param name="path">The directory path to check or create.</param>
        private void EnsureDirectoryExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Exception occurred in {nameof(FileUploadService)}");
                }
            }
        }

        /// <summary>
        /// Determines whether the provided file extension corresponds to a supported image type.
        /// </summary>
        /// <param name="fileExtension">The file extension to check (including the dot).</param>
        /// <returns><c>true</c> if the file extension is supported; otherwise, <c>false</c>.</returns>
        private bool IsSupportedImageType(string fileExtension)
        {
            var supportedTypes = new[] { ".jpg", ".jpeg", ".png" };
            return supportedTypes.Contains(fileExtension);
        }
    }
}
