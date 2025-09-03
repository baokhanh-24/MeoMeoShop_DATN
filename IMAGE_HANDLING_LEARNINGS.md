# Image Handling Learnings from ProductDetailsController

## Overview

Đã học hỏi cách ProductDetailsController xử lý ảnh và áp dụng vào ProductService để cải thiện việc quản lý media uploads.

## Key Learnings from ProductDetailsController

### 1. FileUploadHelper Usage

```csharp
// ProductDetailsController sử dụng FileUploadHelper
var lstMediaToUpload = productDetail.MediaUploads?.Select(c => c.UploadFile).ToList() ?? new List<IFormFile>();
var listFileUploaded = await FileUploadHelper.UploadFilesAsync(_env, lstMediaToUpload, "Products", productId);
```

**Ưu điểm:**

-   Tự động tạo thư mục theo cấu trúc: `/wwwroot/Products/{productId}/`
-   Tạo tên file an toàn với GUID
-   Hỗ trợ validation file size và extension
-   Trả về `FileUploadResult` với thông tin chi tiết

### 2. Rollback Strategy

```csharp
// Nếu tạo sản phẩm thất bại, xóa files đã upload
if (result.ResponseStatus == BaseStatus.Error)
{
    FileUploadHelper.DeleteUploadedFiles(_env, listFileUploaded);
}
```

**Ưu điểm:**

-   Đảm bảo không có orphaned files
-   Cleanup tự động khi có lỗi

### 3. Update Image Handling

```csharp
// Lọc ra các ảnh mới (Id == null)
var newImages = productDetail.MediaUploads?.Where(img => img.Id == null).Select(img => img.UploadFile).ToList();

// Lấy danh sách ảnh cũ
var oldImages = await _productdetailservices.GetOldImagesAsync(id);
var newImageIds = productDetail.MediaUploads?.Where(i => i.Id != null).Select(i => i.Id.Value).ToList() ?? new List<Guid>();
var imagesToDelete = oldImages.Where(img => !newImageIds.Contains(img.Id)).ToList();

// Xóa file vật lý các ảnh không còn
foreach (var img in imagesToDelete)
{
    FileUploadHelper.DeleteUploadedFiles(_env, new List<FileUploadResult> { new FileUploadResult { RelativePath = img.URL } });
}
```

**Ưu điểm:**

-   Chỉ upload files mới
-   Xóa files cũ không còn sử dụng
-   Quản lý memory hiệu quả

## Applied Changes to ProductService

### 1. Updated CreateProductAsync

```csharp
// Sử dụng FileUploadHelper thay vì tự xử lý
var filesToUpload = productDto.MediaUploads
    .Where(m => m.UploadFile != null)
    .Select(m => m.UploadFile)
    .ToList();

if (filesToUpload.Any())
{
    uploadedFiles = await MeoMeo.API.Extensions.FileUploadHelper.UploadFilesAsync(
        _environment,
        filesToUpload,
        "Products",
        createdProduct.Id
    );
}

// Save uploaded files to database
foreach (var uploadedFile in uploadedFiles)
{
    var image = new Image
    {
        Id = Guid.NewGuid(),
        ProductId = createdProduct.Id,
        URL = "/" + uploadedFile.RelativePath,
        FileName = uploadedFile.FileName,
        ContentType = GetContentTypeFromExtension(Path.GetExtension(uploadedFile.FileName)),
        Type = uploadedFile.FileType ?? 0,
        CreationTime = DateTime.Now
    };
    await _imageRepository.AddAsync(image);
}
```

### 2. Updated HandleMediaUploads Method

```csharp
private async Task HandleMediaUploads(Guid productId, CreateOrUpdateProductDTO productDto)
{
    // Get existing images
    var existingImages = await _imageRepository.Query()
        .Where(i => i.ProductId == productId)
        .ToListAsync();

    // Handle new file uploads
    var newFiles = productDto.MediaUploads
        .Where(m => m.UploadFile != null)
        .Select(m => m.UploadFile)
        .ToList();

    List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();
    if (newFiles.Any())
    {
        uploadedFiles = await MeoMeo.API.Extensions.FileUploadHelper.UploadFilesAsync(
            _environment,
            newFiles,
            "Products",
            productId
        );
    }

    // Handle image deletion
    var keepImageIds = productDto.MediaUploads
        .Where(m => m.Id.HasValue)
        .Select(m => m.Id.Value)
        .ToList();

    var imagesToDelete = existingImages
        .Where(img => !keepImageIds.Contains(img.Id))
        .ToList();

    foreach (var image in imagesToDelete)
    {
        await _imageRepository.DeleteAsync(image.Id);

        // Delete physical file if it's a local file
        if (image.URL.StartsWith("/Products/"))
        {
            var fileToDelete = new FileUploadResult
            {
                RelativePath = image.URL.TrimStart('/')
            };
            MeoMeo.API.Extensions.FileUploadHelper.DeleteUploadedFiles(_environment, new List<FileUploadResult> { fileToDelete });
        }
    }
}
```

### 3. Updated DeleteAsync Method

```csharp
// Delete images and files
var images = await _imageRepository.Query()
    .Where(i => i.ProductId == id)
    .ToListAsync();

var filesToDelete = images.Select(i => new FileUploadResult
{
    RelativePath = i.URL.TrimStart('/')
}).ToList();

foreach (var image in images)
{
    await _imageRepository.DeleteAsync(image.Id);
}

// Delete main product
await _repository.DeleteProductAsync(id);

await _unitOfWork.CommitTransactionAsync();

// Delete physical files after successful database deletion
MeoMeo.API.Extensions.FileUploadHelper.DeleteUploadedFiles(_environment, filesToDelete);
```

## FileUploadHelper Features

### 1. UploadFilesAsync Method

```csharp
public static async Task<List<FileUploadResult>> UploadFilesAsync(
    IWebHostEnvironment env,
    List<IFormFile> files,
    string folderName,
    Guid objectId,
    List<string>? acceptedExtensions = null,
    bool createSubFolderPerObject = true,
    long maxFileSizeInBytes = 20 * 1024 * 1024 // default 20 MB
)
```

**Parameters:**

-   `env`: WebHostEnvironment để lấy đường dẫn wwwroot
-   `files`: Danh sách files cần upload
-   `folderName`: Tên thư mục chính (VD: "Products")
-   `objectId`: ID của object (VD: productId)
-   `acceptedExtensions`: Các extension được phép
-   `createSubFolderPerObject`: Tạo subfolder cho mỗi object
-   `maxFileSizeInBytes`: Kích thước file tối đa

### 2. FileUploadResult Structure

```csharp
public class FileUploadResult
{
    public string FileName { get; set; }
    public string RelativePath { get; set; }
    public string FullPath { get; set; }
    public int? FileType { get; set; }
    public long Size { get; set; }
}
```

### 3. DeleteUploadedFiles Method

```csharp
public static void DeleteUploadedFiles(IWebHostEnvironment env, List<FileUploadResult> uploadedFiles)
{
    var webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    foreach (var file in uploadedFiles)
    {
        var absolutePath = Path.Combine(webRootPath, file.RelativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (File.Exists(absolutePath))
        {
            try
            {
                File.Delete(absolutePath);
            }
            catch
            {
                // Log error if needed
            }
        }
    }
}
```

## Best Practices Learned

### 1. File Organization

-   Tạo thư mục riêng cho mỗi product: `/Products/{productId}/`
-   Sử dụng GUID trong tên file để tránh conflict
-   Lưu relative path trong database

### 2. Error Handling

-   Rollback files khi database operation thất bại
-   Sử dụng try-catch khi xóa files
-   Validate file size và extension trước khi upload

### 3. Memory Management

-   Chỉ upload files mới khi update
-   Xóa files cũ không còn sử dụng
-   Sử dụng streaming cho file operations

### 4. Database Consistency

-   Sử dụng transaction để đảm bảo consistency
-   Xóa database records trước khi xóa physical files
-   Lưu đầy đủ metadata (fileName, contentType, fileType)

## File Structure

```
wwwroot/
├── Products/
│   ├── {productId1}/
│   │   ├── {guid1}_image1.jpg
│   │   ├── {guid2}_image2.png
│   │   └── {guid3}_image3.webp
│   └── {productId2}/
│       ├── {guid4}_image1.jpg
│       └── {guid5}_image2.png
```

## Benefits of This Approach

1. **Consistency**: Sử dụng cùng pattern với ProductDetailsController
2. **Reliability**: Rollback mechanism đảm bảo không có orphaned files
3. **Performance**: Chỉ upload files mới, xóa files cũ không cần thiết
4. **Maintainability**: Sử dụng FileUploadHelper có sẵn, không duplicate code
5. **Security**: Validation file size và extension
6. **Organization**: File structure rõ ràng, dễ quản lý

## Next Steps

1. **Testing**: Test tất cả scenarios với file uploads
2. **Performance**: Monitor memory usage với large files
3. **Security**: Add virus scanning if needed
4. **CDN**: Consider using CDN for production
5. **Compression**: Add image compression for optimization
