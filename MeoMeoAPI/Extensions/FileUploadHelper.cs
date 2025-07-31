
using MeoMeo.Contract.Commons;

namespace MeoMeo.API.Extensions
{
    public static class FileUploadHelper
    {
        public static async Task<List<FileUploadResult>> UploadFilesAsync(
            IWebHostEnvironment env,
            List<IFormFile> files,
            string folderName,
            Guid objectId,
            List<string>? acceptedExtensions = null,
            bool createSubFolderPerObject = true,
            long maxFileSizeInBytes = 20 * 1024 * 1024 // default 20 MB
        )
        {
            var results = new List<FileUploadResult>();

            if (files == null || !files.Any())
                return results;

            var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folderPath = createSubFolderPerObject
                ? Path.Combine(webRootPath, folderName, objectId.ToString())
                : Path.Combine(webRootPath, folderName);

            var relativeFolderPath = createSubFolderPerObject
                ?Path.Combine(folderName, objectId.ToString())
                : folderName.Replace("\\", "/");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            try
            {
                foreach (var file in files)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    if (file.Length > maxFileSizeInBytes)
                        continue;

                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant().TrimStart('.');

                    if (acceptedExtensions != null && !acceptedExtensions.Contains(extension))
                        continue;

                    var fileName = GenerateSafeFileName(file.FileName);
                    var fullPath = Path.Combine(folderPath, fileName);
                    var relativePath = Path.Combine(relativeFolderPath, fileName).Replace("\\", "/");

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    results.Add(new FileUploadResult
                    {
                        FileName = fileName,
                        FullPath = fullPath,
                        RelativePath = relativePath,
                        FileType = GetFileType(extension),
                        Size = file.Length
                    });
                }

                return results;
            }
            catch
            {
                foreach (var uploadedFile in results)
                {
                    if (File.Exists(uploadedFile.FullPath))
                    {
                        try { File.Delete(uploadedFile.FullPath); } catch { }
                    }
                }
                throw;
            }
        }
        public static void DeleteUploadedFiles(IWebHostEnvironment env, List<FileUploadResult> uploadedFiles)
        {
            var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            foreach (var file in uploadedFiles)
            {
                // Tạo đường dẫn tuyệt đối từ relative path
                var absolutePath = Path.Combine(webRootPath, file.RelativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(absolutePath))
                {
                    try
                    {
                        File.Delete(absolutePath);
                    }
                    catch
                    {
                        // Có thể log lại nếu cần
                    }
                }
            }
        }
        private static readonly Dictionary<string, int> ExtensionToFileType = new(StringComparer.OrdinalIgnoreCase)
        {
            // 0 = Image
            [".jpg"] = 0,
            [".jpeg"] = 0,
            [".png"] = 0,
            [".gif"] = 0,
            [".webp"] = 0,
            [".bmp"] = 0,
            [".svg"] = 0,

            // 1 = Video
            [".mp4"] = 1,
            [".mov"] = 1,
            [".avi"] = 1,
            [".mkv"] = 1,
            [".webm"] = 1,

            // 2 = Document
            [".pdf"] = 2,
            [".doc"] = 2,
            [".docx"] = 2,
            [".xls"] = 2,
            [".xlsx"] = 2,
            [".csv"] = 2
        };


        private static int? GetFileType(string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;

            return ExtensionToFileType.TryGetValue(extension, out var fileType) ? fileType : null;
        }
        private static string GenerateSafeFileName(string originalFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);
            var safeName = $"{Guid.NewGuid():N}_{fileName}{extension}";
            return safeName.Replace(" ", "_");
        }
    }

   
}
