import { Flex, Tooltip } from "antd";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { GetOrderAtCounterDetailsResponse, OrderAtCounterProduct } from "@/@schema/api/order-at-counter/details";
import { useRouter } from "next/router";
import { InfoCircleOutlined } from "@ant-design/icons";

interface ProductInfoViewProps {
    orderDetails: GetOrderAtCounterDetailsResponse;
    pathCDN: string;
    hasPermission: (permission: string) => boolean;
}

interface TransformedProduct {
    productId: number;
    comboId: number | null;
    sku: string | null;
    name: string;
    thumbnail: string | null;
    unitName: string | null;
    price: number;
    discountPrice: number | null;
    quantity: number;
    promotedQuantity: number;
    nonPromotedQuantity: number;
    total: number;
}

const ProductInfoView = ({ orderDetails, pathCDN, hasPermission }: ProductInfoViewProps) => {
    const router = useRouter();
    const formatPrice = (value: number) => {
        return value.toLocaleString("vi-VN");
    };

    const calcProductTotal = (product: OrderAtCounterProduct) => {
        let total = 0;
        if (product.discountValue > 0) {
            total += product.price * product.nonPromotedQuantity;
            total += (product.price - (product.price * product.discountValue) / 100) * product.promotedQuantity;
        } else {
            total += product.price * product.quantity;
        }
        return total;
    };

    // Transform products data
    const products: TransformedProduct[] = orderDetails.products.map((product) => ({
        productId: product.productId,
        comboId: product.comboId,
        sku: product.sku,
        name: product.productName,
        thumbnail: product.thumbnail,
        unitName: product.unitName,
        price: product.price,
        discountPrice: product.discountValue > 0 ? Math.round(product.price - (product.price * product.discountValue) / 100) : null,
        quantity: product.quantity,
        promotedQuantity: product.promotedQuantity,
        nonPromotedQuantity: product.nonPromotedQuantity,
        total: calcProductTotal(product)
    }));

    // Calculate totals
    const subTotal = products.reduce((sum, product) => sum + product.total, 0);
    const shippingFee = orderDetails.shippingFee;
    const discountCodeValue = orderDetails.discountValue;
    const membershipDiscount = orderDetails.minusMembership || 0;
    const pointsDiscount = orderDetails.minusPointsValue || 0;

    const handleSkuClick = (productId: number, comboId: number | null) => {
        if (comboId) {
            window.open(`/product/combo/${comboId}`, "_blank");
        } else {
            window.open(`/product/detail-product/${productId}`, "_blank");
        }
    };

    const skuTemplate = (rowData: TransformedProduct) => {
        const hasViewPermission = rowData.productId
            ? hasPermission("PRODUCT_MANAGE:PRODUCT:VIEW")
            : hasPermission("PRODUCT_MANAGE:PRODUCT_COMBO:VIEW");
        return (
            <span
                className="txt-14-n-500"
                style={{
                    color: "#188A42",
                    cursor: hasViewPermission ? "pointer" : "default"
                }}
                onClick={hasViewPermission ? () => handleSkuClick(rowData.productId, rowData.comboId) : undefined}
            >
                {rowData.sku || ""}
            </span>
        );
    };

    const priceTemplate = (rowData: TransformedProduct) => {
        return (
            <Flex vertical justify="center" align="flex-end" gap={4}>
                <span className="txt-16-n-500" style={{ color: "#FF3D32" }}>
                    {formatPrice(rowData.discountPrice || rowData.price)}
                </span>
                {rowData.discountPrice && (
                    <span className="txt-13-n-400" style={{ textDecoration: "line-through" }}>
                        {formatPrice(rowData.price)}
                    </span>
                )}
            </Flex>
        );
    };

    const totalPriceTemplate = (rowData: TransformedProduct) => {
        return (
            <div style={{ display: "flex", justifyContent: "flex-end", alignItems: "center", width: "100%" }}>
                {rowData.total.toLocaleString("vi-VN")}
                {rowData.promotedQuantity && rowData.nonPromotedQuantity ? (
                    <Tooltip title={`${rowData.promotedQuantity} sản phẩm được giảm giá, ${rowData.nonPromotedQuantity} sản phẩm giá gốc`}>
                        <InfoCircleOutlined style={{ marginLeft: 8, color: "#646F79", fontSize: 16 }} />
                    </Tooltip>
                ) : null}
            </div>
        );
    };

    return (
        <Flex
            style={{
                background: "white",
                borderRadius: 16,
                overflow: "hidden"
            }}
            vertical
        >
            <DataTable value={products} showGridlines={false} style={{ width: "100%", maxWidth: "100%", padding: "8px" }}>
                <Column field="sku" header="Mã sản phẩm" body={skuTemplate} style={{ width: "116px", minWidth: "116px" }} />
                <Column
                    field="name"
                    header="Sản phẩm"
                    style={{ flex: "1 1 0", maxWidth: "1px", minWidth: "220px" }}
                    body={(rowData: TransformedProduct) => (
                        <Flex align="center">
                            <img
                                src={`${pathCDN}/${rowData.thumbnail}`}
                                alt={rowData.name}
                                style={{
                                    width: "60px",
                                    height: "40px",
                                    marginRight: "16px",
                                    borderRadius: "4px",
                                    border: "1px solid rgba(34, 49, 63, 0.10)",
                                    objectFit: "contain"
                                }}
                            />
                            <span
                                className="txt-14-n-500"
                                style={{
                                    whiteSpace: "nowrap",
                                    overflow: "hidden",
                                    textOverflow: "ellipsis",
                                    maxWidth: "calc(100% - 76px)" // 60px image width + 16px margin
                                }}
                                title={rowData.name} // Show full name on hover
                            >
                                {rowData.name}
                            </span>
                        </Flex>
                    )}
                />
                <Column field="unitName" header="Đơn vị tính" style={{ width: "136px" }} />
                <Column field="price" header="Đơn giá (đ)" body={priceTemplate} style={{ width: "136px", minWidth: "136px" }} align="right" />
                <Column field="quantity" header="Số lượng" style={{ width: "116px", minWidth: "116px" }} align="right" />
                <Column field="total" header="Thành tiền (đ)" style={{ width: "136px", minWidth: "136px" }} align="right" body={totalPriceTemplate} />
            </DataTable>

            <Flex vertical style={{ padding: "16px" }} gap={16}>
                <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                    <div style={{ width: 260 }}>
                        <span className="txt-14-n-400">Cộng thành tiền:</span>
                    </div>
                    <div className="txt-16-n-500" style={{ width: 120, textAlign: "right" }}>
                        {formatPrice(subTotal)} đ
                    </div>
                </Flex>

                <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                    <div style={{ width: 260 }}>
                        <span className="txt-14-n-400">Phí vận chuyển:</span>
                    </div>
                    <div className="txt-16-n-500" style={{ width: 120, textAlign: "right" }}>
                        {formatPrice(shippingFee)} đ
                    </div>
                </Flex>
                {discountCodeValue > 0 && (
                    <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                        <div style={{ width: 260 }}>
                            <span className="txt-14-n-400">Mã giảm giá:</span>
                        </div>
                        <div className="txt-16-n-500" style={{ width: 120, textAlign: "right", color: "#FF3D32" }}>
                            -{formatPrice(discountCodeValue)} ₫
                        </div>
                    </Flex>
                )}
                {membershipDiscount > 0 && (
                    <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                        <div style={{ width: 260 }}>
                            <span className="txt-14-n-400">Hạng thành viên:</span>
                        </div>
                        <div className="txt-16-n-500" style={{ width: 120, textAlign: "right", color: "#FF3D32" }}>
                            -{formatPrice(membershipDiscount)} ₫
                        </div>
                    </Flex>
                )}
                {pointsDiscount > 0 && (
                    <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                        <div style={{ width: 260 }}>
                            <span className="txt-14-n-400">Trừ điểm tích lũy:</span>
                        </div>
                        <div className="txt-16-n-500" style={{ width: 120, textAlign: "right", color: "#FF3D32" }}>
                            -{formatPrice(pointsDiscount)} ₫
                        </div>
                    </Flex>
                )}

                <div style={{ width: "100%", height: 1, background: "rgba(34, 49, 63, 0.1)" }} />

                <Flex align="center" gap={10} style={{ alignSelf: "flex-end" }}>
                    <div style={{ width: 260 }}>
                        <span className="txt-14-n-600">Tổng tiền:</span>
                    </div>
                    <div className="txt-16-n-600" style={{ width: 120, textAlign: "right", color: "#FF3D32" }}>
                        {formatPrice(orderDetails.totalAmount)} đ
                    </div>
                </Flex>
            </Flex>
        </Flex>
    );
};

export default ProductInfoView;
