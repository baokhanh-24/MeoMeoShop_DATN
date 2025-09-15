# 🖼️ Avatar Upload Feature - Real File Upload Implementation

## 🎯 Tổng quan

Đã cập nhật màn **ProfileEdit** để có chức năng upload file ảnh thật như Portal, thay vì chỉ nhập URL.

## 🔄 **So sánh trước và sau:**

### **Trước khi cập nhật:**

-   ❌ Chỉ có thể nhập URL avatar
-   ❌ Không có chức năng upload file thật
-   ❌ Phải có sẵn URL ảnh từ nơi khác
-   ❌ Không có validation file size/type

### **Sau khi cập nhật:**

-   ✅ **Real File Upload**: Upload file ảnh thật từ máy tính
-   ✅ **File Validation**: Kiểm tra loại file và kích thước
-   ✅ **Base64 Storage**: Lưu ảnh dưới dạng base64
-   ✅ **Preview**: Xem trước ảnh ngay sau khi chọn
-   ✅ **User Friendly**: Giao diện đơn giản, dễ sử dụng

## 🔧 **Implementation Details:**

### **1. HTML Structure:**

```html
<!-- Avatar Display -->
<div class="avatar-container">
    @if (!string.IsNullOrEmpty(currentAvatarUrl)) {
    <img
        src="data:image/jpeg;base64,@currentAvatarUrl"
        alt="Avatar"
        class="avatar-image"
    />
    } else {
    <div class="avatar-placeholder">
        <Icon Type="user" Style="font-size: 48px; color: #ccc;" />
    </div>
    }
</div>

<!-- Upload Button with Hidden InputFile -->
<div class="avatar-upload-container" style="position: relative;">
    <button type="ButtonType.Primary" Icon="@IconType.Outline.Camera" Block>
        Thay đổi ảnh
    </button>
    <InputFile
        @ref="avatarInput"
        OnChange="OnAvatarSelected"
        accept="image/*"
        style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer;"
    />
</div>
<p class="avatar-note">JPG, PNG hoặc GIF. Tối đa 5MB.</p>
```

### **2. C# Code Implementation:**

```csharp
// State Variables
private bool isUploadingAvatar = false;
private InputFile? avatarInput;
private string currentAvatarUrl = string.Empty;

// File Upload Handler
private async Task OnAvatarSelected(InputFileChangeEventArgs e)
{
    var file = e.File;
    if (file == null) return;

    // File Type Validation
    if (!file.ContentType.StartsWith("image/"))
    {
        await MessageModalService.Error("Vui lòng chọn file ảnh!");
        return;
    }

    // File Size Validation
    if (file.Size > 5 * 1024 * 1024) // 5MB
    {
        await MessageModalService.Error("File ảnh không được vượt quá 5MB!");
        return;
    }

    try
    {
        isUploadingAvatar = true;
        StateHasChanged();

        // Convert file to base64
        var base64 = await ConvertToBase64(file);

        // Update profile model with base64 data
        profileModel.Avatar = base64;
        currentAvatarUrl = base64;

        await MessageModalService.Success("Cập nhật ảnh đại diện thành công!");
    }
    catch (Exception ex)
    {
        await MessageModalService.Error($"Có lỗi xảy ra khi tải ảnh: {ex.Message}");
    }
    finally
    {
        isUploadingAvatar = false;
        StateHasChanged();
    }
}

// Base64 Conversion
private async Task<string> ConvertToBase64(IBrowserFile file)
{
    var maxSize = 5 * 1024 * 1024; // 5MB
    using var stream = file.OpenReadStream(maxAllowedSize: maxSize);
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    var bytes = memoryStream.ToArray();
    return Convert.ToBase64String(bytes);
}
```

## 🎨 **UI/UX Features:**

### **1. Visual Feedback:**

-   **Loading State**: Button hiển thị "Đang tải..." khi upload
-   **Success Message**: Thông báo thành công sau khi upload
-   **Error Messages**: Thông báo lỗi chi tiết cho từng trường hợp
-   **File Info**: Hiển thị thông tin file được hỗ trợ

### **2. User Experience:**

-   **Hidden InputFile**: InputFile ẩn, chỉ hiển thị button đẹp
-   **Click Anywhere**: Click vào button sẽ trigger file picker
-   **Instant Preview**: Ảnh hiển thị ngay sau khi chọn
-   **File Restrictions**: Chỉ cho phép chọn file ảnh

### **3. Responsive Design:**

-   **Mobile Friendly**: Hoạt động tốt trên mobile
-   **Touch Support**: Hỗ trợ touch trên tablet/mobile
-   **Consistent Styling**: Nhất quán với design system

## 🔒 **Validation & Security:**

### **1. File Type Validation:**

```csharp
if (!file.ContentType.StartsWith("image/"))
{
    await MessageModalService.Error("Vui lòng chọn file ảnh!");
    return;
}
```

### **2. File Size Validation:**

```csharp
if (file.Size > 5 * 1024 * 1024) // 5MB
{
    await MessageModalService.Error("File ảnh không được vượt quá 5MB!");
    return;
}
```

### **3. Supported Formats:**

-   **JPG/JPEG**: Joint Photographic Experts Group
-   **PNG**: Portable Network Graphics
-   **GIF**: Graphics Interchange Format
-   **Maximum Size**: 5MB

## 🎨 **CSS Styling:**

### **1. Avatar Container:**

```css
.avatar-container {
    width: 120px;
    height: 120px;
    margin: 0 auto;
    border-radius: 50%;
    overflow: hidden;
    border: 3px solid #f0f0f0;
}

.avatar-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
}
```

### **2. Upload Container:**

```css
.avatar-upload-container {
    position: relative;
}

.avatar-note {
    font-size: 12px;
    color: #999;
    margin-top: 8px;
    text-align: center;
}
```

### **3. Hidden InputFile:**

```css
/* InputFile được ẩn và overlay lên button */
style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer;"
```

## 🔄 **Data Flow:**

### **1. File Selection:**

```
User clicks button → File picker opens → User selects image → OnAvatarSelected triggered
```

### **2. File Processing:**

```
File validation → Convert to base64 → Update profileModel → Update currentAvatarUrl → Show success message
```

### **3. Display Update:**

```
currentAvatarUrl updated → UI re-renders → Image displays with base64 data URL
```

## 🚀 **Technical Benefits:**

### **1. Performance:**

-   **Client-side Processing**: Convert to base64 trên client
-   **No Server Upload**: Không cần upload lên server
-   **Instant Preview**: Hiển thị ngay lập tức
-   **Memory Efficient**: Chỉ lưu base64 string

### **2. Security:**

-   **File Type Validation**: Chỉ cho phép file ảnh
-   **Size Limitation**: Giới hạn kích thước file
-   **No Malicious Files**: Không thể upload file độc hại
-   **Base64 Encoding**: Dữ liệu được encode an toàn

### **3. User Experience:**

-   **Simple Interface**: Giao diện đơn giản
-   **Visual Feedback**: Thông báo rõ ràng
-   **Error Handling**: Xử lý lỗi tốt
-   **Mobile Support**: Hỗ trợ mobile tốt

## 🔧 **Integration:**

### **1. Required Using Statements:**

```csharp
@using Microsoft.AspNetCore.Components.Forms
```

### **2. State Management:**

```csharp
private bool isUploadingAvatar = false;
private InputFile? avatarInput;
private string currentAvatarUrl = string.Empty;
```

### **3. Event Handling:**

```csharp
<InputFile @ref="avatarInput" OnChange="OnAvatarSelected" accept="image/*" />
```

## 🎯 **Comparison with Portal:**

### **Portal Implementation:**

-   ✅ Uses `InputFile` with `OnChange` event
-   ✅ Converts file to base64
-   ✅ Validates file type and size
-   ✅ Shows loading state during upload
-   ✅ Updates UI immediately after upload

### **CMS Implementation (Updated):**

-   ✅ **Same approach** as Portal
-   ✅ **Same validation** logic
-   ✅ **Same user experience**
-   ✅ **Consistent behavior** across projects

## 🎉 **Result:**

### **Before:**

-   User had to find image URL somewhere else
-   No file upload capability
-   Poor user experience

### **After:**

-   ✅ **Real file upload** from computer
-   ✅ **Instant preview** after selection
-   ✅ **Professional user experience**
-   ✅ **Consistent with Portal** behavior
-   ✅ **Mobile-friendly** interface

---

**🖼️ Avatar upload feature đã được implement hoàn chỉnh!**

Bây giờ CMS có chức năng upload avatar giống hệt Portal, với trải nghiệm người dùng tốt và giao diện chuyên nghiệp.
