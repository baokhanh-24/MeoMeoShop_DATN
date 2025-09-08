# API Endpoints Test Guide

## Product Search API Endpoints

### 1. Search Products

**Endpoint:** `GET /api/Products/search-products-async`

**Query Parameters:**

-   `SearchKeyword` (string, optional): Từ khóa tìm kiếm
-   `SKUFilter` (string, optional): Lọc theo SKU
-   `NameFilter` (string, optional): Lọc theo tên sản phẩm
-   `BrandId` (Guid, optional): Lọc theo brand
-   `CategoryId` (Guid, optional): Lọc theo category
-   `MinPrice` (decimal, optional): Giá tối thiểu
-   `MaxPrice` (decimal, optional): Giá tối đa
-   `InStockOnly` (bool, optional): Chỉ sản phẩm còn hàng (default: true)
-   `SizeId` (Guid, optional): Lọc theo size
-   `ColourId` (Guid, optional): Lọc theo màu
-   `PageIndex` (int, optional): Trang hiện tại (default: 1)
-   `PageSize` (int, optional): Số lượng mỗi trang (default: 20, max: 100)

**Example Request:**

```
GET /api/Products/search-products-async?SearchKeyword=áo&InStockOnly=true&PageIndex=1&PageSize=10
```

**Response:**

```json
{
    "items": [
        {
            "productDetailId": "guid",
            "productId": "guid",
            "sku": "SKU001",
            "productName": "Áo thun nam",
            "brandName": "Nike",
            "categoryName": "Áo",
            "sizeValue": "M",
            "colourName": "Đen",
            "price": 299000,
            "salePrice": 250000,
            "stockQuantity": 50,
            "outOfStock": false,
            "thumbnail": "/images/product1.jpg",
            "description": "Mô tả sản phẩm",
            "rating": 4.5,
            "reviewCount": 120,
            "saleNumber": 500,
            "createdDate": "2024-01-01T00:00:00Z",
            "lastModifiedDate": "2024-01-15T00:00:00Z",
            "isActive": true,
            "allowReturn": true,
            "maxDiscount": 20,
            "barcode": "1234567890123",
            "weight": "200g",
            "dimensions": "30x40cm",
            "material": "Cotton",
            "season": "Summer",
            "imageUrls": ["/images/product1_1.jpg", "/images/product1_2.jpg"],
            "tags": ["hot", "new"]
        }
    ],
    "totalCount": 1,
    "pageIndex": 1,
    "pageSize": 10
}
```

### 2. Get Product by SKU

**Endpoint:** `GET /api/Products/get-product-by-sku-async/{sku}`

**Example Request:**

```
GET /api/Products/get-product-by-sku-async/SKU001
```

**Response:** Same as single item in search response above

### 3. Get Product by Barcode

**Endpoint:** `GET /api/Products/get-product-by-barcode-async/{barcode}`

**Example Request:**

```
GET /api/Products/get-product-by-barcode-async/1234567890123
```

**Response:** Same as single item in search response above

## Error Responses

**400 Bad Request:**

```json
{
    "responseStatus": "Error",
    "message": "Request không hợp lệ"
}
```

**404 Not Found:**

```json
{
    "responseStatus": "Error",
    "message": "Không tìm thấy sản phẩm với SKU: SKU001"
}
```

**500 Internal Server Error:**

```json
{
    "responseStatus": "Error",
    "message": "Lỗi khi tìm kiếm sản phẩm: [error details]"
}
```

## Testing with Postman/curl

### Search Products

```bash
curl -X GET "https://localhost:7001/api/Products/search-products-async?SearchKeyword=áo&InStockOnly=true&PageIndex=1&PageSize=10" \
  -H "accept: application/json"
```

### Get by SKU

```bash
curl -X GET "https://localhost:7001/api/Products/get-product-by-sku-async/SKU001" \
  -H "accept: application/json"
```

### Get by Barcode

```bash
curl -X GET "https://localhost:7001/api/Products/get-product-by-barcode-async/1234567890123" \
  -H "accept: application/json"
```
