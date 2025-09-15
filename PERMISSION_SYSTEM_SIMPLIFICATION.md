# Đơn giản hóa hệ thống phân quyền

## Tổng quan thay đổi

Hệ thống phân quyền đã được đơn giản hóa theo yêu cầu:

### ✅ **Những gì đã thay đổi:**

1. **Permission và PermissionGroup** - Được fix cứng trong database, không có CRUD operations
2. **Chỉ quản lý Role** - Xem role có những permission nào (hiển thị dạng cây)
3. **Chỉ quản lý UserRole** - Gán user vào role

### 🔧 **Thay đổi Backend:**

#### Controllers:

-   **PermissionGroupController**: Loại bỏ POST, PUT, DELETE endpoints
-   **PermissionController**: Loại bỏ POST, PUT, DELETE endpoints
-   **RoleController**: Thêm endpoint `GET /api/role/{id}/permissions-tree`

#### Services:

-   **PermissionGroupService**: Loại bỏ CreatePermissionGroupAsync, UpdatePermissionGroupAsync, DeletePermissionGroupAsync
-   **PermissionService**: Loại bỏ CreatePermissionAsync, UpdatePermissionAsync, DeletePermissionAsync
-   **RoleService**: Thêm GetRolePermissionsTreeAsync method

#### Interfaces:

-   **IPermissionGroupService**: Loại bỏ CRUD method signatures
-   **IPermissionService**: Loại bỏ CRUD method signatures
-   **IRoleService**: Thêm GetRolePermissionsTreeAsync signature

### 🎨 **Thay đổi Frontend:**

#### Pages:

-   **PermissionGroups.razor**: Vô hiệu hóa, hiển thị thông báo chức năng đã bị loại bỏ
-   **Roles.razor**: Cập nhật để hiển thị permissions dạng cây với API mới
-   **UserRoles.razor**: Đơn giản hóa giao diện gán role cho user

#### Services:

-   **RoleClientService**: Thêm GetRolePermissionsTreeAsync method
-   **IRoleClientService**: Thêm GetRolePermissionsTreeAsync signature

### 🌳 **Cấu trúc cây permissions:**

Permissions được hiển thị theo cấu trúc cây dựa trên PermissionGroup:

```
📁 PermissionGroup 1
├── 📄 Permission 1.1
├── 📄 Permission 1.2
└── 📁 PermissionGroup 1.1 (Child)
    ├── 📄 Permission 1.1.1
    └── 📄 Permission 1.1.2

📁 PermissionGroup 2
├── 📄 Permission 2.1
└── 📄 Permission 2.2
```

### 🔄 **Luồng hoạt động mới:**

1. **Quản lý Role**:

    - Xem danh sách roles
    - Tạo/sửa/xóa role
    - Gán permissions cho role (hiển thị dạng cây)
    - Xem permissions của role

2. **Quản lý User-Role**:
    - Xem danh sách users và roles của họ
    - Gán role cho user
    - Xóa role khỏi user

### 📋 **API Endpoints còn lại:**

#### PermissionGroup:

-   `GET /api/permissiongroup` - Lấy tất cả permission groups
-   `GET /api/permissiongroup/tree` - Lấy permission groups dạng cây
-   `GET /api/permissiongroup/{id}` - Lấy permission group theo ID

#### Permission:

-   `GET /api/permission` - Lấy tất cả permissions
-   `GET /api/permission/{id}` - Lấy permission theo ID
-   `GET /api/permission/group/{groupId}` - Lấy permissions theo group
-   `GET /api/permission/role/{roleId}` - Lấy permissions theo role

#### Role:

-   `GET /api/role` - Lấy tất cả roles
-   `GET /api/role/{id}` - Lấy role theo ID
-   `POST /api/role` - Tạo role mới
-   `PUT /api/role` - Cập nhật role
-   `DELETE /api/role/{id}` - Xóa role
-   `POST /api/role/assign-permissions` - Gán permissions cho role
-   `GET /api/role/{id}/permissions-tree` - Lấy permissions của role dạng cây
-   `GET /api/role/{id}/permissions` - Lấy permissions của role (flat)
-   `GET /api/role/{id}/users` - Lấy users của role

#### UserRole:

-   `GET /api/userrole/users` - Lấy tất cả users với roles
-   `GET /api/userrole/users/{userId}` - Lấy user với roles
-   `POST /api/userrole/assign` - Gán role cho user
-   `DELETE /api/userrole/users/{userId}/roles/{roleId}` - Xóa role khỏi user
-   `GET /api/userrole/users/{userId}/roles` - Lấy roles của user
-   `GET /api/userrole/roles/{roleId}/users` - Lấy users của role

### 🎯 **Lợi ích:**

1. **Đơn giản hóa**: Loại bỏ các chức năng không cần thiết
2. **Bảo mật**: Permission và PermissionGroup được fix cứng, không thể thay đổi qua UI
3. **Dễ quản lý**: Chỉ cần quản lý Role và User-Role assignment
4. **Hiển thị trực quan**: Permissions được hiển thị dạng cây dễ hiểu
5. **Hiệu suất**: Giảm số lượng API calls và operations

### 📝 **Ghi chú:**

-   Permission và PermissionGroup vẫn tồn tại trong database nhưng chỉ có thể được quản lý trực tiếp qua database
-   Tất cả các thay đổi đều backward compatible với hệ thống hiện tại
-   Frontend đã được cập nhật để sử dụng API mới
-   CSS đã được cải thiện để hiển thị cây permissions đẹp hơn
