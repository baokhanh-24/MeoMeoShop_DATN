using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartDetaillRepository _cartDetaillRepository;
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IIventoryBatchReposiory _inventoryBatchRepository;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IMapper _mapper;
        public CartService(
            ICartRepository cartRepository,
            IMapper mapper,
            ICartDetaillRepository cartDetaillRepository,
            IProductsDetailRepository productDetailRepository,
            IIventoryBatchReposiory inventoryBatchRepository,
            IPromotionDetailRepository promotionDetailRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _cartDetaillRepository = cartDetaillRepository;
            _productDetailRepository = productDetailRepository;
            _inventoryBatchRepository = inventoryBatchRepository;
            _promotionDetailRepository = promotionDetailRepository;
        }
        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            var cart = await _cartRepository.GetAllCart();
            return cart;
        }

        public async Task<CartResponseDTO> GetCartByUserIdAsync(Guid userId)
        {
            var cartProjection = await _cartRepository
                .Query()
                .Where(c => c.CustomerId == userId)
                .Select(c => new { c.Id, c.CustomerId, c.CreationTime, c.LastModificationTime, c.TotalPrice })
                .FirstOrDefaultAsync();
            if (cartProjection == null)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy giỏ hàng" };
            }

            return new CartResponseDTO
            {
                Id = cartProjection.Id,
                CustomersId = cartProjection.CustomerId,
                TotalPrice = cartProjection.TotalPrice,
                ResponseStatus = BaseStatus.Success
            };
        }

        public async Task<CartResponseDTO> CreateCartAsync(CartDTO cartDto)
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                CustomerId = cartDto.CustomersId,
                CreatedBy = cartDto.createBy,
                CreationTime = cartDto.NgayTao,
                LastModificationTime = cartDto.lastModificationTime,
                TotalPrice = cartDto.TongTien
            };
           await _cartRepository.Create(cart);
            return new CartResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm giỏ hàng thành công"
            };
        }

        public async Task<CartResponseDTO> UpdateCartAsync(CartDTO cartDto)
        {
            var cart = await _cartRepository.GetCartById(cartDto.Id);
            if (cart == null)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(cartDto, cart);
            await _cartRepository.Update(cart);
            return new CartResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }

        public async Task SaveChangesAsync()
        {
            await _cartRepository.Savechanges();
        }

        // Thêm sản phẩm vào giỏ hàng theo AddToCartDTO
        public async Task<CartResponseDTO> AddProductToCartAsync(AddToCartDTO cartDto)
        {
            try
            {
                // Lấy ra CustomerId từ DTO(đã được controller gán nếu null)
                var customerId = cartDto.CustomerId!.Value;
                // Id biến thể sản phẩm cần thêm vào giỏ
                var productDetailId = cartDto.ProductDetailId;
                // Số lượng cần thêm (mặc định tối thiểu 1)
                var quantity = cartDto.Quantity > 0 ? cartDto.Quantity : 1;

                // 1) Kiểm tra biến thể có tồn tại và lấy giá hiện tại
                var variant = await _productDetailRepository.Query()
                    .Where(v => v.Id == productDetailId)
                    .Select(v => new { v.Id, v.Price, v.Status, v.AllowReturn })
                    .FirstOrDefaultAsync();
                if (variant == null)
                {
                    // Không tồn tại biến thể → trả lỗi
                    return new CartResponseDTO
                        { ResponseStatus = BaseStatus.Error, Message = "Biến thể không tồn tại" };
                }

                // 2) Tính tồn kho khả dụng từ bảng InventoryBatch theo ProductDetailId
                var availableStock = await _inventoryBatchRepository.Query()
                    .Where(b => b.ProductDetailId == productDetailId)
                    .SumAsync(b => (int?)b.Quantity) ?? 0;

                // Nếu số lượng yêu cầu vượt tồn kho → trả lỗi
                if (availableStock < quantity)
                {
                    return new CartResponseDTO
                        { ResponseStatus = BaseStatus.Error, Message = "Số lượng không đủ trong kho" };
                }

                // 3) Lấy giỏ hàng hiện tại theo Customer; nếu chưa có thì tạo mới
                var cart = await _cartRepository
                    .Query()
                    .Where(c => c.CustomerId == customerId)
                    .FirstOrDefaultAsync();
                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid(), // Khởi tạo Cart mới
                        CustomerId = customerId,
                        CreationTime = DateTime.UtcNow,
                        TotalPrice = 0
                    };
                    await _cartRepository.Create(cart);
                }

                // 4) Lấy danh sách các dòng cart cùng ProductDetailId trong giỏ để gộp/tách theo chính sách giá & khuyến mại
                var sameVariantItems = await _cartDetaillRepository.Query()
                    .Where(x => x.CartId == cart.Id && x.ProductDetailId == productDetailId)
                    .ToListAsync();

                // Tổng số lượng sau khi cộng thêm (để check tồn kho tổng cho cùng biến thể)
                var totalQtyAfter = sameVariantItems.Sum(x => x.Quantity) + quantity;
                if (totalQtyAfter > availableStock)
                    return new CartResponseDTO
                        { ResponseStatus = BaseStatus.Error, Message = "Vượt quá số lượng tồn kho" };

                // Sai số so sánh float cho giá/discount
                const float eps = 0.0001f;
                // Lấy discount lớn nhất đang áp dụng cho biến thể (không dùng PromotionId từ request)
                var maxDiscount = await _promotionDetailRepository.Query()
                    .Where(p => p.ProductDetailId == productDetailId)
                    .OrderByDescending(p => p.Discount)
                    .Select(p => p.Discount)
                    .FirstOrDefaultAsync();

                // Chính sách: gộp vào dòng có cùng giá và cùng discount; khác giá/discount → tạo dòng mới
                var existingItem = sameVariantItems.FirstOrDefault(x =>
                    Math.Abs(x.Price - variant.Price) < eps &&
                    Math.Abs(x.Discount - maxDiscount) < eps);

                if (existingItem != null)
                {
                    // Gộp số lượng vào dòng hiện có (giữ nguyên discount của dòng đó)
                    existingItem.Quantity += quantity;
                    await _cartDetaillRepository.Update(existingItem);
                }
                else
                {
                    // Tạo mới một dòng cart detail với giá hiện tại và discount lớn nhất
                    await _cartDetaillRepository.Create(new CartDetail
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductDetailId = productDetailId,
                        Quantity = quantity,
                        Price = variant.Price,
                        Discount = maxDiscount
                    });
                }

                // 5) Cập nhật tổng tiền giỏ hàng = tổng((giá - giá*discount%) * quantity) của tất cả dòng
                var total = await _cartDetaillRepository.Query()
                    .Where(d => d.CartId == cart.Id)
                    .SumAsync(i => (decimal)((i.Price - (i.Price * i.Discount / 100f)) * i.Quantity));

                cart.TotalPrice = total; // Gán lại tổng tiền
                await _cartRepository.Update(cart); // Lưu cập nhật giỏ hàng

                // Trả về kết quả thành công
                return new CartResponseDTO
                {
                    ResponseStatus = BaseStatus.Success, Message = "Thêm sản phẩm vào giỏ hàng thành công",
                    Id = cart.Id, CustomersId = cart.CustomerId, TotalPrice = cart.TotalPrice
                };
            }
            catch (Exception e)
            {
                return new CartResponseDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = e.Message
                };
            }
        }

        // Lấy giỏ hàng hiện tại của khách hàng cùng danh sách các dòng chi tiết
        public async Task<CartWithDetailsResponseDTO> GetCurrentCartAsync(Guid customerId)
        {
            // 1) Lấy thông tin giỏ hàng (chỉ định danh & tổng tiền) theo CustomerId
            var cartProjection = await _cartRepository
                .Query()
                .Where(c => c.CustomerId == customerId)
                .Select(c => new { c.Id, c.CustomerId, c.CreationTime, c.LastModificationTime, c.TotalPrice })
                .FirstOrDefaultAsync();

            // Nếu chưa có giỏ hàng → trả về giỏ trống
            if (cartProjection == null)
            {
                return new CartWithDetailsResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Giỏ hàng trống", Items = new() };
            }

            // 2) Lấy danh sách các dòng cart detail của giỏ hàng
            var items = await _cartDetaillRepository.Query()
                .Where(cd => cd.CartId == cartProjection.Id)
                .Select(cd => new CartDetailItemDTO
                {
                    Id = cd.Id,   // Id dòng giỏ
                    ProductName = cd.ProductDetail.Product.Name,
                    Sku = cd.ProductDetail.Sku,
                    SizeName = cd.ProductDetail.Size.Value,
                    ColourName = cd.ProductDetail.Colour.Name,
                    Thumbnail = cd.ProductDetail.Product.Thumbnail,
                    ProductDetailId = cd.ProductDetailId, // Biến thể
                    PromotionDetailId = cd.PromotionDetailId.HasValue? Guid.Parse(cd.PromotionDetailId.ToString()): Guid.Empty, // Mã KM nếu có
                    Quantity = cd.Quantity,              // Số lượng
                    Price = cd.Price,                    // Đơn giá
                    Discount = cd.Discount,              // % giảm
                    
                    // Thông tin vận chuyển từ ProductDetail
                    Weight = cd.ProductDetail.Weight,
                    Length = cd.ProductDetail.Length,
                    Width = cd.ProductDetail.Width,
                    Height = cd.ProductDetail.Height,
                    
                    // Giới hạn mua hàng
                    MaxBuyPerOrder = cd.ProductDetail.MaxBuyPerOrder
                })
                .ToListAsync();

            // 3) Trả về tổng tiền + danh sách chi tiết
            return new CartWithDetailsResponseDTO
            {
                TotalPrice = cartProjection.TotalPrice,
                ResponseStatus = BaseStatus.Success,
                Items = items
            };
        }

        // Cập nhật số lượng cho một dòng giỏ hàng
        public async Task<CartResponseDTO> UpdateCartQuantityAsync(UpdateCartQuantityDTO dto)
        {
            // 1) Lấy dòng giỏ hàng theo CartDetailId
            var item = await _cartDetaillRepository.GetCartDetailById(dto.CartDetailId);
            if (item == null)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy dòng giỏ hàng" };
            }

            // Bảo vệ số lượng tối thiểu là 1
            var newQuantity = dto.Quantity > 0 ? dto.Quantity : 1;

            // 2) Lấy cart tương ứng (theo CustomerId nếu cung cấp; nếu không suy ra từ item.CartId)
            var cart = await _cartRepository.GetCartById(item.CartId);
            if (cart == null || (dto.CustomerId.HasValue && cart.CustomerId != dto.CustomerId.Value))
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Giỏ hàng không hợp lệ" };
            }

            // 3) Kiểm tra tồn kho tổng cho tất cả dòng cùng ProductDetailId trong giỏ (bao gồm dòng hiện tại với số lượng mới)
            var sameVariantItems = await _cartDetaillRepository.Query()
                .Where(x => x.CartId == cart.Id && x.ProductDetailId == item.ProductDetailId && x.Id != item.Id)
                .ToListAsync();

            // Lấy tồn kho khả dụng cho biến thể
            var availableStock = await _inventoryBatchRepository.Query()
                .Where(b => b.ProductDetailId == item.ProductDetailId)
                .SumAsync(b => (int?)b.Quantity) ?? 0;

            // Lấy thông tin giới hạn mua hàng
            var productDetail = await _productDetailRepository.GetByIdAsync(item.ProductDetailId);
            if (productDetail?.MaxBuyPerOrder.HasValue == true && newQuantity > productDetail.MaxBuyPerOrder.Value)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = $"Số lượng vượt quá giới hạn mua hàng ({productDetail.MaxBuyPerOrder.Value} sản phẩm/đơn hàng)" };
            }

            // Tổng dự kiến = các dòng khác + số lượng mới của dòng hiện tại
            var projectedTotal = sameVariantItems.Sum(x => x.Quantity) + newQuantity;
            if (projectedTotal > availableStock)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Vượt quá số lượng tồn kho" };
            }

            // 4) Cập nhật số lượng dòng hiện tại và lưu
            item.Quantity = newQuantity;
            await _cartDetaillRepository.Update(item);

            // 5) Tính lại tổng tiền giỏ hàng
            var total = await _cartDetaillRepository.Query()
                .Where(d => d.CartId == cart.Id)
                .SumAsync(i => (decimal)((i.Price - (i.Price * i.Discount / 100f)) * i.Quantity));

            cart.TotalPrice = total;
            await _cartRepository.Update(cart);

            return new CartResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật số lượng thành công", Id = cart.Id, CustomersId = cart.CustomerId, TotalPrice = cart.TotalPrice };
        }

        // Xoá một dòng giỏ hàng của khách
        public async Task<CartResponseDTO> RemoveCartItemAsync(Guid cartDetailId, Guid customerId)
        {
            // Lấy dòng giỏ hàng
            var item = await _cartDetaillRepository.GetCartDetailById(cartDetailId);
            if (item == null)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy dòng giỏ hàng" };
            }
            // Lấy cart và xác thực quyền sở hữu
            var cart = await _cartRepository.GetCartById(item.CartId);
            if (cart == null || cart.CustomerId != customerId)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Giỏ hàng không hợp lệ" };
            }

            // Xoá dòng giỏ hàng
            await _cartDetaillRepository.Delete(item.Id);

            // Tính lại tổng tiền giỏ hàng
            var total = await _cartDetaillRepository.Query()
                .Where(d => d.CartId == cart.Id)
                .SumAsync(i => (decimal)((i.Price - (i.Price * i.Discount / 100f)) * i.Quantity));
            cart.TotalPrice = total;
            await _cartRepository.Update(cart);

            return new CartResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Đã xoá sản phẩm khỏi giỏ", Id = cart.Id, CustomersId = cart.CustomerId, TotalPrice = cart.TotalPrice };
        }
    }
}
