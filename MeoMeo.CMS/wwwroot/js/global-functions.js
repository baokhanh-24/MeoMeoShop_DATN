// Global function exports for Blazor interop
(function () {
    'use strict';

    // Wait for posFunctions to be available
    function ensureFunctions() {
        if (typeof window.posFunctions !== 'undefined') {
            // Export functions globally
            window.startBarcodeScan = window.posFunctions.startBarcodeScan;
            window.startQRCodeScan = window.posFunctions.startQRCodeScan;
            window.printReceipt = window.posFunctions.printReceipt;
            window.printQRCode = window.posFunctions.printQRCode;
            window.setFocus = window.posFunctions.setFocus;
            window.addKeyboardShortcuts =
                window.posFunctions.addKeyboardShortcuts;

            console.log('Functions exported successfully:', {
                startBarcodeScan: typeof window.startBarcodeScan,
                startQRCodeScan: typeof window.startQRCodeScan,
                printReceipt: typeof window.printReceipt,
                printQRCode: typeof window.printQRCode,
                setFocus: typeof window.setFocus,
                addKeyboardShortcuts: typeof window.addKeyboardShortcuts,
            });

            return true;
        }
        return false;
    }

    // Try to export functions immediately
    if (!ensureFunctions()) {
        // If not available, try again after a short delay
        setTimeout(ensureFunctions, 100);

        // Also try on DOM ready
        document.addEventListener('DOMContentLoaded', ensureFunctions);

        // And on window load
        window.addEventListener('load', ensureFunctions);

        // Final attempt after 2 seconds
        setTimeout(ensureFunctions, 2000);
    }
})();
