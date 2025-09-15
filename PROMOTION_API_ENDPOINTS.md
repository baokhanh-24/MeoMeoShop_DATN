# API Endpoints cho Promotion và Sản phẩm

## Base URL

```
http://localhost:5092/api
```

## 1. PROMOTION APIs

### 1.1 Promotion Management

| Method | Endpoint                                      | Description                           | Request Body                        | Response                                  |
| ------ | --------------------------------------------- | ------------------------------------- | ----------------------------------- | ----------------------------------------- |
| GET    | `/Promotions/get-all-promotion-async`         | Lấy danh sách promotion có phân trang | Query: `GetListPromotionRequestDTO` | `PagedResult<CreateOrUpdatePromotionDTO>` |
| GET    | `/Promotions/find-promotion-by-id-async/{id}` | Lấy promotion theo ID                 | -                                   | `CreateOrUpdatePromotionResponseDTO`      |
| POST   | `/Promotions/create-promotion-async`          | Tạo promotion mới                     | `CreateOrUpdatePromotionDTO`        | `CreateOrUpdatePromotionResponseDTO`      |
| PUT    | `/Promotions/update-promotion-async/{id}`     | Cập nhật promotion                    | `CreateOrUpdatePromotionDTO`        | `CreateOrUpdatePromotionResponseDTO`      |
| DELETE | `/Promotions/delete-promotion-async/{id}`     | Xóa promotion                         | -                                   | `bool`                                    |

### 1.2 Promotion Detail Management

| Method | Endpoint                                                   | Description                                  | Request Body                              | Response                                        |
| ------ | ---------------------------------------------------------- | -------------------------------------------- | ----------------------------------------- | ----------------------------------------------- |
| GET    | `/PromotionDetails/get-all-promotion-detail-async`         | Lấy danh sách promotion detail có phân trang | Query: `GetListPromotionDetailRequestDTO` | `PagedResult<CreateOrUpdatePromotionDetailDTO>` |
| GET    | `/PromotionDetails/find-promotion-detail-by-id-async/{id}` | Lấy promotion detail theo ID                 | -                                         | `CreateOrUpdatePromotionDetailResponseDTO`      |
| POST   | `/PromotionDetails/create-promotion-detail-async`          | Tạo promotion detail mới                     | `CreateOrUpdatePromotionDetailDTO`        | `CreateOrUpdatePromotionDetailResponseDTO`      |
| PUT    | `/PromotionDetails/update-promotion-detail-async/{id}`     | Cập nhật promotion detail                    | `CreateOrUpdatePromotionDetailDTO`        | `CreateOrUpdatePromotionDetailResponseDTO`      |
| DELETE | `/PromotionDetails/delete-promotion-detail-async/{id}`     | Xóa promotion detail                         | -                                         | `bool`                                          |

## 2. PRODUCT APIs

### 2.1 Product Management

| Method | Endpoint                                       | Description                  | Request Body                       | Response                           |
| ------ | ---------------------------------------------- | ---------------------------- | ---------------------------------- | ---------------------------------- |
| GET    | `/Products/get-all-product-async`              | Lấy tất cả sản phẩm          | -                                  | `List<ProductResponseDTO>`         |
| GET    | `/Products/get-paged-products-async`           | Lấy sản phẩm có phân trang   | Query: `GetListProductRequestDTO`  | `PagedResult<ProductResponseDTO>`  |
| GET    | `/Products/get-header-products`                | Lấy sản phẩm cho header      | -                                  | `List<ProductResponseDTO>`         |
| GET    | `/Products/get-product-for-create-update/{id}` | Lấy sản phẩm để tạo/cập nhật | -                                  | `CreateOrUpdateProductDTO`         |
| GET    | `/Products/get-product-by-id-async/{id}`       | Lấy sản phẩm theo ID         | -                                  | `ProductResponseDTO`               |
| GET    | `/Products/get-product-by-sku-async/{sku}`     | Lấy sản phẩm theo SKU        | -                                  | `ProductSearchResponseDTO`         |
| POST   | `/Products/create-product-async`               | Tạo sản phẩm mới             | `CreateOrUpdateProductDTO` + Files | `CreateOrUpdateProductResponseDTO` |
| PUT    | `/Products/update-product-async/{id}`          | Cập nhật sản phẩm            | `CreateOrUpdateProductDTO` + Files | `CreateOrUpdateProductResponseDTO` |
| DELETE | `/Products/delete-product-async/{id}`          | Xóa sản phẩm                 | -                                  | `CreateOrUpdateProductResponseDTO` |

### 2.2 Product Search & Filter

| Method | Endpoint                                | Description                    | Request Body                     | Response                                |
| ------ | --------------------------------------- | ------------------------------ | -------------------------------- | --------------------------------------- |
| GET    | `/Products/search-products-async`       | Tìm kiếm sản phẩm              | Query: `ProductSearchRequestDTO` | `PagedResult<ProductSearchResponseDTO>` |
| GET    | `/Products/best-sellers-week`           | Sản phẩm bán chạy tuần         | Query: `take=10`                 | `List<ProductResponseDTO>`              |
| GET    | `/Products/top-rated`                   | Sản phẩm đánh giá cao          | Query: `take=12`                 | `List<ProductResponseDTO>`              |
| POST   | `/Products/by-ids`                      | Lấy sản phẩm theo danh sách ID | `List<Guid>`                     | `List<ProductResponseDTO>`              |
| PUT    | `/Products/update-variant-status-async` | Cập nhật trạng thái biến thể   | `UpdateProductStatusDTO`         | `BaseResponse`                          |

### 2.3 Legacy Product APIs (Backward Compatibility)

| Method | Endpoint                                 | Description                  | Request Body               | Response                           |
| ------ | ---------------------------------------- | ---------------------------- | -------------------------- | ---------------------------------- |
| GET    | `/Products/get-all-product-async-legacy` | Lấy tất cả sản phẩm (legacy) | -                          | `List<ProductResponseDTO>`         |
| POST   | `/Products/create-product-legacy`        | Tạo sản phẩm (legacy)        | `CreateOrUpdateProductDTO` | `CreateOrUpdateProductResponseDTO` |
| PUT    | `/Products/update-product-legacy/{id}`   | Cập nhật sản phẩm (legacy)   | `CreateOrUpdateProductDTO` | `CreateOrUpdateProductResponseDTO` |
| DELETE | `/Products/delete-product-legacy/{id}`   | Xóa sản phẩm (legacy)        | -                          | `CreateOrUpdateProductResponseDTO` |

## 3. RELATED APIs

### 3.1 Category APIs

| Method | Endpoint                                   | Description            | Request Body                       | Response                   |
| ------ | ------------------------------------------ | ---------------------- | ---------------------------------- | -------------------------- |
| GET    | `/Category/get-all-category-async`         | Lấy danh sách category | Query: `GetListCategoryRequestDTO` | `PagedResult<CategoryDTO>` |
| GET    | `/Category/find-category-by-id-async/{id}` | Lấy category theo ID   | -                                  | `CategoryResponseDTO`      |
| POST   | `/Category/create-category-async`          | Tạo category mới       | `CreateOrUpdateCategoryDTO`        | `CategoryResponseDTO`      |
| PUT    | `/Category/update-category-async/{id}`     | Cập nhật category      | `CreateOrUpdateCategoryDTO`        | `CategoryResponseDTO`      |
| DELETE | `/Category/delete-category-async/{id}`     | Xóa category           | -                                  | `bool`                     |

### 3.2 Brand APIs

| Method | Endpoint                              | Description         | Request Body                    | Response                |
| ------ | ------------------------------------- | ------------------- | ------------------------------- | ----------------------- |
| GET    | `/Brands/get-all-brand-async`         | Lấy danh sách brand | Query: `GetListBrandRequestDTO` | `PagedResult<BrandDTO>` |
| GET    | `/Brands/find-brand-by-id-async/{id}` | Lấy brand theo ID   | -                               | `BrandResponseDTO`      |
| POST   | `/Brands/create-brand-async`          | Tạo brand mới       | `CreateOrUpdateBrandDTO`        | `BrandResponseDTO`      |
| PUT    | `/Brands/update-brand-async/{id}`     | Cập nhật brand      | `CreateOrUpdateBrandDTO`        | `BrandResponseDTO`      |
| DELETE | `/Brands/delete-brand-async/{id}`     | Xóa brand           | -                               | `bool`                  |

### 3.3 Size APIs

| Method | Endpoint                           | Description        | Request Body                   | Response               |
| ------ | ---------------------------------- | ------------------ | ------------------------------ | ---------------------- |
| GET    | `/Size/get-all-size-async`         | Lấy danh sách size | Query: `GetListSizeRequestDTO` | `PagedResult<SizeDTO>` |
| GET    | `/Size/find-size-by-id-async/{id}` | Lấy size theo ID   | -                              | `SizeResponseDTO`      |
| POST   | `/Size/create-size-async`          | Tạo size mới       | `CreateOrUpdateSizeDTO`        | `SizeResponseDTO`      |
| PUT    | `/Size/update-size-async/{id}`     | Cập nhật size      | `CreateOrUpdateSizeDTO`        | `SizeResponseDTO`      |
| DELETE | `/Size/delete-size-async/{id}`     | Xóa size           | -                              | `bool`                 |

### 3.4 Material APIs

| Method | Endpoint                                    | Description            | Request Body                       | Response                   |
| ------ | ------------------------------------------- | ---------------------- | ---------------------------------- | -------------------------- |
| GET    | `/Materials/get-all-material-async`         | Lấy danh sách material | Query: `GetListMaterialRequestDTO` | `PagedResult<MaterialDTO>` |
| GET    | `/Materials/find-material-by-id-async/{id}` | Lấy material theo ID   | -                                  | `MaterialResponseDTO`      |
| POST   | `/Materials/create-material-async`          | Tạo material mới       | `CreateOrUpdateMaterialDTO`        | `MaterialResponseDTO`      |
| PUT    | `/Materials/update-material-async/{id}`     | Cập nhật material      | `CreateOrUpdateMaterialDTO`        | `MaterialResponseDTO`      |
| DELETE | `/Materials/delete-material-async/{id}`     | Xóa material           | -                                  | `bool`                     |

### 3.5 Colour APIs

| Method | Endpoint                               | Description          | Request Body                     | Response                 |
| ------ | -------------------------------------- | -------------------- | -------------------------------- | ------------------------ |
| GET    | `/Colour/get-all-colour-async`         | Lấy danh sách colour | Query: `GetListColourRequestDTO` | `PagedResult<ColourDTO>` |
| GET    | `/Colour/find-colour-by-id-async/{id}` | Lấy colour theo ID   | -                                | `ColourResponseDTO`      |
| POST   | `/Colour/create-colour-async`          | Tạo colour mới       | `CreateOrUpdateColourDTO`        | `ColourResponseDTO`      |
| PUT    | `/Colour/update-colour-async/{id}`     | Cập nhật colour      | `CreateOrUpdateColourDTO`        | `ColourResponseDTO`      |
| DELETE | `/Colour/delete-colour-async/{id}`     | Xóa colour           | -                                | `bool`                   |

### 3.6 Season APIs

| Method | Endpoint                                | Description          | Request Body                     | Response                 |
| ------ | --------------------------------------- | -------------------- | -------------------------------- | ------------------------ |
| GET    | `/Seasons/get-all-season-async`         | Lấy danh sách season | Query: `GetListSeasonRequestDTO` | `PagedResult<SeasonDTO>` |
| GET    | `/Seasons/find-season-by-id-async/{id}` | Lấy season theo ID   | -                                | `SeasonResponseDTO`      |
| POST   | `/Seasons/create-season-async`          | Tạo season mới       | `CreateOrUpdateSeasonDTO`        | `SeasonResponseDTO`      |
| PUT    | `/Seasons/update-season-async/{id}`     | Cập nhật season      | `CreateOrUpdateSeasonDTO`        | `SeasonResponseDTO`      |
| DELETE | `/Seasons/delete-season-async/{id}`     | Xóa season           | -                                | `bool`                   |

## 4. DTOs Structure

### 4.1 Promotion DTOs

```csharp
// CreateOrUpdatePromotionDTO
{
    "id": "guid",
    "title": "string",
    "startDate": "datetime?",
    "endDate": "datetime?",
    "description": "string",
    "status": "EPromotionStatus",
    "creationTime": "datetime",
    "lastModificationTime": "datetime?",
    "createdBy": "guid",
    "updatedBy": "guid?"
}

// GetListPromotionRequestDTO
{
    "titleFilter": "string?",
    "startDateFilter": "DateOnly?",
    "endDateFilter": "DateOnly?",
    "descriptionFilter": "string?",
    "statusFilter": "EPromotionStatus?",
    "pageIndex": "int",
    "pageSize": "int"
}
```

### 4.2 PromotionDetail DTOs

```csharp
// CreateOrUpdatePromotionDetailDTO
{
    "promotionId": "guid",
    "productDetailId": "guid",
    "id": "guid",
    "discount": "float",
    "note": "string",
    "creationTime": "datetime",
    "lastModificationTime": "datetime"
}
```

### 4.3 Product DTOs

```csharp
// CreateOrUpdateProductDTO
{
    "id": "guid",
    "name": "string",
    "description": "string",
    "price": "decimal",
    "categoryId": "guid",
    "brandId": "guid",
    "seasonId": "guid",
    "status": "EProductStatus",
    "images": "List<ImageDTO>",
    "productDetails": "List<CreateOrUpdateProductDetailDTO>"
}

// ProductSearchRequestDTO
{
    "keyword": "string?",
    "categoryId": "guid?",
    "brandId": "guid?",
    "minPrice": "decimal?",
    "maxPrice": "decimal?",
    "status": "EProductStatus?",
    "pageIndex": "int",
    "pageSize": "int"
}
```

## 5. Enums

### 5.1 Promotion Status

```csharp
public enum EPromotionStatus
{
    Draft,           // Lưu tạm
    NotHappenedYet,  // Chưa diễn ra
    IsGoingOn,       // Đang diễn ra
    Ended            // Đã kết thúc
}
```

### 5.2 Product Status

```csharp
public enum EProductStatus
{
    Active,    // Hoạt động
    Inactive, // Không hoạt động
    Draft      // Lưu tạm
}
```

## 6. Usage Examples

### 6.1 Tạo Promotion mới

```http
POST /api/Promotions/create-promotion-async
Content-Type: application/json

{
    "title": "Khuyến mãi mùa hè",
    "startDate": "2024-06-01T00:00:00",
    "endDate": "2024-08-31T23:59:59",
    "description": "Khuyến mãi đặc biệt cho mùa hè",
    "status": 0
}
```

### 6.2 Tạo PromotionDetail

```http
POST /api/PromotionDetails/create-promotion-detail-async
Content-Type: application/json

{
    "promotionId": "guid-promotion-id",
    "productDetailId": "guid-product-detail-id",
    "discount": 20.0,
    "note": "Giảm giá 20% cho sản phẩm này"
}
```

### 6.3 Tìm kiếm sản phẩm

```http
GET /api/Products/search-products-async?keyword=áo&categoryId=guid&pageIndex=1&pageSize=10
```

### 6.4 Lấy danh sách Promotion có phân trang

```http
GET /api/Promotions/get-all-promotion-async?pageIndex=1&pageSize=10&statusFilter=1
```

## 7. Notes

-   Tất cả API đều trả về response theo format chuẩn với `BaseResponse`
-   Các API có phân trang đều sử dụng `PagingExtensions.PagedResult<T, TMetadata>`
-   File upload cho sản phẩm sử dụng multipart/form-data
-   Tất cả ID đều là GUID
-   Status codes: 200 (Success), 400 (Bad Request), 404 (Not Found), 500 (Internal Server Error)
