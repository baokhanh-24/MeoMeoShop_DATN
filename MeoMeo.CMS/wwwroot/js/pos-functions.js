// POS Functions for OrderAtCounter
window.posFunctions = {
    // Barcode Scanner Functions
    startBarcodeScan: async function () {
        try {
            // Check if camera is available
            if (
                !navigator.mediaDevices ||
                !navigator.mediaDevices.getUserMedia
            ) {
                throw new Error(
                    'Camera không được hỗ trợ trên trình duyệt này'
                );
            }

            // Request camera permission
            const stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    facingMode: 'environment', // Use back camera on mobile
                },
            });
            ``````;

            // Create video element
            const video = document.createElement('video');
            video.srcObject = stream;
            video.style.position = 'fixed';
            video.style.top = '0';
            video.style.left = '0';
            video.style.width = '100%';
            video.style.height = '100%';
            video.style.zIndex = '9999';
            video.style.backgroundColor = 'black';
            video.autoplay = true;
            video.playsInline = true;

            // Create overlay
            const overlay = document.createElement('div');
            overlay.style.position = 'fixed';
            overlay.style.top = '0';
            overlay.style.left = '0';
            overlay.style.width = '100%';
            overlay.style.height = '100%';
            overlay.style.zIndex = '10000';
            overlay.style.backgroundColor = 'rgba(0,0,0,0.5)';
            overlay.style.display = 'flex';
            overlay.style.alignItems = 'center';
            overlay.style.justifyContent = 'center';

            // Create scanning frame
            const frame = document.createElement('div');
            frame.style.width = '250px';
            frame.style.height = '150px';
            frame.style.border = '2px solid #1890ff';
            frame.style.borderRadius = '8px';
            frame.style.position = 'relative';
            frame.style.backgroundColor = 'transparent';

            // Add corner indicators
            const corners = [
                'top-left',
                'top-right',
                'bottom-left',
                'bottom-right',
            ];
            corners.forEach((corner) => {
                const cornerEl = document.createElement('div');
                cornerEl.style.position = 'absolute';
                cornerEl.style.width = '20px';
                cornerEl.style.height = '20px';
                cornerEl.style.border = '3px solid #1890ff';
                cornerEl.style.backgroundColor = 'transparent';

                switch (corner) {
                    case 'top-left':
                        cornerEl.style.top = '-3px';
                        cornerEl.style.left = '-3px';
                        cornerEl.style.borderRight = 'none';
                        cornerEl.style.borderBottom = 'none';
                        break;
                    case 'top-right':
                        cornerEl.style.top = '-3px';
                        cornerEl.style.right = '-3px';
                        cornerEl.style.borderLeft = 'none';
                        cornerEl.style.borderBottom = 'none';
                        break;
                    case 'bottom-left':
                        cornerEl.style.bottom = '-3px';
                        cornerEl.style.left = '-3px';
                        cornerEl.style.borderRight = 'none';
                        cornerEl.style.borderTop = 'none';
                        break;
                    case 'bottom-right':
                        cornerEl.style.bottom = '-3px';
                        cornerEl.style.right = '-3px';
                        cornerEl.style.borderLeft = 'none';
                        cornerEl.style.borderTop = 'none';
                        break;
                }
                frame.appendChild(cornerEl);
            });

            // Add instruction text
            const instruction = document.createElement('div');
            instruction.style.position = 'absolute';
            instruction.style.bottom = '-40px';
            instruction.style.left = '50%';
            instruction.style.transform = 'translateX(-50%)';
            instruction.style.color = 'white';
            instruction.style.fontSize = '16px';
            instruction.style.textAlign = 'center';
            instruction.textContent = 'Đưa barcode vào khung để quét';
            frame.appendChild(instruction);

            // Add close button
            const closeBtn = document.createElement('button');
            closeBtn.style.position = 'absolute';
            closeBtn.style.top = '20px';
            closeBtn.style.right = '20px';
            closeBtn.style.background = 'rgba(0,0,0,0.7)';
            closeBtn.style.color = 'white';
            closeBtn.style.border = 'none';
            closeBtn.style.borderRadius = '50%';
            closeBtn.style.width = '40px';
            closeBtn.style.height = '40px';
            closeBtn.style.fontSize = '20px';
            closeBtn.style.cursor = 'pointer';
            closeBtn.textContent = '×';
            closeBtn.onclick = () => {
                stream.getTracks().forEach((track) => track.stop());
                document.body.removeChild(video);
                document.body.removeChild(overlay);
            };

            overlay.appendChild(frame);
            overlay.appendChild(closeBtn);
            document.body.appendChild(video);
            document.body.appendChild(overlay);

            // Simulate barcode detection (in real implementation, use a barcode scanning library like QuaggaJS or ZXing)
            return new Promise((resolve) => {
                // For demo purposes, return a mock barcode after 3 seconds
                setTimeout(() => {
                    stream.getTracks().forEach((track) => track.stop());
                    document.body.removeChild(video);
                    document.body.removeChild(overlay);
                    resolve('1234567890123'); // Mock barcode
                }, 3000);
            });
        } catch (error) {
            console.error('Barcode scan error:', error);
            throw error;
        }
    },

    // Print Receipt Functions
    printReceipt: function (receiptData) {
        try {
            // Create receipt HTML
            const receiptHtml = `
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <title>Hóa đơn bán hàng</title>
                    <style>
                        body {
                            font-family: 'Courier New', monospace;
                            font-size: 12px;
                            margin: 0;
                            padding: 10px;
                            width: 300px;
                        }
                        .header {
                            text-align: center;
                            border-bottom: 1px dashed #000;
                            padding-bottom: 10px;
                            margin-bottom: 10px;
                        }
                        .company-name {
                            font-weight: bold;
                            font-size: 16px;
                            margin-bottom: 5px;
                        }
                        .receipt-info {
                            margin-bottom: 10px;
                        }
                        .receipt-info div {
                            margin-bottom: 3px;
                        }
                        .items-table {
                            width: 100%;
                            border-collapse: collapse;
                            margin-bottom: 10px;
                        }
                        .items-table th,
                        .items-table td {
                            padding: 3px 0;
                            text-align: left;
                            border-bottom: 1px dashed #ccc;
                        }
                        .items-table th {
                            font-weight: bold;
                            border-bottom: 1px solid #000;
                        }
                        .items-table .qty {
                            text-align: center;
                            width: 30px;
                        }
                        .items-table .price {
                            text-align: right;
                            width: 60px;
                        }
                        .items-table .total {
                            text-align: right;
                            width: 60px;
                        }
                        .summary {
                            border-top: 1px dashed #000;
                            padding-top: 10px;
                        }
                        .summary-row {
                            display: flex;
                            justify-content: space-between;
                            margin-bottom: 3px;
                        }
                        .summary-row.total {
                            font-weight: bold;
                            font-size: 14px;
                            border-top: 1px solid #000;
                            padding-top: 5px;
                            margin-top: 5px;
                        }
                        .footer {
                            text-align: center;
                            margin-top: 20px;
                            font-size: 10px;
                        }
                        @media print {
                            body { margin: 0; }
                        }
                    </style>
                </head>
                <body>
                    <div class="header">
                        <div class="company-name">MEOMEO SHOP</div>
                        <div>Địa chỉ: 123 Đường ABC, Quận XYZ, TP.HCM</div>
                        <div>Điện thoại: 0123-456-789</div>
                    </div>
                    
                    <div class="receipt-info">
                        <div><strong>Mã đơn:</strong> ${
                            receiptData.OrderCode
                        }</div>
                        <div><strong>Thời gian:</strong> ${new Date(
                            receiptData.OrderTime
                        ).toLocaleString('vi-VN')}</div>
                        <div><strong>Khách hàng:</strong> ${
                            receiptData.CustomerName
                        }</div>
                        ${
                            receiptData.CustomerPhone
                                ? `<div><strong>SĐT:</strong> ${receiptData.CustomerPhone}</div>`
                                : ''
                        }
                    </div>
                    
                    <table class="items-table">
                        <thead>
                            <tr>
                                <th>Tên sản phẩm</th>
                                <th class="qty">SL</th>
                                <th class="price">Đơn giá</th>
                                <th class="total">Thành tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${receiptData.Items.map(
                                (item) => `
                                <tr>
                                    <td>${item.Name}</td>
                                    <td class="qty">${item.Quantity}</td>
                                    <td class="price">${item.Price.toLocaleString(
                                        'vi-VN'
                                    )}</td>
                                    <td class="total">${item.Total.toLocaleString(
                                        'vi-VN'
                                    )}</td>
                                </tr>
                            `
                            ).join('')}
                        </tbody>
                    </table>
                    
                    <div class="summary">
                        <div class="summary-row">
                            <span>Tạm tính:</span>
                            <span>${receiptData.SubTotal.toLocaleString(
                                'vi-VN'
                            )} đ</span>
                        </div>
                        ${
                            receiptData.ShippingFee > 0
                                ? `
                            <div class="summary-row">
                                <span>Phí vận chuyển:</span>
                                <span>${receiptData.ShippingFee.toLocaleString(
                                    'vi-VN'
                                )} đ</span>
                            </div>
                        `
                                : ''
                        }
                        ${
                            receiptData.DiscountAmount > 0
                                ? `
                            <div class="summary-row">
                                <span>Giảm giá:</span>
                                <span>-${receiptData.DiscountAmount.toLocaleString(
                                    'vi-VN'
                                )} đ</span>
                            </div>
                        `
                                : ''
                        }
                        <div class="summary-row total">
                            <span>TỔNG CỘNG:</span>
                            <span>${receiptData.Total.toLocaleString(
                                'vi-VN'
                            )} đ</span>
                        </div>
                        <div class="summary-row">
                            <span>Thanh toán:</span>
                            <span>${receiptData.PaymentMethod}</span>
                        </div>
                    </div>
                    
                    <div class="footer">
                        <div>Cảm ơn quý khách!</div>
                        <div>Hẹn gặp lại!</div>
                    </div>
                </body>
                </html>
            `;

            // Open print window
            const printWindow = window.open(
                '',
                '_blank',
                'width=400,height=600'
            );
            printWindow.document.write(receiptHtml);
            printWindow.document.close();

            // Wait for content to load then print
            printWindow.onload = function () {
                setTimeout(() => {
                    printWindow.print();
                    printWindow.close();
                }, 500);
            };
        } catch (error) {
            console.error('Print receipt error:', error);
            throw error;
        }
    },

    // Utility Functions
    setFocus: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.focus();
        }
    },

    addKeyboardShortcuts: function () {
        document.addEventListener('keydown', function (e) {
            // Ctrl + S: Save order
            if (e.ctrlKey && e.key === 's') {
                e.preventDefault();
                // Trigger save button click
                const saveBtn = document.querySelector(
                    '[onclick*="HandleSave"]'
                );
                if (saveBtn) {
                    saveBtn.click();
                }
            }

            // Ctrl + P: Print receipt
            if (e.ctrlKey && e.key === 'p') {
                e.preventDefault();
                const printBtn = document.querySelector(
                    '[onclick*="HandlePrintOrder"]'
                );
                if (printBtn) {
                    printBtn.click();
                }
            }

            // Ctrl + N: New order
            if (e.ctrlKey && e.key === 'n') {
                e.preventDefault();
                const cancelBtn = document.querySelector(
                    '[onclick*="HandleCancel"]'
                );
                if (cancelBtn) {
                    cancelBtn.click();
                }
            }

            // F1: Focus search
            if (e.key === 'F1') {
                e.preventDefault();
                const searchInput = document.getElementById(
                    'product-search-input'
                );
                if (searchInput) {
                    searchInput.focus();
                }
            }

            // F2: Barcode scan
            if (e.key === 'F2') {
                e.preventDefault();
                const scanBtn = document.querySelector(
                    '[onclick*="StartBarcodeScan"]'
                );
                if (scanBtn) {
                    scanBtn.click();
                }
            }

            // Escape: Close dropdowns/modals
            if (e.key === 'Escape') {
                const dropdowns = document.querySelectorAll(
                    '.search-results-dropdown'
                );
                dropdowns.forEach((dropdown) => {
                    dropdown.style.display = 'none';
                });
            }
        });
    },

    // Enhanced barcode scanning with QuaggaJS (optional)
    initBarcodeScanner: function () {
        // This would integrate with QuaggaJS for real barcode scanning
        // For now, we'll use the camera-based approach above
        console.log('Barcode scanner initialized');
    },
};

// Make functions globally available
window.startBarcodeScan = window.posFunctions.startBarcodeScan;
window.printReceipt = window.posFunctions.printReceipt;
window.setFocus = window.posFunctions.setFocus;
window.addKeyboardShortcuts = window.posFunctions.addKeyboardShortcuts;
