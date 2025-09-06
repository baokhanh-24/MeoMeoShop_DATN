using System;

namespace MeoMeo.Shared.IServices
{
    public interface ICartStateService
    {
        event Action<int> OnCartCountChanged;
        int CartItemCount { get; }
        void UpdateCartCount(int count);
        void IncrementCartCount(int increment = 1);
        void DecrementCartCount(int decrement = 1);
    }
}
