// Product Comparison JavaScript Functions
window.addToRecentProducts = function (productId) {
    try {
        let recentIds = localStorage.getItem('recent_product_ids') || '';
        let ids = recentIds ? recentIds.split(',') : [];

        // Remove if already exists to avoid duplicates
        ids = ids.filter((id) => id !== productId);

        // Add to beginning (most recent first)
        ids.unshift(productId);

        // Keep only 20 most recent products
        ids = ids.slice(0, 20);

        // Save back to localStorage
        localStorage.setItem('recent_product_ids', ids.join(','));

        console.log('Product added to recent list:', productId);
    } catch (error) {
        console.error('Error adding product to recent list:', error);
    }
};

window.getRecentProducts = function () {
    try {
        let recentIds = localStorage.getItem('recent_product_ids') || '';
        return recentIds ? recentIds.split(',') : [];
    } catch (error) {
        console.error('Error getting recent products:', error);
        return [];
    }
};

window.clearRecentProducts = function () {
    try {
        localStorage.removeItem('recent_product_ids');
        console.log('Recent products cleared');
    } catch (error) {
        console.error('Error clearing recent products:', error);
    }
};

// Auto-save product when page loads (for product detail pages)
window.addEventListener('DOMContentLoaded', function () {
    // Check if we're on a product detail page
    const path = window.location.pathname;
    const productDetailMatch = path.match(/\/san-pham\/([a-f0-9-]+)/);

    if (productDetailMatch) {
        const productId = productDetailMatch[1];
        window.addToRecentProducts(productId);
    }
});
