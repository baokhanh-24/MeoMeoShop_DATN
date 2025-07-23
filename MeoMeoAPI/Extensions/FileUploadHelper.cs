
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
                ?Path.Combine("Uploads", folderName).Replace("\\", "/")
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
        public static void DeleteUploadedFiles(List<FileUploadResult> uploadedFiles)
        {
            foreach (var file in uploadedFiles)
            {
                if (File.Exists(file.FullPath))
                {
                    try { File.Delete(file.FullPath); } catch { }
                }
            }
        }
        private static readonly Dictionary<string, int> ExtensionToFileType = new(StringComparer.OrdinalIgnoreCase)
        {
            // 1 = Image
            [".jpg"] = 1, [".jpeg"] = 1, [".png"] = 1, [".gif"] = 1, [".webp"] = 1, [".bmp"] = 1, [".svg"] = 1,

            // 2 = Video
            [".mp4"] = 2, [".mov"] = 2, [".avi"] = 2, [".mkv"] = 2, [".webm"] = 2,

            // 3 = Document
            [".pdf"] = 3, [".doc"] = 3, [".docx"] = 3, [".xls"] = 3, [".xlsx"] = 3, [".csv"] = 3
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
