using Microsoft.AspNetCore.Mvc;
using MeoMeo.Contract.Commons;

namespace MeoMeo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment environment;

    public UploadsController(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    [HttpPost("upload/image")]
    public IActionResult UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "Invalid file type. Only images are allowed." });
            }

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { error = "File size too large. Maximum 10MB allowed." });
            }

            // Generate unique filename
            var fileName = $"product-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "products");
            
            // Create directory if not exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return response in format expected by AntDesign Upload
            var fileUrl = $"/uploads/products/{fileName}";
            
            return Ok(new
            {
                status = "done",
                url = fileUrl,
                name = fileName,
                uid = Guid.NewGuid().ToString()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("upload/multiple")]
    public IActionResult UploadMultipleImages(IFormFile[] files)
    {
        try
        {
            if (files == null || files.Length == 0)
            {
                return BadRequest(new { error = "No files uploaded" });
            }

            var results = new List<object>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    results.Add(new
                    {
                        status = "error",
                        name = file.FileName,
                        uid = Guid.NewGuid().ToString(),
                        error = "Invalid file type"
                    });
                    continue;
                }
                if (file.Length > 10 * 1024 * 1024)
                {
                    results.Add(new
                    {
                        status = "error",
                        name = file.FileName,
                        uid = Guid.NewGuid().ToString(),
                        error = "File size too large"
                    });
                    continue;
                }
                var fileName = $"product-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var fileUrl = $"/uploads/products/{fileName}";
                results.Add(new
                {
                    status = "done",
                    url = fileUrl,
                    name = fileName,
                    uid = Guid.NewGuid().ToString()
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("upload/video")]
    public IActionResult UploadVideo(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "Invalid file type. Only video files are allowed." });
            }

            // Validate file size (max 100MB for videos)
            if (file.Length > 100 * 1024 * 1024)
            {
                return BadRequest(new { error = "File size too large. Maximum 100MB allowed for videos." });
            }

            // Generate unique filename
            var fileName = $"product-video-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "products", "videos");
            
            // Create directory if not exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Return response in format expected by AntDesign Upload
            var fileUrl = $"/uploads/products/videos/{fileName}";
            
            return Ok(new
            {
                status = "done",
                url = fileUrl,
                name = fileName,
                uid = Guid.NewGuid().ToString()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("upload/multiple-videos")]
    public IActionResult UploadMultipleVideos(IFormFile[] files)
    {
        try
        {
            if (files == null || files.Length == 0)
            {
                return BadRequest(new { error = "No files uploaded" });
            }

            var results = new List<object>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;

                var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    results.Add(new
                    {
                        status = "error",
                        name = file.FileName,
                        uid = Guid.NewGuid().ToString(),
                        error = "Invalid file type"
                    });
                    continue;
                }
                if (file.Length > 100 * 1024 * 1024)
                {
                    results.Add(new
                    {
                        status = "error",
                        name = file.FileName,
                        uid = Guid.NewGuid().ToString(),
                        error = "File size too large"
                    });
                    continue;
                }
                var fileName = $"product-video-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "products", "videos");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var fileUrl = $"/uploads/products/videos/{fileName}";
                results.Add(new
                {
                    status = "done",
                    url = fileUrl,
                    name = fileName,
                    uid = Guid.NewGuid().ToString()
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("upload/delete")]
    public IActionResult DeleteImage([FromQuery] string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new { error = "File name is required" });
            }

            var filePath = Path.Combine(environment.WebRootPath, "uploads", "products", fileName);
            
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok(new { success = true, message = "File deleted successfully" });
            }

            return NotFound(new { error = "File not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // Cleanup old files (can be called by a scheduled job)
    [HttpPost("upload/cleanup")]
    public IActionResult CleanupOldFiles()
    {
        try
        {
            var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "products");
            var deletedCount = 0;

            if (Directory.Exists(uploadPath))
            {
                var files = Directory.GetFiles(uploadPath);
                var cutoffDate = DateTime.Now.AddDays(-7); // Delete files older than 7 days

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                            deletedCount++;
                        }
                        catch
                        {
                            // Log error but continue
                        }
                    }
                }
            }

            return Ok(new { deletedCount, message = $"Deleted {deletedCount} old files" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}