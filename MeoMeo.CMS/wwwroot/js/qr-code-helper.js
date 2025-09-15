// QR Code Helper Functions

window.printQRCode = function (printData) {
    // Tạo cửa sổ in mới
    const printWindow = window.open('', '_blank');

    if (!printWindow) {
        alert('Không thể mở cửa sổ in. Vui lòng kiểm tra popup blocker.');
        return;
    }

    // HTML template cho trang in
    const printContent = `
        <!DOCTYPE html>
        <html>
        <head>
            <title>In QR Code - ${printData.ProductName}</title>
            <style>
                @media print {
                    @page { margin: 1cm; }
                    body { margin: 0; }
                }
                body {
                    font-family: Arial, sans-serif;
                    text-align: center;
                    padding: 20px;
                }
                .print-container {
                    max-width: 400px;
                    margin: 0 auto;
                    border: 2px solid #000;
                    padding: 20px;
                    border-radius: 10px;
                }
                .title {
                    font-size: 18px;
                    font-weight: bold;
                    margin-bottom: 15px;
                    color: #1890ff;
                }
                .product-info {
                    text-align: left;
                    margin-bottom: 20px;
                }
                .product-info div {
                    margin: 5px 0;
                    font-size: 14px;
                }
                .product-name {
                    font-weight: bold;
                    font-size: 16px;
                    color: #333;
                }
                .price {
                    color: #f5222d;
                    font-weight: bold;
                }
                .qr-section {
                    margin: 20px 0;
                }
                .qr-code {
                    border: 2px solid #f0f0f0;
                    border-radius: 8px;
                    padding: 10px;
                    display: inline-block;
                }
                .instructions {
                    font-size: 12px;
                    color: #666;
                    margin-top: 15px;
                    line-height: 1.4;
                }
                .print-time {
                    font-size: 10px;
                    color: #999;
                    margin-top: 15px;
                    border-top: 1px solid #f0f0f0;
                    padding-top: 10px;
                }
            </style>
        </head>
        <body>
            <div class="print-container">
                <div class="title">${printData.Title}</div>
                
                <div class="product-info">
                    <div class="product-name">${printData.ProductName}</div>
                    <div><strong>SKU:</strong> ${printData.Sku}</div>
                    <div><strong>Thương hiệu:</strong> ${printData.Brand}</div>
                    <div class="price"><strong>Giá:</strong> ${printData.Price.toLocaleString(
                        'vi-VN'
                    )} đ</div>
                    <div><strong>Size:</strong> ${printData.Size}</div>
                    <div><strong>Màu:</strong> ${printData.Color}</div>
                </div>

                <div class="qr-section">
                    <div id="qr-placeholder" style="display: flex; justify-content: center;"></div>
                </div>


                <div class="print-time">
                    Thời gian in: ${printData.PrintTime}
                </div>
            </div>

            <script>
                // Tìm QR code element từ trang gốc và copy vào trang in
                setTimeout(() => {
                    const originalQRCode = parent.document.querySelector('[id*="product-qr-"]');
                    if (originalQRCode) {
                        const qrPlaceholder = document.getElementById('qr-placeholder');
                        const clonedQR = originalQRCode.cloneNode(true);
                        qrPlaceholder.appendChild(clonedQR);
                    }
                    
                    // Tự động in sau khi load xong
                    setTimeout(() => {
                        window.print();
                        window.close();
                    }, 500);
                }, 100);
            </script>
        </body>
        </html>
    `;

    printWindow.document.write(printContent);
    printWindow.document.close();
};

window.downloadQRCode = function (qrElementId, fileName) {
    try {
        // Tìm QR code element
        const qrElement = document.getElementById(qrElementId);
        if (!qrElement) {
            alert('Không tìm thấy QR code để tải về');
            return;
        }

        // Tìm SVG element bên trong
        const svgElement = qrElement.querySelector('svg');
        if (!svgElement) {
            alert('Không tìm thấy SVG element');
            return;
        }

        // Tạo canvas để convert SVG sang PNG
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        // Lấy kích thước của SVG
        const svgRect = svgElement.getBoundingClientRect();
        canvas.width = svgRect.width;
        canvas.height = svgRect.height;

        // Convert SVG to data URL
        const svgData = new XMLSerializer().serializeToString(svgElement);
        const svgBlob = new Blob([svgData], {
            type: 'image/svg+xml;charset=utf-8',
        });
        const svgUrl = URL.createObjectURL(svgBlob);

        // Tạo image và vẽ lên canvas
        const img = new Image();
        img.onload = function () {
            ctx.fillStyle = 'white';
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0);

            // Tạo download link
            canvas.toBlob(function (blob) {
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = fileName || 'qr-code.png';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }, 'image/png');

            URL.revokeObjectURL(svgUrl);
        };

        img.onerror = function () {
            // Fallback: Download SVG directly
            const url = URL.createObjectURL(svgBlob);
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName.replace('.png', '.svg') || 'qr-code.svg';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        };

        img.src = svgUrl;
    } catch (error) {
        console.error('Error downloading QR code:', error);
        alert('Có lỗi khi tải QR code: ' + error.message);
    }
};
