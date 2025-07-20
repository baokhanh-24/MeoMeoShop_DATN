import { useMainLayout } from "@/layouts/MainLayout";
import { Breadcrumb, Button, Tabs, TabsProps, Tooltip } from "antd";
import { useRouter } from "next/router";
import { Fragment, useState, useEffect } from "react";
import { Container } from "react-bootstrap";
import styles from "./OrderAtCounterListView.module.scss";
import { OrderAtCounterItem, ListOrderAtCounterResponse, ListOrderAtCounterRequest } from "@/@schema/api/order-at-counter/list";
import { OrderAtCounterListCountResponse, OrderAtCounterListCountRequest } from "@/@schema/api/order-at-counter/list-count";
import { GetOrderAtCounterHistoryRequest, GetOrderAtCounterHistoryResponse } from "@/@schema/api/order-at-counter/history";
import { ApiResponse } from "@/utils/ApiUtils";
import { DataTable, DataTableStateEvent } from "primereact/datatable";
import { Column } from "primereact/column";
import StringFilterInput from "@/components/filterInput/string";
import EnumFilterInput from "@/components/filterInput/enum";
import DateFilterInput from "@/components/filterInput/date";
import NumberFilterInput from "@/components/filterInput/number";
import dayjs from "dayjs";
import { DeleteOrderAtCounterRequest, DeleteOrderAtCounterResponse } from "@/@schema/api/order-at-counter/delete";
import { UpdateStatusOrderAtCounterResponse, UpdateStatusOrderAtCounterRequest } from "@/@schema/api/order-at-counter/update-status";
import { ImportToWarehouseOrderAtCounterRequest, ImportToWarehouseOrderAtCounterResponse } from "@/@schema/api/order-at-counter/import-to-warehouse";
import type { HistoryItem } from "@/components/HistoryDrawer";
import HistoryDrawer from "@/components/HistoryDrawer";
import ExcelJS from "exceljs";
import { saveAs } from "file-saver";
import { usePermissionStore } from "@/store/permissionStore";
import { ConfirmPaymentOrderAtCounterRequest, ConfirmPaymentOrderAtCounterResponse } from "@/@schema/api/order-at-counter/confirm-payment";
import { PlusOutlined } from "@ant-design/icons";

export const OrderAtCounterListView = () => {
    const { setSpinning, showConfirmModal, showMessage, showReasonConfirmModal } = useMainLayout();
    const router = useRouter();
    const { hasPermission } = usePermissionStore();
    const enumStatus: Record<number, string> = {
        // 0: "Chờ xác nhận",
        1: "Chờ giao hàng",
        2: "Đang giao hàng",
        3: "Hoàn thành",
        // 4: "Chờ hủy",
        5: "Đã hủy",
        6: "Lưu tạm"
    };
    const enumStatusPay: Record<number, string> = {
        0: "Chưa thanh toán",
        1: "Đã thanh toán"
    };
    const [loading, setLoading] = useState(false); // Loading state

    const [currentTab, setCurrentTab] = useState<string>("-1");
    const [listOrder, setListOrder] = useState<ListOrderAtCounterResponse>();
    const [count, setCount] = useState<OrderAtCounterListCountResponse>();
    const [selectedOrder, setSelectedOrder] = useState([]);
    const [request, setRequest] = useState<ListOrderAtCounterRequest>({
        pageIndex: 0,
        pageSize: 10,
        filter: {
            orderCode: undefined,
            customerName: undefined,
            customerPhoneNumber: undefined,
            orderTime: undefined,
            totalAmount: undefined,
            createdBy: undefined,
            paymentStatus: undefined,
            status: undefined
        }
    });

    const [isHistoryDrawerOpen, setIsHistoryDrawerOpen] = useState(false);
    const [histories, setHistories] = useState<HistoryItem[]>([]);
    const [isHistoryLoading, setIsHistoryLoading] = useState(false);

    const isFilterRequest = (): boolean => {
        return (
            request.filter?.orderCode != undefined ||
            request.filter?.customerName != undefined ||
            request.filter?.customerPhoneNumber != undefined ||
            request.filter?.orderTime != undefined ||
            request.filter?.totalAmount != undefined ||
            request.filter?.createdBy != undefined ||
            request.filter?.paymentStatus != undefined ||
            (request.filter?.status != undefined && currentTab == "-1")
        );
    };

    const fetchCount = async () => {
        try {
            const response = await fetch("/api/order-at-counter/list-count", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (response.ok || response.status === 400) {
                const result: ApiResponse<OrderAtCounterListCountResponse> = await response.json();
                if (result.status === 1) {
                    setCount(result.data);
                } else {
                    showMessage("error", result.message || "Không thể lấy số lượng đơn hàng");
                }
            } else {
                showMessage("error", "Không thể lấy số lượng đơn hàng");
            }
        } catch (error) {
            console.error("Error fetching count:", error);
            showMessage("error", "Không thể lấy số lượng đơn hàng");
        } finally {
        }
    };

    const fetchOrders = async () => {
        try {
            setLoading(true);
            const response = await fetch("/api/order-at-counter/list", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request)
            });

            if (response.ok || response.status === 400) {
                const result: ApiResponse<ListOrderAtCounterResponse> = await response.json();
                if (result.status === 1) {
                    setListOrder(result.data);
                } else {
                    showMessage("error", result.message || "Không thể lấy danh sách đơn hàng");
                }
            } else {
                showMessage("error", "Không thể lấy danh sách đơn hàng");
            }
        } catch (error) {
            console.error("Error fetching orders:", error);
            showMessage("error", "Không thể lấy danh sách đơn hàng");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCount();
        fetchOrders();
    }, [request]);
    const itemsTab: TabsProps["items"] = [
        {
            key: "-1",
            label: "Tất cả (" + (count?.totalStatus || 0) + ")"
        },
        {
            key: "1",
            label: "Chờ giao hàng (" + (count?.totalStatus1 || 0) + ")"
        },
        {
            key: "2",
            label: "Đang giao hàng (" + (count?.totalStatus2 || 0) + ")"
        },
        {
            key: "3",
            label: "Hoàn thành (" + (count?.totalStatus3 || 0) + ")"
        },
        {
            key: "5",
            label: "Đã hủy (" + (count?.totalStatus5 || 0) + ")"
        }
    ];

    const handleTabChange = (key: string) => {
        if (loading) return;
        setCurrentTab(key);
        setSelectedOrders([]);
        setRequest((prev: any) => ({ ...prev, pageIndex: 0, filter: { ...prev.filter, status: parseInt(key) == -1 ? undefined : parseInt(key) } }));
    };

    const getActionColumnBody = (rowData: OrderAtCounterItem) => {
        return (
            <>
                {rowData.status == 6 && (
                    <>
                        {hasPermission("ORDER:POS_ORDER:EDIT") && (
                            <Tooltip title="Sửa">
                                <img
                                    className="mx-1"
                                    src="/images/actions/update-action.svg"
                                    alt=""
                                    onClick={(e) => handleActionClick(e, 1, rowData)}
                                />
                            </Tooltip>
                        )}
                        {hasPermission("ORDER:POS_ORDER:DEL") && (
                            <Tooltip title="Xóa">
                                <img
                                    className="mx-1"
                                    src="/images/actions/delete-action.svg"
                                    alt=""
                                    onClick={(e) => handleActionClick(e, 2, rowData)}
                                />
                            </Tooltip>
                        )}
                    </>
                )}
                {rowData.status == 1 && hasPermission("ORDER:POS_ORDER:CONFIRM_DELIVERY") && (
                    <Tooltip title="Xác nhận gửi giao hàng">
                        <img className="mx-1" src="/images/actions/icon-ship.svg" alt="" onClick={(e) => handleActionClick(e, 3, rowData)} />
                    </Tooltip>
                )}
                {rowData.status == 2 && hasPermission("ORDER:POS_ORDER:CONFIRM_COMPLETE") && (
                    <Tooltip title="Xác nhận hoàn thành">
                        <img className="mx-1" src="/images/actions/icon-checked.svg" alt="" onClick={(e) => handleActionClick(e, 4, rowData)} />
                    </Tooltip>
                )}
                {[1, 2].includes(rowData.status) && hasPermission("ORDER:POS_ORDER:CANCEL") && (
                    <Tooltip title="Hủy đơn hàng">
                        <img className="mx-1" src="/images/actions/cancel-action.svg" alt="" onClick={(e) => handleActionClick(e, 5, rowData)} />
                    </Tooltip>
                )}
                {rowData.status == 5 && rowData.imported === 0 && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE") && (
                    <Tooltip title="Xác nhận nhập kho">
                        <img className="mx-1" src="/images/actions/chalet-action.svg" alt="" onClick={(e) => handleActionClick(e, 6, rowData)} />
                    </Tooltip>
                )}
                {rowData.status == 5 && rowData.imported === 1 && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE") && (
                    <Tooltip title="Đã nhập kho">
                        <img className="mx-1" src="/images/actions/chalet_green.svg" alt="" />
                    </Tooltip>
                )}
                {hasPermission("ORDER:POS_ORDER:VIEW_HISTORY") && (
                    <Tooltip title="Lịch sử">
                        <img className="mx-1" src="/images/actions/history-action.svg" alt="" onClick={(e) => handleActionClick(e, 7, rowData)} />
                    </Tooltip>
                )}
            </>
        );
    };

    const getStatusColumnBody = (rowData: OrderAtCounterItem) => {
        let styleName = "temp-status";
        if (rowData.status === 0) {
            styleName = "pending-status";
        } else if (rowData.status === 1) {
            styleName = "announced-status";
        } else if (rowData.status === 2) {
            styleName = "temp-status";
        } else if (rowData.status === 3) {
            styleName = "approved-status";
        } else if (rowData.status === 4) {
            styleName = "cancellation-status";
        } else if (rowData.status === 4) {
            styleName = "rejected-status";
        }
        return <span className={`${styles[styleName]}`}>{enumStatus[rowData.status] || ""}</span>;
    };
    const getStatusPayColumnBody = (rowData: OrderAtCounterItem) => {
        if (rowData.paymentStatus == 0) {
            return (
                <span
                    style={{ cursor: hasPermission("ORDER:POS_ORDER:CONFIRM_PAYMENT") ? "pointer" : "default" }}
                    className={`${styles["rejected-status"]} `}
                    onClick={() => {
                        if (hasPermission("ORDER:POS_ORDER:CONFIRM_PAYMENT")) {
                            showConfirmModal({
                                title: "Xác nhận thanh toán",
                                content: `Bạn có chắc chắn muốn xác nhận đơn hàng <span>${rowData.orderCode}</span> đã thanh toán không?`,
                                onConfirmAsync: async () => {
                                    await confirmOrderPayment(rowData);
                                }
                            });
                        }
                    }}
                >
                    {enumStatusPay[rowData.paymentStatus] || ""}
                </span>
            );
        } else if (rowData.paymentStatus === 1) {
            return <span className={`${styles["approved-status"]}`}>{enumStatusPay[rowData.paymentStatus] || ""}</span>;
        }
    };

    const deleteOrderAtCounter = async (record: OrderAtCounterItem) => {
        try {
            setSpinning(true);
            const deleteRequest: DeleteOrderAtCounterRequest = {
                id: record.id
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
                if (result.status === 1) {
                    showMessage("success", "Xóa đơn hàng thành công");
                    await fetchOrders();
                    await fetchCount();
                } else {
                    showMessage("error", result.message || "Xóa đơn hàng không thành công");
                }
            } else {
                showMessage("error", "Xóa đơn hàng không thành công");
            }
        } catch (error) {
            console.error("Error deleting order:", error);
            showMessage("error", "Xóa đơn hàng không thành công");
        } finally {
            setSpinning(false);
        }
    };

    const updateOrderStatus = async (record: OrderAtCounterItem, newStatus: number, note?: string) => {
        try {
            setSpinning(true);
            const updateRequest: UpdateStatusOrderAtCounterRequest = {
                ids: [record.id],
                newStatus: newStatus,
                note: note
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
                if (result.status === 1) {
                    let successMessage = "";
                    switch (newStatus) {
                        case 2:
                            successMessage = "Gửi giao hàng thành công";
                            break;
                        case 3:
                            successMessage = "Hoàn thành đơn hàng thành công";
                            break;
                        case 5:
                            successMessage = "Hủy đơn hàng thành công";
                            break;
                        default:
                            successMessage = "Thay đổi trạng thái đơn hàng thành công";
                    }
                    showMessage("success", successMessage);
                    await fetchOrders();
                    await fetchCount();
                } else {
                    showMessage("error", result.message || "Thay đổi trạng thái đơn hàng không thành công");
                }
            } else {
                showMessage("error", "Thay đổi trạng thái đơn hàng không thành công");
            }
        } catch (error) {
            console.error("Error updating order status:", error);
            showMessage("error", "Thay đổi trạng thái đơn hàng không thành công");
        } finally {
            setSpinning(false);
        }
    };

    const updateMultipleOrderStatus = async (orders: OrderAtCounterItem[], newStatus: number) => {
        try {
            setSpinning(true);
            const updateRequest: UpdateStatusOrderAtCounterRequest = {
                ids: orders.map((order) => order.id),
                newStatus: newStatus
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
                if (result.status === 1) {
                    let successMessage = "";
                    switch (newStatus) {
                        case 2:
                            successMessage = "Gửi giao hàng thành công";
                            break;
                        case 3:
                            successMessage = "Hoàn thành đơn hàng thành công";
                            break;
                        default:
                            successMessage = "Thay đổi trạng thái đơn hàng thành công";
                    }
                    showMessage("success", successMessage);
                    setSelectedOrders([]);
                    await fetchOrders();
                    await fetchCount();
                } else {
                    showMessage("error", result.message || "Thay đổi trạng thái đơn hàng không thành công");
                }
            } else {
                showMessage("error", "Thay đổi trạng thái đơn hàng không thành công");
            }
        } catch (error) {
            console.error("Error updating multiple order status:", error);
            showMessage("error", "Thay đổi trạng thái đơn hàng không thành công");
        } finally {
            setSpinning(false);
        }
    };

    const importToWarehouse = async (orders: OrderAtCounterItem[]) => {
        try {
            setSpinning(true);
            const importRequest: ImportToWarehouseOrderAtCounterRequest = {
                ids: orders.map((order) => order.id)
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
                if (result.status === 1) {
                    showMessage("success", "Nhập kho thành công");
                    await fetchOrders();
                    await fetchCount();
                    if (orders === selectedOrders) {
                        setSelectedOrders([]);
                    }
                } else {
                    showMessage("error", result.message || "Nhập kho không thành công");
                }
            } else {
                showMessage("error", "Nhập kho không thành công");
            }
        } catch (error) {
            console.error("Error importing to warehouse:", error);
            showMessage("error", "Nhập kho không thành công");
        } finally {
            setSpinning(false);
        }
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

    const handleViewHistory = async (record: OrderAtCounterItem) => {
        try {
            setIsHistoryLoading(true);
            setIsHistoryDrawerOpen(true);

            const historyRequest: GetOrderAtCounterHistoryRequest = {
                orderId: record.id
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
                        result.data.histories.map((history) => ({
                            icon: `/images/actions/${getHistoryIcon(history.type)}.svg`,
                            title: history.action,
                            note: history.note,
                            time: history.actionTime,
                            userName: history.userName,
                            userEmail: history.userEmail
                        }))
                    );
                } else {
                    showMessage("error", result.message || "Không thể lấy lịch sử đơn hàng");
                }
            } else {
                showMessage("error", "Không thể lấy lịch sử đơn hàng");
            }
        } catch (error) {
            console.error("Error fetching history:", error);
            showMessage("error", "Không thể lấy lịch sử đơn hàng");
        } finally {
            setIsHistoryLoading(false);
        }
    };

    const handleActionClick = (e: any, action: number, record: OrderAtCounterItem) => {
        e.stopPropagation();
        switch (action) {
            case 1:
                router.push(`/order-at-counter/update/${record.id}`);
                break;
            case 2:
                showConfirmModal({
                    title: `Xóa đơn hàng`,
                    content: `Bạn có chắc chắn muốn xoá đơn hàng <span>${record.orderCode}</span> không?`,
                    onConfirmAsync: async () => {
                        await deleteOrderAtCounter(record);
                    }
                });
                break;
            case 3:
                if (record.status === 1) {
                    showConfirmModal({
                        title: `Xác nhận gửi giao hàng`,
                        content: `Bạn có chắc chắn muốn xác nhận đã gửi đơn hàng <span>${record.orderCode}</span> cho đơn vị vận chuyển không?`,
                        onConfirmAsync: async () => {
                            await updateOrderStatus(record, 2);
                        }
                    });
                }
                break;
            case 4:
                if (record.status === 2) {
                    showConfirmModal({
                        title: `Xác nhận đơn hàng hoàn thành`,
                        content: `Bạn có chắc chắn muốn xác nhận đơn hàng <span>${record.orderCode}</span> đã hoàn thành không?`,
                        onConfirmAsync: async () => {
                            await updateOrderStatus(record, 3);
                        }
                    });
                }
                break;
            case 5:
                if ([1, 2].includes(record.status)) {
                    showReasonConfirmModal({
                        title: `Hủy đơn hàng`,
                        content: `Bạn có chắc chắn muốn hủy đơn hàng <span>${record.orderCode}</span> không?`,
                        onConfirmAsync: async (reason) => {
                            // Even though we're not using the reason parameter in the API call yet
                            await updateOrderStatus(record, 5, reason);
                        }
                    });
                }
                break;
            case 6:
                if (record.status === 5) {
                    showConfirmModal({
                        title: "Xác nhận nhập kho",
                        content: `Bạn có chắc chắn muốn nhập kho đơn hàng <span>${record.orderCode}</span> không?`,
                        onConfirmAsync: async () => {
                            await importToWarehouse([record]);
                        }
                    });
                }
                break;
            case 7:
                handleViewHistory(record);
                break;
        }
    };

    const [selectedOrders, setSelectedOrders] = useState<OrderAtCounterItem[]>([]);

    const getOrderCodeColumnBody = (rowData: OrderAtCounterItem) => {
        return (
            <span style={{ cursor: "pointer", color: "#ae510f", fontWeight: 600 }}>
                <span onClick={() => router.push(`/order-at-counter/details/${rowData.id}`)}>{rowData.orderCode}</span>
            </span>
        );
    };

    const exportOrdersToExcel = async () => {
        try {
            setSpinning(true);
            let allOrders: OrderAtCounterItem[] = [];
            let currentPage = 0;
            const pageSize = 1000;
            let hasMore = true;

            // Keep current filters but reset pagination
            const exportRequest: ListOrderAtCounterRequest = {
                ...request,
                pageIndex: 0,
                pageSize: pageSize,
                filter: {
                    ...request.filter
                }
            };

            // Fetch all orders
            while (hasMore) {
                const response = await fetch("/api/order-at-counter/list", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(exportRequest)
                });

                if (response.ok || response.status === 400) {
                    const result: ApiResponse<ListOrderAtCounterResponse> = await response.json();
                    if (result.status === 1 && result.data?.items) {
                        allOrders = [...allOrders, ...result.data.items];

                        // Check if we have more pages
                        if (result.data.items.length < pageSize) {
                            hasMore = false;
                        } else {
                            currentPage++;
                            exportRequest.pageIndex = currentPage;
                        }
                    } else {
                        showMessage("error", result.message || "Không thể lấy danh sách đơn hàng để xuất");
                        return;
                    }
                } else {
                    showMessage("error", "Không thể lấy danh sách đơn hàng để xuất");
                    return;
                }
            }

            // Create Excel workbook
            const workbook = new ExcelJS.Workbook();
            const worksheet = workbook.addWorksheet("DS Don Hang");

            // Set column widths and properties
            worksheet.columns = [
                { key: "stt", width: 10 },
                { key: "orderCode", width: 20 },
                { key: "customerName", width: 30 },
                { key: "customerPhoneNumber", width: 20 },
                { key: "orderTime", width: 25 },
                { key: "totalAmount", width: 20 },
                { key: "createdBy", width: 20 },
                { key: "paymentStatus", width: 25 },
                { key: "status", width: 20 }
            ];

            // Add title row
            worksheet.mergeCells("A1:I1");
            const titleCell = worksheet.getCell("A1");
            titleCell.value = "DANH SÁCH ĐƠN HÀNG TẠI QUẦY";
            titleCell.font = { bold: true, size: 16 };
            titleCell.alignment = { horizontal: "center", vertical: "middle" };

            // Style header row
            const headerRow = worksheet.getRow(2);
            headerRow.font = { bold: true, size: 13 };
            headerRow.alignment = { horizontal: "center", vertical: "middle" };
            headerRow.height = 30;
            headerRow.values = [
                "STT",
                "Mã đơn hàng",
                "Khách hàng",
                "Số điện thoại",
                "Thời gian đặt hàng",
                "Tổng tiền",
                "Người tạo",
                "Trạng thái thanh toán",
                "Trạng thái"
            ];

            // Add data
            allOrders.forEach((order, index) => {
                worksheet.addRow({
                    stt: index + 1,
                    orderCode: order.orderCode,
                    customerName: order.customerName,
                    customerPhoneNumber: order.customerPhoneNumber,
                    orderTime: order.orderTime,
                    totalAmount: order.totalAmount.toLocaleString("vi-VN"),
                    createdBy: order.createdBy,
                    paymentStatus: enumStatusPay[order.paymentStatus] || "",
                    status: enumStatus[order.status] || ""
                });
            });

            // Style data rows
            worksheet.eachRow((row, rowNumber) => {
                if (rowNumber > 2) {
                    // Skip title row
                    row.font = { size: 13 };
                    row.alignment = { vertical: "middle" };
                    row.height = 25;

                    // Center specific columns
                    ["A", "D", "E", "H", "I"].forEach((col) => {
                        const cell = row.getCell(col);
                        cell.alignment = { horizontal: "center", vertical: "middle" };
                    });
                    row.getCell("B").alignment = { horizontal: "left", vertical: "middle" };
                    row.getCell("C").alignment = { horizontal: "left", vertical: "middle" };
                    row.getCell("G").alignment = { horizontal: "left", vertical: "middle" };

                    // Right align amount column
                    row.getCell("F").alignment = { horizontal: "right", vertical: "middle" };
                }
            });

            // Add borders to all cells
            worksheet.eachRow((row, rowNumber) => {
                row.eachCell((cell) => {
                    cell.border = {
                        top: { style: "thin" },
                        left: { style: "thin" },
                        bottom: { style: "thin" },
                        right: { style: "thin" }
                    };
                });
            });

            // Export file
            const buffer = await workbook.xlsx.writeBuffer();
            const fileData = new Blob([buffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            saveAs(fileData, `Danh_sach_Don_Hang_Tai_Quay_${dayjs().format("DDMMYYYY")}.xlsx`);
        } catch (error) {
            console.error("Error exporting orders:", error);
            showMessage("error", "Có lỗi xảy ra khi xuất file");
        } finally {
            setSpinning(false);
        }
    };

    const confirmOrderPayment = async (record: OrderAtCounterItem) => {
        try {
            setSpinning(true);
            const confirmRequest: ConfirmPaymentOrderAtCounterRequest = {
                orderId: record.id
            };

            const response = await fetch("/api/order-at-counter/confirm-payment", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(confirmRequest)
            });

            if (response.ok || response.status === 400) {
                const result: ApiResponse<ConfirmPaymentOrderAtCounterResponse> = await response.json();
                if (result.status === 1) {
                    showMessage("success", "Xác nhận thanh toán thành công");
                    await fetchOrders();
                    await fetchCount();
                } else {
                    showMessage("error", result.message || "Xác nhận thanh toán không thành công");
                }
            } else {
                showMessage("error", "Xác nhận thanh toán không thành công");
            }
        } catch (error) {
            console.error("Error confirming payment:", error);
            showMessage("error", "Xác nhận thanh toán không thành công");
        } finally {
            setSpinning(false);
        }
    };

    return (
        <Fragment>
            <div className="content-background">
                <Container fluid className="">
                    <Breadcrumb
                        className={`${styles["bread-crumb"]}`}
                        items={[
                            {
                                title: "Quản lý đơn hàng"
                            },
                            {
                                title: <a className="current">Đơn hàng tại quầy</a>
                            }
                        ]}
                    />
                    <div className="group-breadcumb">
                        {count && count.totalStatus > 0 && (
                            <>
                                {hasPermission("ORDER:POS_ORDER:ADD") && (
                                    <Button onClick={() => router.push("/order-at-counter/create")} className="btn-create" icon={<PlusOutlined />}>
                                        Thêm mới
                                    </Button>
                                )}
                                {currentTab === "1" && selectedOrders.length > 0 && hasPermission("ORDER:POS_ORDER:CONFIRM_DELIVERY") && (
                                    <Button
                                        className="btn-send"
                                        onClick={() => {
                                            showConfirmModal({
                                                title: "Xác nhận gửi giao hàng",
                                                content: `Bạn có chắc chắn muốn xác nhận đã gửi <span>${selectedOrders.length} đơn hàng</span> cho đơn vị vận chuyển không?`,
                                                onConfirmAsync: async () => {
                                                    await updateMultipleOrderStatus(selectedOrders, 2);
                                                }
                                            });
                                        }}
                                        icon={
                                            <img
                                                src="/images/actions/delivery_truck_speed.svg"
                                                alt="icon"
                                                style={{ width: "24px", height: "24px" }}
                                            />
                                        }
                                    >
                                        Giao hàng
                                    </Button>
                                )}
                                {currentTab === "2" && selectedOrders.length > 0 && hasPermission("ORDER:POS_ORDER:CONFIRM_COMPLETE") && (
                                    <Button
                                        className="btn-create"
                                        onClick={() => {
                                            showConfirmModal({
                                                title: "Xác nhận đơn hàng hoàn thành",
                                                content: `Bạn có chắc chắn muốn xác nhận <span>${selectedOrders.length} đơn hàng</span> đã hoàn thành không?`,
                                                onConfirmAsync: async () => {
                                                    await updateMultipleOrderStatus(selectedOrders, 3);
                                                }
                                            });
                                        }}
                                        icon={
                                            <img
                                                src="/images/actions/assignment_turned_in.svg"
                                                alt="icon"
                                                style={{ width: "24px", height: "24px" }}
                                            />
                                        }
                                    >
                                        Hoàn thành
                                    </Button>
                                )}
                                {currentTab === "5" && selectedOrders.length > 0 && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE") && (
                                    <Button
                                        className="btn-export"
                                        onClick={() => {
                                            showConfirmModal({
                                                title: "Xác nhận nhập kho",
                                                content: `Bạn có chắc chắn muốn nhập kho <span>${selectedOrders.length} đơn hàng</span> không?`,
                                                onConfirmAsync: async () => {
                                                    await importToWarehouse(selectedOrders);
                                                }
                                            });
                                        }}
                                        icon={<img src="/images/actions/chalet.svg" alt="icon" style={{ width: "24px", height: "24px" }} />}
                                    >
                                        Nhập kho
                                    </Button>
                                )}
                                <Button
                                    className="btn-export"
                                    icon={<img src="/images/actions/icon-export.svg" alt="icon" style={{ width: "24px", height: "24px" }} />}
                                    onClick={() => exportOrdersToExcel()}
                                >
                                    Xuất file
                                </Button>
                            </>
                        )}
                    </div>
                    <div className="row">
                        <div className="col-12">
                            {count && count.totalStatus > 0 ? (
                                <>
                                    <div className={`${styles["cover-space"]} box-tab wtab-200 row`}>
                                        <Tabs defaultActiveKey="-1" items={itemsTab} onChange={handleTabChange} activeKey={currentTab} />
                                    </div>
                                    <div className={`${styles["cover-space"]} row mt-2`}>
                                        <DataTable
                                            lazy
                                            value={listOrder?.items}
                                            loading={loading}
                                            emptyMessage={
                                                isFilterRequest() ? (
                                                    <div className="d-flex flex-column align-items-center justify-content-center my-6">
                                                        <img src="/images/search_no_record.svg" alt="" style={{ width: "200px", height: "200px" }} />
                                                        <span className="txt-20-n-600 text-center mt-5">Không có kết quả phù hợp.</span>
                                                        <span className="txt-16-n-400 text-center mt-2">
                                                            Vui lòng kiểm tra lại từ khóa tìm kiếm của bạn
                                                        </span>
                                                    </div>
                                                ) : (
                                                    <></>
                                                )
                                            }
                                            paginator
                                            filterDisplay="row"
                                            paginatorLeft={`Hiển thị ${listOrder?.items?.length || 0} trên tổng số ${listOrder?.total || 0} bản ghi`}
                                            rows={request.pageSize}
                                            totalRecords={listOrder?.total}
                                            rowsPerPageOptions={[10, 20, 50, 100]}
                                            first={request.pageIndex * request.pageSize}
                                            onPage={(e: DataTableStateEvent) => {
                                                setRequest((prev) => ({
                                                    ...prev,
                                                    pageIndex: e.page ?? 0,
                                                    pageSize: e.rows ?? 10
                                                }));
                                            }}
                                            selectionMode={
                                                (currentTab === "1" && hasPermission("ORDER:POS_ORDER:CONFIRM_DELIVERY")) ||
                                                (currentTab === "2" && hasPermission("ORDER:POS_ORDER:CONFIRM_COMPLETE")) ||
                                                (currentTab === "5" && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE"))
                                                    ? "checkbox"
                                                    : null
                                            }
                                            selection={selectedOrders}
                                            onSelectionChange={(e: any) => setSelectedOrders(e.value)}
                                            dataKey="id"
                                            isDataSelectable={(e: any) => {
                                                if (currentTab === "5") {
                                                    return e.data.imported === 0;
                                                }
                                                return true;
                                            }}
                                        >
                                            {((currentTab === "1" && hasPermission("ORDER:POS_ORDER:CONFIRM_DELIVERY")) ||
                                                (currentTab === "2" && hasPermission("ORDER:POS_ORDER:CONFIRM_COMPLETE")) ||
                                                (currentTab === "5" && hasPermission("ORDER:POS_ORDER:IMPORT_WAREHOUSE"))) && (
                                                <Column selectionMode="multiple" headerStyle={{ width: "30px" }}></Column>
                                            )}
                                            <Column header="STT" body={(_, { rowIndex }) => rowIndex + 1} style={{ width: "46px" }} align="center" />

                                            <Column
                                                header="Mã đơn hàng"
                                                field="orderCode"
                                                body={getOrderCodeColumnBody}
                                                filter
                                                filterElement={
                                                    <StringFilterInput
                                                        placeholder="Nhập..."
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    orderCode: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{
                                                            height: "36px",
                                                            lineHeight: "36px", // Ensures text aligns with the height
                                                            padding: "0 11px" // Adjust horizontal padding to maintain layout
                                                        }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                style={{ width: "136px" }}
                                            />
                                            <Column
                                                header="Khách hàng"
                                                field="customerName"
                                                body={(rowData: OrderAtCounterItem) => (
                                                    <span
                                                        style={{
                                                            cursor: hasPermission("CUSTOMER:CUSTOMER_LIST:VIEW") ? "pointer" : "default",
                                                            color: "#ae510f",
                                                            fontWeight: 600
                                                        }}
                                                    >
                                                        <span
                                                            onClick={
                                                                hasPermission("CUSTOMER:CUSTOMER_LIST:VIEW")
                                                                    ? () => router.push(`/customer/${rowData.customerId}`)
                                                                    : undefined
                                                            }
                                                        >
                                                            {rowData.customerName}
                                                        </span>
                                                    </span>
                                                )}
                                                filter
                                                filterElement={
                                                    <StringFilterInput
                                                        placeholder="Nhập..."
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    customerName: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{
                                                            height: "36px",
                                                            lineHeight: "36px", // Ensures text aligns with the height
                                                            padding: "0 11px" // Adjust horizontal padding to maintain layout
                                                        }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                style={{ minWidth: "180px" }}
                                            />
                                            <Column
                                                header="Số điện thoại"
                                                field="customerPhoneNumber"
                                                filter
                                                filterElement={
                                                    <StringFilterInput
                                                        placeholder="Nhập..."
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    customerPhoneNumber: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{
                                                            height: "36px",
                                                            lineHeight: "36px", // Ensures text aligns with the height
                                                            padding: "0 11px" // Adjust horizontal padding to maintain layout
                                                        }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                style={{ width: "120px" }}
                                            />
                                            <Column
                                                header="Thời gian đặt hàng"
                                                field="orderTime"
                                                filter
                                                filterElement={
                                                    <DateFilterInput
                                                        placeholder="Chọn"
                                                        maxDate={dayjs()}
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    orderTime: value?.format("DD/MM/YYYY") || undefined
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{ height: "36px" }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                style={{ width: "160px" }}
                                                align="center"
                                            />
                                            <Column
                                                header="Tổng tiền (đ)"
                                                field="totalAmount"
                                                filter
                                                filterElement={
                                                    <NumberFilterInput
                                                        placeholder="Nhập..."
                                                        autoFormat={true}
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    totalAmount: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{
                                                            height: "36px",
                                                            lineHeight: "36px", // Ensures text aligns with the height
                                                            padding: "0 11px" // Adjust horizontal padding to maintain layout
                                                        }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                body={(rowData) => rowData.totalAmount.toLocaleString("vi-VN")}
                                                style={{ width: "140px" }}
                                                align="right"
                                            />
                                            <Column
                                                header="Người tạo"
                                                field="createdBy"
                                                filter
                                                filterElement={
                                                    <StringFilterInput
                                                        placeholder="Nhập..."
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    createdBy: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{
                                                            height: "36px",
                                                            lineHeight: "36px", // Ensures text aligns with the height
                                                            padding: "0 11px" // Adjust horizontal padding to maintain layout
                                                        }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                style={{ width: "150px" }}
                                            />
                                            <Column
                                                header="Thanh toán"
                                                filter
                                                filterElement={
                                                    <EnumFilterInput
                                                        placeholder="Chọn"
                                                        enum={enumStatusPay}
                                                        onChange={(value) => {
                                                            setRequest((prev) => ({
                                                                ...prev,
                                                                filter: {
                                                                    ...prev.filter,
                                                                    paymentStatus: value
                                                                },
                                                                pageIndex: 0
                                                            }));
                                                        }}
                                                        style={{ height: "36px" }}
                                                    />
                                                }
                                                showFilterMenu={false}
                                                showClearButton={false}
                                                body={getStatusPayColumnBody}
                                                style={{ width: "130px" }}
                                                align="center"
                                            />
                                            {currentTab == "-1" && (
                                                <Column
                                                    header="Trạng thái"
                                                    filter
                                                    filterElement={
                                                        <EnumFilterInput
                                                            placeholder="Chọn"
                                                            enum={enumStatus}
                                                            onChange={(value) => {
                                                                setRequest((prev) => ({
                                                                    ...prev,
                                                                    filter: {
                                                                        ...prev.filter,
                                                                        status: value
                                                                    },
                                                                    pageIndex: 0
                                                                }));
                                                            }}
                                                            style={{ height: "36px" }}
                                                        />
                                                    }
                                                    showFilterMenu={false}
                                                    showClearButton={false}
                                                    body={getStatusColumnBody}
                                                    style={{ width: "130px" }}
                                                    align="center"
                                                />
                                            )}

                                            <Column header="Thao tác" body={getActionColumnBody} style={{ width: "116px" }} align="right" />
                                        </DataTable>
                                    </div>
                                </>
                            ) : (
                                <div className={`${styles["empty-record"]}`}>
                                    <img src="/images/no_order.svg" alt="" style={{ width: "194px", height: "208px" }} />
                                    <span className="txt-20-n-600 mt-6 mb-3">Chưa có đơn hàng tại quầy</span>
                                    {hasPermission("ORDER:POS_ORDER:ADD") && (
                                        <Button
                                            onClick={() => router.push("/order-at-counter/create")}
                                            className="btn-create"
                                            icon={<PlusOutlined />}
                                        >
                                            Thêm mới
                                        </Button>
                                    )}
                                </div>
                            )}
                        </div>
                    </div>
                </Container>
            </div>
            <HistoryDrawer
                open={isHistoryDrawerOpen}
                onClose={() => setIsHistoryDrawerOpen(false)}
                title="Lịch sử đơn hàng tại quầy"
                items={histories}
                loading={isHistoryLoading}
            />
        </Fragment>
    );
};
