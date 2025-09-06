using MeoMeo.Shared.IServices;

namespace MeoMeo.Shared.Services
{
    public class CartStateService : ICartStateService
    {
        public event Action<int>? OnCartCountChanged;
        
        private int _cartItemCount = 0;
        
        public int CartItemCount => _cartItemCount;

        public void UpdateCartCount(int count)
        {
            _cartItemCount = count;
            OnCartCountChanged?.Invoke(_cartItemCount);
        }

        public void IncrementCartCount(int increment = 1)
        {
            _cartItemCount += increment;
            OnCartCountChanged?.Invoke(_cartItemCount);
        }

        public void DecrementCartCount(int decrement = 1)
        {
            _cartItemCount = Math.Max(0, _cartItemCount - decrement);
            OnCartCountChanged?.Invoke(_cartItemCount);
        }
    }
}
