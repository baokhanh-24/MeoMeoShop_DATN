// POS Functions for OrderAtCounter
(function () {
    'use strict';

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

                overlay.appendChild(frame);
                overlay.appendChild(closeBtn);
                document.body.appendChild(video);
                document.body.appendChild(overlay);

                // Real barcode detection using QuaggaJS
                return new Promise((resolve) => {
                    let isScanning = true;

                    // Initialize QuaggaJS
                    Quagga.init(
                        {
                            inputStream: {
                                name: 'Live',
                                type: 'LiveStream',
                                target: video,
                                constraints: {
                                    width: 640,
                                    height: 480,
                                    facingMode: 'environment',
                                },
                            },
                            decoder: {
                                readers: [
                                    'code_128_reader',
                                    'ean_reader',
                                    'ean_8_reader',
                                    'code_39_reader',
                                    'code_39_vin_reader',
                                    'codabar_reader',
                                    'upc_reader',
                                    'upc_e_reader',
                                    'i2of5_reader',
                                ],
                            },
                            locate: true,
                            locator: {
                                patchSize: 'medium',
                                halfSample: true,
                            },
                        },
                        function (err) {
                            if (err) {
                                console.error(
                                    'QuaggaJS initialization error:',
                                    err
                                );
                                isScanning = false;
                                stream
                                    .getTracks()
                                    .forEach((track) => track.stop());
                                document.body.removeChild(video);
                                document.body.removeChild(overlay);
                                resolve(null);
                                return;
                            }

                            // Start QuaggaJS
                            Quagga.start();

                            // Listen for barcode detection
                            Quagga.onDetected(function (data) {
                                if (!isScanning) return;

                                const code = data.codeResult.code;
                                console.log('Barcode detected:', code);

                                isScanning = false;
                                Quagga.stop();
                                stream
                                    .getTracks()
                                    .forEach((track) => track.stop());
                                document.body.removeChild(video);
                                document.body.removeChild(overlay);

                                resolve(code);
                            });
                        }
                    );

                    // Update close button to stop scanning
                    closeBtn.onclick = () => {
                        isScanning = false;
                        Quagga.stop();
                        stream.getTracks().forEach((track) => track.stop());
                        document.body.removeChild(video);
                        document.body.removeChild(overlay);
                        resolve(null); // User cancelled
                    };

                    // Add timeout to prevent infinite scanning
                    setTimeout(() => {
                        if (isScanning) {
                            isScanning = false;
                            Quagga.stop();
                            stream.getTracks().forEach((track) => track.stop());
                            document.body.removeChild(video);
                            document.body.removeChild(overlay);
                            resolve(null); // No barcode found
                        }
                    }, 30000); // 30 seconds timeout
                });
            } catch (error) {
                console.error('Barcode scan error:', error);
                throw error;
            }
        },

        // QR Code Scanner Functions
        startQRCodeScan: async function () {
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

                // Create scanning frame (square for QR code)
                const frame = document.createElement('div');
                frame.style.width = '250px';
                frame.style.height = '250px';
                frame.style.border = '2px solid #52c41a';
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
                    cornerEl.style.border = '3px solid #52c41a';
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
                instruction.textContent =
                    'Đưa QR code sản phẩm vào khung để quét';
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

                overlay.appendChild(frame);
                overlay.appendChild(closeBtn);
                document.body.appendChild(video);
                document.body.appendChild(overlay);

                // Real QR code detection using jsQR
                return new Promise((resolve) => {
                    let scanInterval;
                    let isScanning = true;

                    // Update close button to stop scanning
                    closeBtn.onclick = () => {
                        isScanning = false;
                        if (scanInterval) clearInterval(scanInterval);
                        stream.getTracks().forEach((track) => track.stop());
                        document.body.removeChild(video);
                        document.body.removeChild(overlay);
                        resolve(null); // User cancelled
                    };

                    // Start scanning loop
                    const startScanning = () => {
                        scanInterval = setInterval(() => {
                            if (!isScanning) return;

                            // Create canvas to capture video frame
                            const canvas = document.createElement('canvas');
                            const context = canvas.getContext('2d');

                            // Set canvas size to match video
                            canvas.width = video.videoWidth;
                            canvas.height = video.videoHeight;

                            // Draw video frame to canvas
                            context.drawImage(
                                video,
                                0,
                                0,
                                canvas.width,
                                canvas.height
                            );

                            // Get image data
                            const imageData = context.getImageData(
                                0,
                                0,
                                canvas.width,
                                canvas.height
                            );

                            // Use jsQR to detect QR code
                            const code = jsQR(
                                imageData.data,
                                imageData.width,
                                imageData.height
                            );

                            if (code) {
                                // QR code detected!
                                console.log('QR Code detected:', code.data);
                                isScanning = false;
                                clearInterval(scanInterval);

                                // Clean up
                                stream
                                    .getTracks()
                                    .forEach((track) => track.stop());
                                document.body.removeChild(video);
                                document.body.removeChild(overlay);

                                resolve(code.data);
                            }
                        }, 100); // Check every 100ms
                    };

                    // Start scanning when video is ready
                    video.onloadedmetadata = () => {
                        video.play().then(() => {
                            startScanning();
                        });
                    };

                    // Add timeout to prevent infinite scanning
                    setTimeout(() => {
                        if (isScanning) {
                            isScanning = false;
                            clearInterval(scanInterval);
                            stream.getTracks().forEach((track) => track.stop());
                            document.body.removeChild(video);
                            document.body.removeChild(overlay);
                            resolve(null); // No QR code found
                        }
                    }, 30000); // 30 seconds timeout
                });
            } catch (error) {
                console.error('QR code scan error:', error);
                throw error;
            }
        },

        // Print QR Code Functions
        printQRCode: function (qrData) {
            try {
                // Create QR code print HTML
                const printHtml = `
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <title>QR Code Sản Phẩm</title>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            font-size: 12px;
                            margin: 0;
                            padding: 20px;
                            text-align: center;
                        }
                        .qr-header {
                            margin-bottom: 20px;
                        }
                        .company-name {
                            font-weight: bold;
                            font-size: 18px;
                            margin-bottom: 10px;
                        }
                        .product-info {
                            margin-bottom: 20px;
                            text-align: left;
                            max-width: 300px;
                            margin-left: auto;
                            margin-right: auto;
                        }
                        .product-info div {
                            margin-bottom: 5px;
                        }
                        .qr-code-container {
                            margin: 20px 0;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 8px;
                            display: inline-block;
                        }
                        .qr-instruction {
                            margin-top: 20px;
                            font-style: italic;
                            color: #666;
                        }
                        .print-time {
                            margin-top: 20px;
                            font-size: 10px;
                            color: #999;
                        }
                        @media print {
                            body { margin: 0; }
                        }
                    </style>
                </head>
                <body>
                
                    <div class="product-info">
                        <div><strong>Tên sản phẩm:</strong> ${
                            qrData.ProductName
                        }</div>
                        <div><strong>SKU:</strong> ${qrData.Sku}</div>
                        <div><strong>Giá:</strong> ${qrData.Price.toLocaleString(
                            'vi-VN'
                        )} đ</div>
                        <div><strong>Thương hiệu:</strong> ${qrData.Brand}</div>
                        <div><strong>Size:</strong> ${qrData.Size}</div>
                        <div><strong>Màu:</strong> ${qrData.Color}</div>
                    </div>
                    
                    <div class="qr-code-container">
                        <div style="font-size: 14px; margin-bottom: 10px; font-weight: bold;">
                            Quét mã QR để thanh toán
                        </div>
                        <!-- QR Code sẽ được tạo bởi component Blazor -->
                        <div style="width: 200px; height: 200px; border: 1px solid #ccc; margin: 0 auto; display: flex; align-items: center; justify-content: center; background: #f9f9f9;">
                            [QR Code sẽ hiển thị ở đây]
                        </div>
                    </div>
                    
                    <div class="qr-instruction">
                        Quét mã QR này bằng ứng dụng thanh toán để thêm sản phẩm vào giỏ hàng
                    </div>
                    
                    <div class="print-time">
                        In lúc: ${qrData.PrintTime}
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
                printWindow.document.write(printHtml);
                printWindow.document.close();

                // Wait for content to load then print
                printWindow.onload = function () {
                    setTimeout(() => {
                        printWindow.print();
                        printWindow.close();
                    }, 500);
                };
            } catch (error) {
                console.error('Print QR code error:', error);
                throw error;
            }
        },

        // Print Receipt Functions
        printReceipt: function (receiptData) {
            try {
                // Debug: Log receipt data structure
                console.log('Receipt data received:', receiptData);
                console.log('Items:', receiptData.Items);

                // Validate receipt data
                if (!receiptData) {
                    throw new Error('Receipt data is null or undefined');
                }

                // Ensure Items is an array
                const items = Array.isArray(receiptData.Items)
                    ? receiptData.Items
                    : [];
                console.log('Items array:', items);
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
                            receiptData.OrderCode || 'N/A'
                        }</div>
                        <div><strong>Thời gian:</strong> ${
                            receiptData.OrderTime
                                ? new Date(
                                      receiptData.OrderTime
                                  ).toLocaleString('vi-VN')
                                : new Date().toLocaleString('vi-VN')
                        }</div>
                        <div><strong>Khách hàng:</strong> ${
                            receiptData.CustomerName || 'Khách lẻ'
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
                            ${items
                                .map(
                                    (item) => `
                                <tr>
                                    <td>${item.Name || 'N/A'}</td>
                                    <td class="qty">${item.Quantity || 0}</td>
                                    <td class="price">${(
                                        item.Price || 0
                                    ).toLocaleString('vi-VN')}</td>
                                    <td class="total">${(
                                        item.Total || 0
                                    ).toLocaleString('vi-VN')}</td>
                                </tr>
                            `
                                )
                                .join('')}
                        </tbody>
                    </table>
                    
                    <div class="summary">
                        <div class="summary-row">
                            <span>Tạm tính:</span>
                            <span>${(receiptData.SubTotal || 0).toLocaleString(
                                'vi-VN'
                            )} đ</span>
                        </div>
                        ${
                            (receiptData.ShippingFee || 0) > 0
                                ? `
                            <div class="summary-row">
                                <span>Phí vận chuyển:</span>
                                <span>${(
                                    receiptData.ShippingFee || 0
                                ).toLocaleString('vi-VN')} đ</span>
                            </div>
                        `
                                : ''
                        }
                        ${
                            (receiptData.DiscountAmount || 0) > 0
                                ? `
                            <div class="summary-row">
                                <span>Giảm giá:</span>
                                <span>-${(
                                    receiptData.DiscountAmount || 0
                                ).toLocaleString('vi-VN')} đ</span>
                            </div>
                        `
                                : ''
                        }
                        <div class="summary-row total">
                            <span>TỔNG CỘNG:</span>
                            <span>${(receiptData.Total || 0).toLocaleString(
                                'vi-VN'
                            )} đ</span>
                        </div>
                        <div class="summary-row">
                            <span>Thanh toán:</span>
                            <span>${
                                receiptData.PaymentMethod || 'Tiền mặt'
                            }</span>
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

                // F3: QR code scan
                if (e.key === 'F3') {
                    e.preventDefault();
                    const qrScanBtn = document.querySelector(
                        '[onclick*="StartQRCodeScan"]'
                    );
                    if (qrScanBtn) {
                        qrScanBtn.click();
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

    // Make functions globally available immediately
    window.startBarcodeScan = window.posFunctions.startBarcodeScan;
    window.startQRCodeScan = window.posFunctions.startQRCodeScan;
    window.printReceipt = window.posFunctions.printReceipt;
    window.printQRCode = window.posFunctions.printQRCode;
    window.setFocus = window.posFunctions.setFocus;
    window.addKeyboardShortcuts = window.posFunctions.addKeyboardShortcuts;
})(); // End IIFE

// Simple export - global-functions.js will handle the rest
console.log('posFunctions loaded:', typeof window.posFunctions);
