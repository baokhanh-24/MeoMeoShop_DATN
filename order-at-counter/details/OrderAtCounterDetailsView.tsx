import { useMainLayout } from "@/layouts/MainLayout";
import { Breadcrumb, Button, Row, Col, Flex } from "antd";
import { Container } from "react-bootstrap";
import { Fragment, useState, useRef, useEffect, useCallback } from "react";
import styles from "./OrderAtCounterDetailsView.module.scss";
import { useRouter } from "next/router";
import { GetOrderAtCounterDetailsResponse } from "@/@schema/api/order-at-counter/details";
import { GetOrderAtCounterHistoryRequest, GetOrderAtCounterHistoryResponse } from "@/@schema/api/order-at-counter/history";
import { ApiResponse } from "@/utils/ApiUtils";
import DeliveryInfoView from "./DeliveryInfoView";
import ProductInfoView from "./ProductInfoView";
import { DeleteOrderAtCounterRequest, DeleteOrderAtCounterResponse } from "@/@schema/api/order-at-counter/delete";
import { UpdateStatusOrderAtCounterRequest, UpdateStatusOrderAtCounterResponse } from "@/@schema/api/order-at-counter/update-status";
import { ImportToWarehouseOrderAtCounterRequest, ImportToWarehouseOrderAtCounterResponse } from "@/@schema/api/order-at-counter/import-to-warehouse";
import type { HistoryItem } from "@/components/HistoryDrawer";
import HistoryDrawer from "@/components/HistoryDrawer";
import dayjs from "dayjs";
import { usePermissionStore } from "@/store/permissionStore";
import { OrderReceipt, OrderReceiptProps } from "@/components/order-receipt/OrderReceipt";
import { BankInfo } from "@/views/order-at-counter/add-or-update/View";
import { useReactToPrint } from "react-to-print";

interface OrderAtCounterDetailsViewProps {
    orderDetails: GetOrderAtCounterDetailsResponse;
    pathCDN: string;
    ratePurchase: number;
    bankInfo?: BankInfo;
}

const GeneralInfoView = ({ orderDetails }: { orderDetails: GetOrderAtCounterDetailsResponse }) => {
    const enumType: Record<number, string> = {
        0: "Giao trực tiếp",
        1: "Mua online trên hệ thống",
        2: "Đơn hàng đi giao"
    };

    const enumPaymentType: Record<number, string> = {
        0: "Tiền mặt",
        1: "Chuyển khoản"
    };

    const enumPaymentStatus: Record<number, string> = {
        0: "Chưa thanh toán",
        1: "Đã thanh toán"
    };

    const getStatusClass = (status: number) => {
        switch (status) {
            case 6: // Lưu tạm
                return styles["temp-status"];
            case 1: // Chờ giao hàng
                return styles["pending-status"];
            case 2: // Đang giao hàng
                return styles["delivering-status"];
            case 3: // Hoàn thành
                return styles["approved-status"];
            case 5: // Đã hủy
                return styles["rejected-status"];
            default:
                return styles["temp-status"];
        }
    };

    const getStatusText = (status: number) => {
        switch (status) {
            case 6:
                return "Lưu tạm";
            case 1:
                return "Chờ giao hàng";
            case 2:
                return "Đang giao hàng";
            case 3:
                return "Hoàn thành";
            case 5:
                return "Đã hủy";
            default:
                return "Lưu tạm";
        }
    };

    const getPaymentStatusClass = (paymentStatus: number) => {
        return paymentStatus === 0 ? styles["rejected-status"] : styles["approved-status"];
    };

    return (
        <div style={{ padding: 16, background: "white", borderRadius: 16, width: "100%", marginBottom: 16 }}>
            <Flex vertical gap={16}>
                <Flex justify="space-between" align="center">
                    <div className="txt-18-n-600">Thông tin chung</div>
                    <div className={getStatusClass(orderDetails.status)}>
                        <div className={styles.dot} />
                        <div>{getStatusText(orderDetails.status)}</div>
                    </div>
                </Flex>

                <Row gutter={12}>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 140 }}>
                                Mã đơn hàng:
                            </div>
                            <div className="txt-14-n-500">{orderDetails.orderCode}</div>
                        </Flex>
                    </Col>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 140 }}>
                                Loại:
                            </div>
                            <div className="txt-14-n-500">{enumType[orderDetails.type]}</div>
                        </Flex>
                    </Col>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 200 }}>
                                Phương thức thanh toán:
                            </div>
                            <div className="txt-14-n-500">{enumPaymentType[orderDetails.paymentType]}</div>
                        </Flex>
                    </Col>
                </Row>

                <Row gutter={12}>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 140 }}>
                                Họ tên khách hàng:
                            </div>
                            <div className="txt-14-n-500" style={{ color: "#188A42" }}>
                                {orderDetails.customer?.name}
                            </div>
                        </Flex>
                    </Col>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 140 }}>
                                Thời gian đặt hàng:
                            </div>
                            <div className="txt-14-n-500">{dayjs(orderDetails.orderTime).format("HH:mm DD/MM/YYYY")}</div>
                        </Flex>
                    </Col>
                    <Col span={8}>
                        <Flex align="center" gap={12}>
                            <div className="txt-14-n-400" style={{ width: 200 }}>
                                Trạng thái thanh toán
                            </div>
                            <div className={getPaymentStatusClass(orderDetails.paymentStatus)}>{enumPaymentStatus[orderDetails.paymentStatus]}</div>
                        </Flex>
                    </Col>
                </Row>
            </Flex>
        </div>
    );
};

export const OrderAtCounterDetailsView = ({ orderDetails, pathCDN, bankInfo, ratePurchase }: OrderAtCounterDetailsViewProps) => {
    const { setSpinning, showMessage, showConfirmModal, showReasonConfirmModal } = useMainLayout();
    const router = useRouter();
    const { isLoading, hasPermission } = usePermissionStore();
    useEffect(() => {
        if (!isLoading && !hasPermission("ORDER:POS_ORDER:VIEW")) {
            router.push("/");
        }
    }, [hasPermission]);
    const [isHistoryDrawerOpen, setIsHistoryDrawerOpen] = useState(false);
    const [histories, setHistories] = useState<HistoryItem[]>([]);
    const [isHistoryLoading, setIsHistoryLoading] = useState(false);
    const receiptRef = useRef<HTMLDivElement>(null);
    const [orderReceipt, setOrderReceipt] = useState<OrderReceiptProps | null>(null);
    const [receiptHeight, setReceiptHeight] = useState<number>(0);

    useEffect(() => {
        const updateHeight = () => {
            if (receiptRef.current) {
                const height = receiptRef.current.clientHeight;
                if (height > 0) {
                    setReceiptHeight(height);
                }
            }
        };

        // Create ResizeObserver to monitor size changes
        const resizeObserver = new ResizeObserver(updateHeight);

        if (receiptRef.current) {
            resizeObserver.observe(receiptRef.current);
            // Call immediately after observing
            updateHeight();
        }

        // Also keep the window resize listener for safety
        window.addEventListener("resize", updateHeight);

        // Call updateHeight after a short delay to ensure content is rendered
        const timeoutId = setTimeout(updateHeight, 100);

        return () => {
            window.removeEventListener("resize", updateHeight);
            resizeObserver.disconnect();
            clearTimeout(timeoutId);
        };
    }, [receiptRef, orderReceipt]);

    const handleGoBack = () => {
        router.back();
    };

    const handleEdit = () => {
        router.push(`/order-at-counter/update/${orderDetails.id}`);
    };

    const handleDelete = async () => {
        showConfirmModal({
            title: `Xóa đơn hàng`,
            content: `Bạn có chắc chắn muốn xoá đơn hàng <span>${orderDetails.orderCode}</span> không?`,
            onConfirmAsync: async () => {
                setSpinning(true);
                const deleteRequest: DeleteOrderAtCounterRequest = {
                    id: orderDetails.id
                };

                const response = await fetch("/api/order-at-counter/delete", {
                    method: "DELETE",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(deleteRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<DeleteOrderAtCounterResponse> = await response.json();
                    if (result.status == 1) {
                        showMessage("success", "Xóa đơn hàng thành công");
                        router.back();
                    } else {
                        showMessage("error", result.message || "Xóa đơn hàng không thành công");
                    }
                } else {
                    showMessage("error", "Xóa đơn hàng không thành công");
                }
                setSpinning(false);
            }
        });
    };

    const handleDelivery = async () => {
        showConfirmModal({
            title: `Xác nhận gửi giao hàng`,
            content: `Bạn có chắc chắn muốn xác nhận đã gửi đơn hàng <span>${orderDetails.orderCode}</span> cho đơn vị vận chuyển không?`,
            onConfirmAsync: async () => {
                setSpinning(true);
                const updateRequest: UpdateStatusOrderAtCounterRequest = {
                    ids: [orderDetails.id],
                    newStatus: 2
                };

                const response = await fetch("/api/order-at-counter/update-status", {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(updateRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<UpdateStatusOrderAtCounterResponse> = await response.json();
                    if (result.status == 1) {
                        showMessage("success", "Gửi giao hàng thành công");
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showMessage("error", result.message || "Gửi giao hàng không thành công");
                    }
                } else {
                    showMessage("error", "Gửi giao hàng không thành công");
                }
                setSpinning(false);
            }
        });
    };

    const handleCancel = async () => {
        showReasonConfirmModal({
            title: `Hủy đơn hàng`,
            content: `Bạn có chắc chắn muốn hủy đơn hàng <span>${orderDetails.orderCode}</span> không?`,
            onConfirmAsync: async (reason) => {
                setSpinning(true);
                const updateRequest: UpdateStatusOrderAtCounterRequest = {
                    ids: [orderDetails.id],
                    newStatus: 5
                };

                const response = await fetch("/api/order-at-counter/update-status", {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(updateRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<UpdateStatusOrderAtCounterResponse> = await response.json();
                    if (result.status == 1) {
                        showMessage("success", "Hủy đơn hàng thành công");
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showMessage("error", result.message || "Hủy đơn hàng không thành công");
                    }
                } else {
                    showMessage("error", "Hủy đơn hàng không thành công");
                }
                setSpinning(false);
            }
        });
    };

    const handleComplete = async () => {
        showConfirmModal({
            title: `Xác nhận hoàn thành`,
            content: `Bạn có chắc chắn muốn xác nhận đơn hàng <span>${orderDetails.orderCode}</span> đã hoàn thành không?`,
            onConfirmAsync: async () => {
                setSpinning(true);
                const updateRequest: UpdateStatusOrderAtCounterRequest = {
                    ids: [orderDetails.id],
                    newStatus: 3
                };

                const response = await fetch("/api/order-at-counter/update-status", {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(updateRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<UpdateStatusOrderAtCounterResponse> = await response.json();
                    if (result.status == 1) {
                        showMessage("success", "Hoàn thành đơn hàng thành công");
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showMessage("error", result.message || "Hoàn thành đơn hàng không thành công");
                    }
                } else {
                    showMessage("error", "Hoàn thành đơn hàng không thành công");
                }
                setSpinning(false);
            }
        });
    };

    const handleImportToWarehouse = async () => {
        showConfirmModal({
            title: "Xác nhận nhập kho",
            content: `Bạn có chắc chắn muốn nhập kho đơn hàng <span>${orderDetails.orderCode}</span> không?`,
            onConfirmAsync: async () => {
                setSpinning(true);
                const importRequest: ImportToWarehouseOrderAtCounterRequest = {
                    ids: [orderDetails.id]
                };

                const response = await fetch("/api/order-at-counter/import-to-warehouse", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(importRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<ImportToWarehouseOrderAtCounterResponse> = await response.json();
                    if (result.status == 1) {
                        showMessage("success", "Nhập kho thành công");
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showMessage("error", result.message || "Nhập kho không thành công");
                    }
                } else {
                    showMessage("error", "Nhập kho không thành công");
                }
                setSpinning(false);
            }
        });
    };

    const handleViewHistory = async () => {
        setIsHistoryLoading(true);
        setIsHistoryDrawerOpen(true);

        const historyRequest: GetOrderAtCounterHistoryRequest = {
            orderId: orderDetails.id
        };

        const response = await fetch("/api/order-at-counter/history", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(historyRequest)
        });

        if (response.ok || response.status === 400) {
            const result: ApiResponse<GetOrderAtCounterHistoryResponse> = await response.json();
            if (result.status === 1 && result.data) {
                setHistories(
                    result.data.histories.map((history) => {
                        return {
                            icon: `/images/actions/${getHistoryIcon(history.type)}.svg`,
                            title: history.action,
                            note: history.note,
                            time: history.actionTime,
                            userName: history.userName,
                            userEmail: history.userEmail
                        };
                    })
                );
            } else {
                showMessage("error", result.message || "Không thể lấy lịch sử đơn hàng");
            }
        } else {
            showMessage("error", "Không thể lấy lịch sử đơn hàng");
        }
        setIsHistoryLoading(false);
    };

    const getHistoryIcon = (type: number): string => {
        switch (type) {
            case 0:
                return "local-mall-action";
            case 3:
                return "his-5-reject";
            case 5:
                return "close";
            case 6:
                return "delivery_truck_speed_orange";
            case 7:
                return "chalet-action";
            case 8:
                return "his-4-approve";
            default:
                return "his_change";
        }
    };

    const blobToBase64 = (blob: Blob): Promise<string> => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result as string);
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    };

    const getBillQrBase64Image = useCallback(async (bankInfo: BankInfo, orderCode: string, total: number) => {
        const url = `https://img.vietqr.io/image/${bankInfo.bin}-${bankInfo.accountNumber}-qr_only.png?amount=${total}&addInfo=Thanh toán đơn hàng ${orderCode}`;
        const response = await fetch(url);
        const blob = await response.blob();
        const base64 = await blobToBase64(blob);
        return base64;
    }, []);

    useEffect(() => {
        const fetchOrderReceipt = async () => {
            let pointReceive = 0;
            if (orderDetails.status === 3) {
                pointReceive = orderDetails.pointReceive || 0;
            } else {
                pointReceive = Math.round(((orderDetails.totalAmount - Number(orderDetails.shippingFee)) * ratePurchase) / 100);
            }

            let qrBase64Image = null;
            if (bankInfo && orderDetails.totalAmount > 0) {
                qrBase64Image = await getBillQrBase64Image(bankInfo, orderDetails.orderCode || "", orderDetails.totalAmount);
            }
            setOrderReceipt({
                orderCode: orderDetails.orderCode,
                orderTime: dayjs(orderDetails.orderTime),
                customerName: orderDetails.customer?.name,
                customerPhone: orderDetails.customer?.phone,
                products: orderDetails.products.map((p) => ({
                    id: `${p.productId}-${p.comboId}`,
                    sku: p.sku || "",
                    name: p.productName,
                    price: p.price,
                    quantity: p.quantity,
                    unitName: p.unitName || "",
                    discountPercent: p.discountValue || 0,
                    promotedQuantity: p.promotedQuantity || 0,
                    nonPromotedQuantity: p.nonPromotedQuantity || 0
                })),
                shippingFee: Number(orderDetails.shippingFee),
                minusPoints: orderDetails.minusPoints || 0,
                minusPointsValue: orderDetails.minusPointsValue || 0,
                membershipValue: orderDetails.minusMembership || 0,
                discountValue: orderDetails.discountValue || 0,
                pointReceive: orderDetails.pointReceive || 0,
                total: orderDetails.totalAmount || 0,
                consigneeAddress: orderDetails.delivery.consigneeFullAddress || undefined,
                userName: orderDetails.createdBy || "",
                qrBase64Image: qrBase64Image || undefined
            });
        };
        fetchOrderReceipt();
    }, [orderDetails, bankInfo, ratePurchase, getBillQrBase64Image]);

    const handlePrint = useReactToPrint({
        content: () => receiptRef.current,
        pageStyle: () => {
            return `
                @page {
                    size: 228px ${receiptHeight}px;
                    margin: 0;
                }
                @media print {
                    body {
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }
                    html, body {
                        width: 100%;
                        height: 100%;
                    }
                }
            `;
        },
        onAfterPrint: () => {
            // showMessage("success", "Đã in hóa đơn thành công");
        },
        onPrintError: () => {
            showMessage("error", "Lỗi khi in hóa đơn");
        }
    });

    const handlePrintOrder = async () => {
        if (!orderReceipt) {
            showMessage("error", "Không thể in hóa đơn");
            return;
        }
        handlePrint?.();
    };

    return (
        <Fragment>
            <div className="content-background">
                <Container fluid>
                    <Breadcrumb
                        className={styles["bread-crumb"]}
                        items={[
                            {
                                title: "Quản lý đơn hàng"
                            },
                            {
                                title: <span className="current">Chi tiết đơn hàng</span>
                            }
                        ]}
                    />
                    <div className="group-breadcumb">
                        {orderDetails.status === 6 && (
                            <>
                                {hasPermission("ORDER:POS_ORDER:EDIT") && (
                                    <Button className="btn-reset" icon={<img src="/images/actions/stylus.svg" />} onClick={handleEdit}>
                                        Sửa
                                    </Button>
                                )}
                                {hasPermission("ORDER:POS_ORDER:DEL") && (
                                    <Button className="btn-deny-outline" icon={<img src="/images/actions/close.svg" />} onClick={handleDelete}>
                                        Xóa
                                    </Button>
                                )}
                            </>
                        )}
                        {orderDetails.status === 1 && (
                            <>
                                {hasPermission("ORDER:POS_ORDER:CONFIRM_DELIVERY") && (
                                    <Button
                                        className="btn-send"
                                        icon={<img src="/images/actions/delivery_truck_speed.svg" />}
                                        onClick={handleDelivery}
                                    >
                                        Giao hàng
                                    </Button>
                                )}
                                {hasPermission("ORDER:POS_ORDER:CANCEL") && (
                                    <Button className="btn-deny" icon={<img src="/images/actions/cancel_order.svg" />} onClick={handleCancel}>
                                        Hủy đơn
                                    </Button>
                                )}
                            </>
                        )}
                        {orderDetails.status === 2 && (
                            <>
                                {hasPermission("ORDER:POS_ORDER:CONFIRM_COMPLETE") && (
                                    <Button
                                        className="btn-create"
                                        icon={<img src="/images/actions/assignment_turned_in.svg" />}
                                        onClick={handleComplete}
                                    >
                                        Hoàn thành
                                    </Button>
                                )}
                                {hasPermission("ORDER:POS_ORDER:CANCEL") && (
                                    <Button className="btn-deny" icon={<img src="/images/actions/cancel_order.svg" />} onClick={handleCancel}>
                                        Hủy đơn
                                    </Button>
                                )}
                            </>
                        )}
                        {orderDetails.status === 5 && orderDetails.imported === 0 && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE") && (
                            <Button className="btn-reset" icon={<img src="/images/actions/chalet.svg" />} onClick={handleImportToWarehouse}>
                                Nhập kho
                            </Button>
                        )}
                        {hasPermission("ORDER:POS_ORDER:PRINT") && (
                            <Button className="btn-reset" icon={<img src="/images/actions/print_connect.svg" />} onClick={handlePrintOrder}>
                                In đơn
                            </Button>
                        )}
                        {hasPermission("ORDER:POS_ORDER:VIEW_HISTORY") && (
                            <Button className="btn-reset" icon={<img src="/images/actions/search_activity.svg" />} onClick={handleViewHistory}>
                                Lịch sử
                            </Button>
                        )}
                        <Button className="btn-back" icon={<img src="/images/actions/icon-back.svg" />} onClick={handleGoBack}>
                            Quay lại
                        </Button>
                    </div>
                    <GeneralInfoView orderDetails={orderDetails} />
                    <span className="txt-18-n-500">Thông tin sản phẩm</span>
                    <Row gutter={16} style={{ marginTop: 8 }}>
                        <Col span={8}>
                            <DeliveryInfoView
                                customerName={orderDetails.delivery.consigneeName || ""}
                                phoneNumber={orderDetails.delivery.consigneePhone || ""}
                                address={orderDetails.delivery.consigneeFullAddress || ""}
                                note={orderDetails.delivery.consigneeNote || ""}
                            />
                        </Col>
                        <Col span={16}>
                            <ProductInfoView orderDetails={orderDetails} pathCDN={pathCDN} hasPermission={hasPermission} />
                        </Col>
                    </Row>
                </Container>
            </div>
            <HistoryDrawer
                open={isHistoryDrawerOpen}
                onClose={() => setIsHistoryDrawerOpen(false)}
                title="Lịch sử đơn hàng tại quầy"
                items={histories}
                loading={isHistoryLoading}
            />
            {orderReceipt && (
                <div style={{ display: "block", position: "fixed", left: "-9999px" }}>
                    {/* <div style={{ display: "block", position: "fixed", left: "400px", top: "0px", zIndex: 10000 }}> */}
                    <div ref={receiptRef}>
                        <OrderReceipt
                            orderCode={orderReceipt.orderCode}
                            orderTime={orderReceipt.orderTime}
                            customerName={orderReceipt.customerName}
                            customerPhone={orderReceipt.customerPhone}
                            products={orderReceipt.products}
                            shippingFee={orderReceipt.shippingFee}
                            minusPoints={orderReceipt.minusPoints}
                            minusPointsValue={orderReceipt.minusPointsValue}
                            membershipValue={orderReceipt.membershipValue}
                            discountValue={orderReceipt.discountValue}
                            pointReceive={orderReceipt.pointReceive}
                            total={orderReceipt.total}
                            consigneeAddress={orderReceipt.consigneeAddress}
                            userName={orderReceipt.userName}
                            qrBase64Image={orderReceipt.qrBase64Image}
                        />
                    </div>
                </div>
            )}
        </Fragment>
    );
};
