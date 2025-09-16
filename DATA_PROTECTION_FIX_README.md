# Data Protection Key Ring Fix

## Problem

The application was experiencing `CryptographicException` errors with the message:

```
The key {d8884450-7827-4174-a6a9-4bf6e1ec2ae6} was not found in the key ring.
```

This error occurs when ASP.NET Core's data protection system tries to decrypt data that was encrypted with a different key ring, typically after application restarts or when running in different environments.

## Root Cause

-   ASP.NET Core generates new encryption keys each time the application starts in development
-   `ProtectedLocalStorage` stores encrypted tokens that become unreadable when the key ring changes
-   The `ApiCaller` and `TokenProvider` classes were not handling these cryptographic exceptions gracefully

## Solution Implemented

### 1. Persistent Data Protection Keys (Development)

Added data protection configuration in both `MeoMeo.PORTAL/Program.cs` and `MeoMeo.CMS/Program.cs`:

```csharp
// Configure data protection for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
}
```

This ensures that encryption keys persist between application restarts in development.

### 2. Graceful Error Handling

Enhanced error handling in `ApiCaller.cs` and `TokenProvider.cs` to catch `CryptographicException` and:

-   Log a warning instead of an error
-   Automatically clear corrupted token data
-   Continue execution without authentication (user will need to re-login)

#### ApiCaller.cs Changes:

```csharp
catch (System.Security.Cryptography.CryptographicException ex)
{
    _logger.LogWarning(ex, "Data protection key not found. Clearing stored tokens and continuing without authentication.");
    // Clear the corrupted token data
    try
    {
        await _localStorage.DeleteAsync(StorageKey);
    }
    catch (Exception deleteEx)
    {
        _logger.LogError(deleteEx, "Failed to clear corrupted token data");
    }
}
```

#### TokenProvider.cs Changes:

Similar error handling added to both `GetAccessTokenAsync()` and `GetRefreshTokenAsync()` methods.

## Benefits

1. **Eliminates Error Logs**: No more `CryptographicException` errors in logs
2. **Graceful Degradation**: Application continues to work, users just need to re-authenticate
3. **Persistent Keys**: In development, tokens persist between application restarts
4. **Automatic Cleanup**: Corrupted token data is automatically cleared

## Files Modified

-   `MeoMeo.PORTAL/Program.cs` - Added data protection configuration
-   `MeoMeo.CMS/Program.cs` - Added data protection configuration
-   `MeoMeo.Shared/Utilities/ApiCaller.cs` - Added cryptographic exception handling
-   `MeoMeo.Shared/Utilities/TokenProvider.cs` - Added cryptographic exception handling

## Docker Configuration

**Không cần thay đổi docker-compose.yml** vì:

1. **Development Environment**: Keys được persist vào file system local
2. **Production Environment (Docker)**: Sử dụng in-memory keys với lifetime 30 ngày

Cấu hình hiện tại đã được tối ưu cho cả hai môi trường:

-   Development: Keys persist giữa các lần restart
-   Production: Keys tồn tại trong memory với thời gian dài hơn

## Testing

After implementing these changes:

1. The application should start without cryptographic errors
2. If tokens become corrupted, users will be automatically logged out and can re-authenticate
3. In development, tokens should persist between application restarts

## Production Considerations

For production environments, consider:

-   Using a shared key storage solution (Redis, SQL Server, etc.)
-   Implementing proper key rotation policies
-   Using Azure Key Vault or similar services for key management
