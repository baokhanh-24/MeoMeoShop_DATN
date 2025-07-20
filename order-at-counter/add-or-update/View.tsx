import { useMainLayout } from "@/layouts/MainLayout";
import styles from "./View.module.scss";
import { Container, Row, Col } from "react-bootstrap";
import { Breadcrumb, Button, Form, Select, DatePicker, Input, Spin, InputNumber, Empty, Checkbox, Popover, List, Tooltip, Radio, Switch } from "antd";
import { useRouter } from "next/router";
import { useState, useEffect, useMemo, useRef, useCallback } from "react";
import { PlusOutlined, InfoCircleOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import debounce from "lodash/debounce";
import { ProvinceItem } from "@/@schema/api/province/list";
import { DistrictItem } from "@/@schema/api/order-at-counter/location/district";
import { CommuneItem } from "@/@schema/api/order-at-counter/location/commune";
import { CustomerItem } from "@/@schema/api/order-at-counter/customer/list";
import { ProductSearchItem } from "@/@schema/api/order-at-counter/product/search";
import { GetDraftOrderAtCounterDetailsResponse } from "@/@schema/api/order-at-counter/draft-details";
// PrimeReact imports
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import AddCustomerModal from "./add-customer-modal/View";
import { CreateCustomerRequest, CreateCustomerResponse } from "@/@schema/api/order-at-counter/customer/create";
import { OrderReceipt, OrderReceiptProps } from "@/components/order-receipt/OrderReceipt";
import html2canvas from "html2canvas";
import ReactDOM from "react-dom/client";
import { useUser } from "@/pages/_app";
import CurrencyInput from "@/components/currency";
const { Search } = Input;
import { usePermissionStore } from "@/store/permissionStore";
import { ApiResponse } from "@/utils/ApiUtils";
import SelectCouponModal from "./select-coupon-modal/SelectCouponModal";
import { useReactToPrint } from "react-to-print";
import { CheckDiscountCodeResponse } from "@/@schema/api/order-at-counter/discount-code/check";
import DiscountMiniSummaryView from "./DiscountMiniSummaryView";
import { CreateOrderAtCounterRequest } from "@/@schema/api/order-at-counter/create";
export type OrderAtCounterAddOrUpdateViewProps = {
    pathCDN: string;
    detailsItem?: GetDraftOrderAtCounterDetailsResponse;
    rateAmountPay: number;
    ratePurchase: number;
    bankInfo?: BankInfo;
};

export interface BankInfo {
    ownerName?: string;
    bin: string;
    accountNumber: string;
}

// Define product with quantity for the form
export interface OrderProductItem extends ProductSearchItem {
    quantity: number;
    promotedQuantity?: number;
    nonPromotedQuantity?: number;
}

// Define the form data type with proper types
type FormDataType = {
    type: number;
    orderTime: dayjs.Dayjs;
    customerId?: number;
    delivery: {
        consigneeName: string; // required
        consigneePhone: string; // required
        provinceId?: number; // required on type = 2: Đơn hàng đi giao
        districtId?: number; // required on type = 2: Đơn hàng đi giao
        communeId?: number; // required on type = 2: Đơn hàng đi giao
        consigneeLat?: string;
        consigneeLong?: string;
        consigneeAddress?: string; // required on type = 2: Đơn hàng đi giao
        consigneeNote?: string; // optional
    };
};

export const OrderAtCounterAddOrUpdateView = ({
    pathCDN,
    detailsItem,
    rateAmountPay,
    ratePurchase,
    bankInfo
}: OrderAtCounterAddOrUpdateViewProps) => {
    const { showMessage, setSpinning, formRequiredMark, showConfirmModal } = useMainLayout();
    const router = useRouter();
    const user = useUser();
    const { hasPermission } = usePermissionStore();
    const [form] = Form.useForm<FormDataType>();
    const [orderType, setOrderType] = useState<number>(detailsItem?.type ?? 0);
    const searchInputRef = useRef<any>(null);

    // Product search state
    const [productSearchKeyword, setProductSearchKeyword] = useState("");
    const [searchingProduct, setSearchingProduct] = useState(false);
    const [products, setProducts] = useState<OrderProductItem[]>(
        detailsItem?.products.map((p) => ({
            productId: p.productId,
            comboId: p.comboId,
            isCombo: p.isCombo,
            name: p.name,
            sku: p.sku || "",
            thumbnail: p.thumbnail || "",
            unitName: p.unitName || "",
            price: p.price,
            discountPercent: p.discountPercent,
            promotionId: p.promotionId || null,
            promotionQtyLimit: p.promotionQtyLimit || null,
            barcode: "",
            quantity: p.quantity,
            promotedQuantity: p.promotionQtyLimit ? Math.min(p.quantity, p.promotionQtyLimit) : p.quantity,
            nonPromotedQuantity: p.promotionQtyLimit ? p.quantity - Math.min(p.quantity, p.promotionQtyLimit) : 0,
            comboProducts: p.comboProducts
        })) || []
    );
    const [selectedProducts, setSelectedProducts] = useState<OrderProductItem[]>([]);

    // New states for product search with dropdown
    const [searchResults, setSearchResults] = useState<ProductSearchItem[]>([]);
    const [searchDropdownVisible, setSearchDropdownVisible] = useState(false);
    const [productPage, setProductPage] = useState(1);
    const [productTotal, setProductTotal] = useState(0);
    const [productHasMore, setProductHasMore] = useState(true);
    const [amountPayRate, setAmountPayRate] = useState(rateAmountPay);

    // Province state management
    const [provinces, setProvinces] = useState<ProvinceItem[]>(
        detailsItem?.delivery.provinceId
            ? [
                  {
                      id: detailsItem.delivery.provinceId,
                      name: detailsItem.delivery.provinceName || ""
                  }
              ]
            : []
    );
    const [provincesLoading, setProvincesLoading] = useState(false);
    const [provinceSearch, setProvinceSearch] = useState("");
    const [provincePage, setProvincePage] = useState(1);
    const [provinceTotal, setProvinceTotal] = useState(0);
    const [provinceHasMore, setProvinceHasMore] = useState(true);

    // District state management
    const [districts, setDistricts] = useState<DistrictItem[]>(
        detailsItem?.delivery.districtId
            ? [
                  {
                      id: detailsItem.delivery.districtId,
                      name: detailsItem.delivery.districtName || "",
                      provinceId: detailsItem.delivery.provinceId || 0
                  }
              ]
            : []
    );
    const [districtsLoading, setDistrictsLoading] = useState(false);
    const [districtSearch, setDistrictSearch] = useState("");
    const [districtTotal, setDistrictTotal] = useState(0);

    // Commune state management
    const [communes, setCommunes] = useState<CommuneItem[]>(
        detailsItem?.delivery.communeId
            ? [
                  {
                      id: detailsItem.delivery.communeId,
                      name: detailsItem.delivery.communeName || "",
                      districtId: detailsItem.delivery.districtId || 0
                  }
              ]
            : []
    );
    const [communesLoading, setCommunesLoading] = useState(false);
    const [communeSearch, setCommuneSearch] = useState("");
    const [communeTotal, setCommuneTotal] = useState(0);

    // Customer state management
    const [customers, setCustomers] = useState<CustomerItem[]>([]);
    const [customersLoading, setCustomersLoading] = useState(false);
    const [customerSearch, setCustomerSearch] = useState("");
    const [customerPage, setCustomerPage] = useState(1);
    const [customerTotal, setCustomerTotal] = useState(0);
    const [customerHasMore, setCustomerHasMore] = useState(true);

    //Add customer modal state
    const [addCustomerModalVisible, setAddCustomerModalVisible] = useState(false);

    //Select coupon modal state
    const [selectCouponModalVisible, setSelectCouponModalVisible] = useState(false);

    // Get selected values
    const selectedProvinceId = Form.useWatch(["delivery", "provinceId"], form);
    const selectedDistrictId = Form.useWatch(["delivery", "districtId"], form);

    // Define type options with numerical values matching FormDataType
    const typeOptions = [
        { value: 0, label: "Giao trực tiếp" },
        // { value: 1, label: "Tự đến lấy" },
        { value: 2, label: "Đơn hàng đi giao" }
    ];

    const mapMembershipIcon: Map<string, string> = new Map([
        ["Thành viên", "/images/normal_rank.svg"],
        ["Bạc", "/images/sliver_rank.svg"],
        ["Vàng", "/images/golden_rank.svg"],
        ["Kim cương", "/images/diamond_rank.svg"]
    ]);

    // Add this state after other state declarations
    const [selectedCustomer, setSelectedCustomer] = useState<CustomerItem | null>(null);

    // Add these new states after other state declarations
    // const [loyaltyPoints, setLoyaltyPoints] = useState(10000); // Example value, will be from API
    const [shippingFee, setShippingFee] = useState<string>("");
    const [discountCode, setDiscountCode] = useState<string>("");
    const [discountCodeResponse, setDiscountCodeResponse] = useState<ApiResponse<CheckDiscountCodeResponse> | null>(null);
    const [discountCodeAmount, setDiscountCodeAmount] = useState<number>(0);
    const [discountCodeError, setDiscountCodeError] = useState<string | null>(null);
    const [usePoint, setUsePoint] = useState<boolean>(false);
    const [paymentType, setPaymentType] = useState<number>(0); // Changed from number | null to number
    const [minusMembership, setMinusMembership] = useState<number>(0);

    // Add this state near other state declarations
    const [orderReceipt, setOrderReceipt] = useState<OrderReceiptProps | null>(null);
    const receiptRef = useRef<HTMLDivElement>(null);

    const [receiptHeight, setReceiptHeight] = useState<number>(0);
    // Add this after other useRef declarations
    const abortControllerRef = useRef<AbortController | null>(null);

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
    }, [receiptRef, orderReceipt]); // Add orderReceipt as dependency

    // Add this to your component
    const handlePrint = useReactToPrint({
        content: () => receiptRef.current,
        pageStyle: () => {
            console.log("pageStyle", receiptHeight);
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
        }, // Add the print styles
        onAfterPrint: () => {
            // showMessage("success", "Đã in hóa đơn thành công");
        },
        onPrintError: () => {
            showMessage("error", "Lỗi khi in hóa đơn");
        }
    });

    // Dynamic data loading functions for provinces
    const fetchProvinces = async (keyword = "", page = 1, loadMore = false, id: number | undefined = undefined) => {
        setProvincesLoading(true);
        try {
            const response = await fetch("/api/order-at-counter/location/getListProvince", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    pageIndex: page - 1, // Convert to 0-based for API
                    pageSize: 10,
                    keyword: keyword || undefined, // Ensure empty string is sent as undefined
                    id: id || undefined
                })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tải danh sách tỉnh/thành phố");
                return;
            }

            const result = await response.json();

            if (result.status === 1) {
                const { items, total, totalPages } = result.data;

                if (loadMore) {
                    setProvinces((prev) => [...prev, ...items]);
                } else {
                    setProvinces(items);
                }

                setProvinceTotal(total);
                setProvinceHasMore(page < totalPages);
            } else {
                showMessage("error", result.message || "Failed to load provinces");
            }
        } catch (error) {
            console.error("Error fetching provinces:", error);
            showMessage("error", "Failed to load provinces");
        } finally {
            setProvincesLoading(false);
        }
    };

    // Dynamic data loading functions for districts
    const fetchDistricts = async (provinceId?: number, keyword = "") => {
        if (!provinceId) {
            setDistricts([]);
            return;
        }

        setDistrictsLoading(true);
        try {
            const response = await fetch("/api/order-at-counter/location/getListDistrict", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    provinceId,
                    keyword: keyword || undefined
                })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tải danh sách quận/huyện");
                return;
            }

            const result = await response.json();

            if (result.status === 1) {
                const { items, total, totalPages } = result.data;

                setDistricts(items);

                setDistrictTotal(total);
            } else {
                showMessage("error", result.message || "Không thể tải danh sách quận/huyện");
            }
        } catch (error) {
            console.error("Error fetching districts:", error);
            showMessage("error", "Lỗi khi tải danh sách quận/huyện");
        } finally {
            setDistrictsLoading(false);
        }
    };

    // Dynamic data loading functions for communes
    const fetchCommunes = async (districtId?: number, keyword = "") => {
        if (!districtId) {
            setCommunes([]);
            return;
        }

        setCommunesLoading(true);
        try {
            const response = await fetch("/api/order-at-counter/location/getListCommune", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    districtId,
                    keyword: keyword || undefined
                })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tải danh sách phường/xã");
                return;
            }

            const result = await response.json();

            if (result.status === 1) {
                const { items, total, totalPages } = result.data;

                setCommunes(items);

                setCommuneTotal(total);
            } else {
                showMessage("error", result.message || "Không thể tải danh sách phường/xã");
            }
        } catch (error) {
            console.error("Error fetching communes:", error);
            showMessage("error", "Lỗi khi tải danh sách phường/xã");
        } finally {
            setCommunesLoading(false);
        }
    };

    // Dynamic data loading function for customers
    const fetchCustomers = async (keyword = "", page = 1, loadMore = false) => {
        setCustomersLoading(true);
        try {
            const response = await fetch("/api/order-at-counter/customer/getList", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    pageIndex: page - 1, // Convert to 0-based for API
                    pageSize: 10,
                    keyword: keyword || undefined // Ensure empty string is sent as undefined
                })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tải danh sách khách hàng");
                return;
            }

            const result = await response.json();

            if (result.status === 1) {
                const { items, total, totalPages } = result.data;

                if (loadMore) {
                    setCustomers((prev) => [...prev, ...items]);
                } else {
                    setCustomers(items);
                }

                setCustomerTotal(total);
                setCustomerHasMore(page < totalPages);
            } else {
                showMessage("error", result.message || "Failed to load customers");
            }
        } catch (error) {
            console.error("Error fetching customers:", error);
            showMessage("error", "Failed to load customers");
        } finally {
            setCustomersLoading(false);
        }
    };

    // Debounced search functions
    const debouncedProvinceSearch = useMemo(
        () =>
            debounce((value: string) => {
                setProvinceSearch(value);
                setProvincePage(1);
                fetchProvinces(value, 1);
            }, 500),
        []
    );

    const debouncedDistrictSearch = useMemo(
        () =>
            debounce((value: string) => {
                setDistrictSearch(value);
                fetchDistricts(selectedProvinceId, value);
            }, 500),
        [selectedProvinceId]
    );

    const debouncedCommuneSearch = useMemo(
        () =>
            debounce((value: string) => {
                setCommuneSearch(value);
                fetchCommunes(selectedDistrictId, value);
            }, 500),
        [selectedDistrictId]
    );

    const debouncedCustomerSearch = useMemo(
        () =>
            debounce((value: string) => {
                setCustomerSearch(value);
                setCustomerPage(1);
                fetchCustomers(value, 1);
            }, 500),
        []
    );

    // Debounced product search
    const debouncedProductSearch = useMemo(
        () =>
            debounce((value: string) => {
                setProductPage(1);
                fetchProducts(value, 1);
            }, 300), // Reduced from 500ms to 300ms for better responsiveness
        []
    );

    // Scroll handlers for infinite loading
    const handleProvinceScroll = (e: React.UIEvent<HTMLDivElement>) => {
        const target = e.target as HTMLDivElement;
        if (!provincesLoading && provinceHasMore && target.scrollTop + target.offsetHeight >= target.scrollHeight - 20) {
            const nextPage = provincePage + 1;
            setProvincePage(nextPage);
            fetchProvinces(provinceSearch, nextPage, true);
        }
    };

    const handleCustomerScroll = (e: React.UIEvent<HTMLDivElement>) => {
        const target = e.target as HTMLDivElement;
        if (!customersLoading && customerHasMore && target.scrollTop + target.offsetHeight >= target.scrollHeight - 20) {
            const nextPage = customerPage + 1;
            setCustomerPage(nextPage);
            fetchCustomers(customerSearch, nextPage, true);
        }
    };

    // Handle scroll for infinite loading products
    const handleProductScroll = (e: React.UIEvent<HTMLDivElement>) => {
        const target = e.target as HTMLDivElement;
        if (!searchingProduct && productHasMore && target.scrollTop + target.offsetHeight >= target.scrollHeight - 20) {
            const nextPage = productPage + 1;
            setProductPage(nextPage);
            fetchProducts(productSearchKeyword, nextPage, true);
        }
    };

    // Handle selection changes
    const handleProvinceChange = (value: number) => {
        // Reset district and commune when province changes
        form.setFieldsValue({
            delivery: {
                ...form.getFieldValue("delivery"),
                districtId: undefined,
                communeId: undefined
            }
        });

        // Reset states
        setDistricts([]);
        setCommunes([]);

        // Load districts for the selected province
        if (value) {
            fetchDistricts(value);
        }
    };

    const handleDistrictChange = (value: number) => {
        // Reset commune when district changes
        form.setFieldsValue({
            delivery: {
                ...form.getFieldValue("delivery"),
                communeId: undefined
            }
        });

        // Reset states
        setCommunes([]);

        // Load communes for the selected district
        if (value) {
            fetchCommunes(value);
        }
    };

    // Load initial provinces when component mounts
    useEffect(() => {
        fetchProvinces("", 1, false, detailsItem?.delivery.provinceId || undefined);
        fetchCustomers();
    }, []);

    // Load districts when province changes
    useEffect(() => {
        if (selectedProvinceId) {
            fetchDistricts(selectedProvinceId);
        } else {
            setDistricts([]);
        }
    }, [selectedProvinceId]);

    // Load communes when district changes
    useEffect(() => {
        if (selectedDistrictId) {
            fetchCommunes(selectedDistrictId);
        } else {
            setCommunes([]);
        }
    }, [selectedDistrictId]);

    // Cleanup debounce functions on unmount
    useEffect(() => {
        return () => {
            debouncedProvinceSearch.cancel();
            debouncedDistrictSearch.cancel();
            debouncedCommuneSearch.cancel();
            debouncedCustomerSearch.cancel();
            debouncedProductSearch.cancel();
        };
    }, [debouncedProvinceSearch, debouncedDistrictSearch, debouncedCommuneSearch, debouncedCustomerSearch, debouncedProductSearch]);

    const checkDiscountCode = async () => {
        setDiscountCodeError(null);
        setDiscountCodeAmount(0);
        if (discountCode.length === 0) {
            setDiscountCodeResponse(null);
            return;
        }
        setSpinning(true);
        try {
            const response = await fetch("/api/order-at-counter/discount-code/check", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ code: discountCode })
            });
            const result: ApiResponse<CheckDiscountCodeResponse> = await response.json();
            setDiscountCodeResponse(result);
        } catch (error) {}

        setSpinning(false);
    };

    // Watch for changes in the order type
    useEffect(() => {
        const type = form.getFieldValue("type");
        setOrderType(type);
    }, [form]);

    const handleTypeChange = (value: number) => {
        setOrderType(value);
        setShippingFee("");

        // // Reset delivery fields when changing type
        // if (value !== 2) {
        //     form.setFieldsValue({
        //         delivery: {
        //             ...form.getFieldValue("delivery"),
        //             provinceId: undefined,
        //             districtId: undefined,
        //             communeId: undefined,
        //             consigneeAddress: undefined
        //         }
        //     });

        //     // Reset location data arrays
        //     setProvinces([]);
        //     setDistricts([]);
        //     setCommunes([]);
        // } else {
        //     // Load provinces when switching to delivery type
        //     fetchProvinces();
        // }
    };

    const handleCancel = () => {
        form.resetFields();
        setProductSearchKeyword("");
        if (detailsItem) {
            setOrderType(detailsItem.type);
            setProducts(
                detailsItem.products.map((p) => ({
                    productId: p.productId,
                    comboId: p.comboId,
                    isCombo: p.isCombo,
                    name: p.name,
                    sku: p.sku || "",
                    thumbnail: p.thumbnail || "",
                    unitName: p.unitName || "",
                    price: p.price,
                    discountPercent: p.discountPercent,
                    promotionId: p.promotionId || null,
                    promotionQtyLimit: p.promotionQtyLimit || null,
                    barcode: "",
                    quantity: p.quantity,
                    promotedQuantity: p.promotionQtyLimit ? Math.min(p.quantity, p.promotionQtyLimit) : p.quantity,
                    nonPromotedQuantity: p.promotionQtyLimit ? p.quantity - Math.min(p.quantity, p.promotionQtyLimit) : 0,
                    comboProducts: p.comboProducts
                }))
            );
            setProductPage(1);
            setProductTotal(0);
            setProductHasMore(false);
            setSearchResults([]);
            setSearchDropdownVisible(false);
            setSearchingProduct(false);
            setProvinces(
                detailsItem.delivery.provinceId
                    ? [
                          {
                              id: detailsItem.delivery.provinceId,
                              name: detailsItem.delivery.provinceName || ""
                          }
                      ]
                    : []
            );
            setDistricts(
                detailsItem.delivery.districtId
                    ? [
                          {
                              id: detailsItem.delivery.districtId,
                              name: detailsItem.delivery.districtName || "",
                              provinceId: detailsItem.delivery.provinceId || 0
                          }
                      ]
                    : []
            );
            setCommunes(
                detailsItem.delivery.communeId
                    ? [
                          {
                              id: detailsItem.delivery.communeId,
                              name: detailsItem.delivery.communeName || "",
                              districtId: detailsItem.delivery.districtId || 0
                          }
                      ]
                    : []
            );
            if (detailsItem.customer) {
                const customer = {
                    id: detailsItem.customer.id,
                    name: detailsItem.customer.name,
                    phone: detailsItem.customer.phone,
                    remainingPoint: detailsItem.customer.remainingPoint,
                    membershipId: detailsItem.customer.membershipId,
                    membershipName: detailsItem.customer.membershipName,
                    membershipDiscount: detailsItem.customer.membershipDiscount,
                    membershipMinTotalAmount: detailsItem.customer.membershipMinTotalAmount,
                    membershipOtherProgram: detailsItem.customer.membershipOtherProgram,
                    userId: detailsItem.customer.userId || -1 // Set default userId if not provided
                };
                setCustomers([customer]);
                setSelectedCustomer(customer);
            }
            if (detailsItem.discountCode) {
                setDiscountCode(detailsItem.discountCode.code);
                setDiscountCodeResponse({
                    status: 1,
                    data: {
                        id: detailsItem.discountCode.id,
                        code: detailsItem.discountCode.code,
                        type: detailsItem.discountCode.type,
                        discount: detailsItem.discountCode.discount,
                        minOder: detailsItem.discountCode.minOder,
                        maxDiscount: detailsItem.discountCode.maxDiscount || undefined
                    }
                });
            }
        } else {
            setOrderType(0);
            setProducts([]);
            setProductPage(1);
            setProductTotal(0);
            setProductHasMore(false);
            setSearchResults([]);
            setSearchDropdownVisible(false);
            setSearchingProduct(false);
            setCustomers([]);
            setCustomerPage(1);
            setCustomerSearch("");
            setCustomersLoading(false);
            setDistricts([]);
            setDistrictTotal(0);
            setDistrictsLoading(false);
            setDistrictSearch("");
            setCommunes([]);
            setCommuneTotal(0);
            setCommunesLoading(false);
            setCommuneSearch("");
            setSearchResults([]);
            setSearchingProduct(false);
            setProductSearchKeyword("");
            setDiscountCodeResponse(null);
            setDiscountCode("");
            setDiscountCodeError(null);
            setDiscountCodeAmount(0);
        }
    };

    const handleSave = async (saveAsDraft: boolean = false) => {
        try {
            const values = await form.validateFields();
            if (products.length === 0) {
                showMessage("error", "Vui lòng chọn sản phẩm vào danh sách");
                return;
            }
            const requestBody: CreateOrderAtCounterRequest = {
                type: values.type,
                orderTime: values.orderTime.format("YYYY-MM-DD HH:mm:ss"),
                customerId: values.customerId,
                delivery:
                    values.type === 2
                        ? {
                              consigneeName: values.delivery.consigneeName,
                              consigneePhone: values.delivery.consigneePhone,
                              provinceId: values.delivery.provinceId,
                              districtId: values.delivery.districtId,
                              communeId: values.delivery.communeId,
                              consigneeAddress: values.delivery.consigneeAddress,
                              consigneeLat: values.delivery.consigneeLat,
                              consigneeLong: values.delivery.consigneeLong,
                              consigneeNote: values.delivery.consigneeNote
                          }
                        : null,
                products: products.map((p) => ({
                    productId: p.productId,
                    comboId: p.comboId,
                    quantity: p.quantity,
                    promotionId: p.promotionId || null
                })),
                shippingFee: Number(shippingFee) || 0,
                discountCodeId: discountCodeResponse?.data?.id || null,
                discountCodeAmount: discountCodeAmount,
                membership: selectedCustomer?.membershipId
                    ? {
                          id: selectedCustomer.membershipId,
                          name: selectedCustomer.membershipName,
                          otherProgram: selectedCustomer.membershipOtherProgram,
                          discount: selectedCustomer.membershipDiscount,
                          minTotalAmount: selectedCustomer.membershipMinTotalAmount
                      }
                    : null,
                pointUse: calcPointUse(),
                amountPayRate: amountPayRate,
                paymentType: paymentType,
                totalAmount: calculateTotal(),
                saveType: saveAsDraft ? 0 : 1 // 0: Draft, 1: Save
            };

            setSpinning(true);

            // Determine if this is an update or create operation
            const isUpdate = !!detailsItem;
            const endpoint = isUpdate ? `/api/order-at-counter/update` : `/api/order-at-counter/create`;
            const method = isUpdate ? "PUT" : "POST";

            // Add id to request body if updating
            const finalRequestBody = isUpdate ? { ...requestBody, id: detailsItem.id } : requestBody;

            const response = await fetch(endpoint, {
                method,
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(finalRequestBody)
            });

            const result = await response.json();

            if (result.status === 1) {
                showMessage("success", saveAsDraft ? "Lưu tạm thành công" : isUpdate ? "Cập nhật đơn hàng thành công" : "Tạo đơn hàng thành công");
                router.push("/order-at-counter");
            } else {
                if (result.message === "POINT_CARD_CHANGE") {
                    showConfirmModal({
                        title: "Thông báo",
                        content:
                            "Giá trị trừ điểm tích lũy đã thay đổi (có thể do nhà bán hàng thay đổi quy tắc đổi điểm hoặc số điểm của khách hàng đã thay đổi), vui lòng kiểm tra lại",
                        disableCancel: true
                    });
                    setAmountPayRate(result.data.rateAmountPay);
                    setSelectedCustomer({
                        ...selectedCustomer!,
                        remainingPoint: result.data.remainingPoint || 0
                    });
                } else if (result.message === "MEMBERSHIP_CHANGE") {
                    showConfirmModal({
                        title: "Thông báo",
                        content:
                            "Giá trị trừ tiền theo hạng thành viên đã thay đổi (có thể do nhà bán hàng thay đổi chiết khấu hoặc hạng thành viên của khách hàng đã thay đổi), vui lòng kiểm tra lại",
                        disableCancel: true
                    });
                    setSelectedCustomer({
                        ...selectedCustomer!,
                        membershipId: result.data.membership?.id || null,
                        membershipName: result.data.membership?.name || "",
                        membershipDiscount: result.data.membership?.discount || 0,
                        membershipMinTotalAmount: result.data.membership?.minTotalAmount || 0,
                        membershipOtherProgram: result.data.membership?.otherProgram || false
                    });
                } else if (
                    result.message === "DISCOUNT_CODE_NOT_FOUND" ||
                    result.message === "DISCOUNT_CODE_EXPIRED" ||
                    result.message === "DISCOUNT_CODE_OUT_OF_USE"
                ) {
                    showConfirmModal({
                        title: "Thông báo",
                        content: "Mã giảm giá không còn hiệu lực (có thể do hết thời gian áp dụng hoặc hết lượt sử dụng), vui lòng kiểm tra lại",
                        disableCancel: true
                    });
                    setDiscountCodeResponse(null);
                    setDiscountCode("");
                    setDiscountCodeError(null);
                    setDiscountCodeAmount(0);
                } else if (result.message === "DISCOUNT_CODE_AMOUNT_CHANGE") {
                    showConfirmModal({
                        title: "Thông báo",
                        content: "Số tiền áp dụng mã giảm giá đã thay đổi, vui lòng kiểm tra lại",
                        disableCancel: true
                    });
                    if (result.data.discountCode) {
                        setDiscountCodeResponse({
                            status: 1,
                            data: {
                                id: result.data.discountCode.id,
                                code: result.data.discountCode.code,
                                type: result.data.discountCode.type,
                                discount: result.data.discountCode.discount,
                                minOder: result.data.discountCode.minOder,
                                maxDiscount: result.data.discountCode.maxDiscount
                            }
                        });
                    }
                } else if (result.message === "TOTAL_AMOUNT_CHANGE") {
                    showConfirmModal({
                        title: "Thông báo",
                        content: "Số tiền đơn hàng đã thay đổi, vui lòng kiểm tra lại",
                        disableCancel: true
                    });
                } else {
                    showMessage("error", result.message || (isUpdate ? "Lỗi khi cập nhật đơn hàng" : "Lỗi khi tạo đơn hàng"));
                }
            }
        } finally {
            // catch (error) {
            //     debugger;
            //     console.error("Error saving order:", error);
            //     showMessage("error", detailsItem ? "Lỗi khi cập nhật đơn hàng" : "Lỗi khi tạo đơn hàng");
            // }
            setSpinning(false);
        }
    };

    const handleGoBack = () => {
        router.back(); // Quay lại trang trước đó
    };

    const handlePrintOrder = async () => {
        if (!orderReceipt) {
            showMessage("error", "Thông tin đơn hàng chưa đầy đủ");
            return;
        }
        handlePrint?.();
    };

    const handleAddNewCustomer = () => {
        setAddCustomerModalVisible(true);
    };

    const handleCloseAddCustomerModal = () => {
        setAddCustomerModalVisible(false);
        // Optionally refresh customer list after adding
        fetchCustomers();
    };

    const handleCloseSelectCouponModal = () => {
        setSelectCouponModalVisible(false);
    };

    // Get the current date and time for the default value
    const currentDateTime = dayjs();

    // Format customer label for display in dropdown
    const formatCustomerLabel = (customer: CustomerItem) => {
        return `${customer.name}/${customer.phone}`;
    };

    // Fetch products with keyword search and pagination
    const fetchProducts = async (keyword = "", page = 1, loadMore = false) => {
        // Only show loading indicator for first page or manual search
        if (page === 1) {
            setSearchingProduct(true);
        }

        try {
            const response = await fetch("/api/order-at-counter/product/search", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    keyword: keyword || undefined,
                    pageIndex: page - 1,
                    pageSize: 10
                })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tìm kiếm sản phẩm");
                return;
            }

            const result = await response.json();

            if (result.status === 1) {
                const { items, totalItems, totalPages } = result.data;

                if (loadMore) {
                    setSearchResults((prev) => [...prev, ...items]);
                } else {
                    setSearchResults(items);
                }

                setProductTotal(totalItems);
                setProductHasMore(page < totalPages);

                // Always show dropdown with either results or empty message
                setSearchDropdownVisible(true);
            } else {
                showMessage("error", result.message || "Không thể tìm kiếm sản phẩm");
                setSearchDropdownVisible(false);
            }
        } catch (error) {
            console.error("Error searching products:", error);
            showMessage("error", "Lỗi khi tìm kiếm sản phẩm");
            setSearchDropdownVisible(false);
        } finally {
            setSearchingProduct(false);
        }
    };

    // Add product from search results
    const addProductFromSearch = (product: ProductSearchItem) => {
        addProductToTable(product);
        setProductSearchKeyword("");
        setSearchResults([]);
        setSearchDropdownVisible(false);
    };

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            // Check if click is outside the search input and dropdown
            const searchInput = document.getElementById("product-search-input");
            const searchDropdown = document.getElementById("product-search-dropdown");

            if (searchInput && searchDropdown && !searchInput.contains(event.target as Node) && !searchDropdown.contains(event.target as Node)) {
                setSearchDropdownVisible(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, []);

    // Update product quantity
    const updateProductQuantity = (productId: number, comboId: number, quantity: number) => {
        if (quantity < 1) return;

        const updatedProducts = products.map((product) => {
            if (product.productId === productId && comboId === 0) {
                // Get promotion limit if available
                const promotionLimit = product.promotionQtyLimit || quantity;

                // Calculate promoted and non-promoted quantities
                const promotedQuantity = Math.min(quantity, promotionLimit);
                const nonPromotedQuantity = quantity - promotedQuantity;

                return {
                    ...product,
                    quantity,
                    promotedQuantity,
                    nonPromotedQuantity
                };
            } else if (productId === 0 && product.comboId === comboId) {
                return {
                    ...product,
                    quantity: quantity,
                    promotedQuantity: quantity,
                    nonPromotedQuantity: 0
                };
            }
            return product;
        });
        setProducts(updatedProducts);
    };

    // Remove product from list
    const removeProduct = (productId: number, comboId: number) => {
        const updatedProducts = products.filter((product) => product.productId !== productId || product.comboId !== comboId);
        setProducts(updatedProducts);
    };

    const skuTemplate = (rowData: OrderProductItem) => {
        const hasViewPermission = rowData.productId
            ? hasPermission("PRODUCT_MANAGE:PRODUCT:VIEW")
            : hasPermission("PRODUCT_MANAGE:PRODUCT_COMBO:VIEW");
        return (
            <span style={{ cursor: hasViewPermission ? "pointer" : "default", color: "#ae510f", fontWeight: 600 }}>
                <span
                    onClick={() =>
                        hasViewPermission
                            ? rowData.isCombo
                                ? window.open(`/product/combo/${rowData.comboId}`, "_blank")
                                : window.open(`/product/detail-product/${rowData.productId}`, "_blank")
                            : undefined
                    }
                >
                    {rowData.sku || "-"}
                </span>
            </span>
        );
    };

    const imageTemplate = (rowData: OrderProductItem) => {
        return (
            <div style={{ width: 60, height: 40, display: "flex", alignItems: "center", justifyContent: "center" }}>
                <img
                    src={`${pathCDN}${rowData.thumbnail}`}
                    alt={rowData.name}
                    style={{
                        maxWidth: "100%",
                        maxHeight: "100%",
                        objectFit: "contain",
                        border: "1px solid #22313F1A",
                        borderRadius: "4px"
                    }}
                />
            </div>
        );
    };

    const priceTemplate = (rowData: OrderProductItem) => {
        const price = rowData.price;
        const discount = rowData.discountPercent || 0;
        const discountedPrice = price - (price * discount) / 100;

        return (
            <div
                style={{
                    width: 100,
                    height: 40,
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "flex-start",
                    gap: 4,
                    display: "inline-flex"
                }}
            >
                <div
                    style={{
                        width: 100,
                        textAlign: "right",
                        color: "#FF3D32",
                        fontSize: 14,
                        fontFamily: "Inter",
                        fontWeight: 500,
                        lineHeight: "20px"
                    }}
                >
                    {discountedPrice.toLocaleString("vi-VN")}
                </div>
                {discount > 0 && (
                    <div
                        style={{
                            width: 100,
                            textAlign: "right",
                            color: "rgba(34, 49, 63, 0.70)",
                            fontSize: 13,
                            fontFamily: "Inter",
                            fontStyle: "italic",
                            fontWeight: 400,
                            textDecoration: "line-through",
                            lineHeight: "16px"
                        }}
                    >
                        {price.toLocaleString("vi-VN")}
                    </div>
                )}
            </div>
        );
    };

    // Quantity template using Ant Design InputNumber
    const quantityTemplate = (rowData: OrderProductItem) => {
        return (
            <InputNumber
                value={rowData.quantity}
                onChange={(value) => updateProductQuantity(rowData.productId, rowData.comboId, Number(value) || 1)}
                min={1}
                step={1}
                controls
                // height={"20px"}
                style={{ width: "100%", textAlignLast: "center" }}
            />
        );
    };

    const totalPriceTemplate = (rowData: OrderProductItem) => {
        const price = rowData.price;
        const discount = rowData.discountPercent || 0;
        const discountedPrice = price - (price * discount) / 100;

        let totalPrice = 0;

        if (rowData.promotedQuantity && rowData.promotedQuantity > 0) {
            // Calculate price for promoted quantity
            totalPrice += discountedPrice * rowData.promotedQuantity;

            // Add price for non-promoted quantity if exists
            if (rowData.nonPromotedQuantity && rowData.nonPromotedQuantity > 0) {
                totalPrice += price * rowData.nonPromotedQuantity;
            }
        } else {
            // No promotion split, calculate normally
            totalPrice = discountedPrice * rowData.quantity;
        }

        return (
            <div style={{ display: "flex", justifyContent: "flex-end", alignItems: "center", width: "100%" }}>
                {totalPrice.toLocaleString("vi-VN")}
                {rowData.promotedQuantity && rowData.nonPromotedQuantity ? (
                    <Tooltip title={`${rowData.promotedQuantity} sản phẩm được giảm giá, ${rowData.nonPromotedQuantity} sản phẩm giá gốc`}>
                        <InfoCircleOutlined style={{ marginLeft: 8, color: "#646F79", fontSize: 16 }} />
                    </Tooltip>
                ) : null}
            </div>
        );
    };

    const actionTemplate = (rowData: OrderProductItem) => {
        return (
            <Tooltip title="Xóa">
                <img
                    className="mx-1"
                    src="/images/actions/delete-action.svg"
                    alt="Xóa"
                    onClick={() => removeProduct(rowData.productId, rowData.comboId)}
                    style={{ cursor: "pointer" }}
                />
            </Tooltip>
        );
    };

    // Add state for default address checkbox
    const [useDefaultAddress, setUseDefaultAddress] = useState(false);
    // Get selected customer ID for watching changes
    const selectedCustomerId = Form.useWatch("customerId", form);
    // Handle checkbox change for default address
    const handleDefaultAddressChange = async (e: any) => {
        const checked = e.target.checked;
        setUseDefaultAddress(checked);

        if (checked) {
            // If checked, try to fetch and apply default address
            await fetchAndApplyDefaultAddress();
        } else {
            // If unchecked, reset address fields
            resetAddressFields();
        }
    };

    // Fetch and apply default address
    const fetchAndApplyDefaultAddress = async () => {
        console.log(form.getFieldsValue());
        const customerId = form.getFieldValue("customerId");

        if (!customerId) {
            showMessage("warning", "Chưa chọn khách hàng");
            setUseDefaultAddress(false);
            return;
        }

        try {
            setSpinning(true);
            const response = await fetch("/api/order-at-counter/customer/getDefaultDeliveryAddress", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ customerId })
            });

            if (!response.ok) {
                showMessage("error", "Lỗi khi tải địa chỉ mặc định");
                setUseDefaultAddress(false);
                return;
            }

            const result = await response.json();

            if (result.status === 1 && result.data.address) {
                // Apply address data to form
                const address = result.data.address;

                // Make sure location data is loaded

                // Trigger data loading for districts and communes
                if (address.provinceId) {
                    await fetchProvinces("", 1, false, address.provinceId);
                    await fetchDistricts(address.provinceId);
                }
                if (address.districtId) {
                    await fetchCommunes(address.districtId);
                }

                form.setFieldsValue({
                    delivery: {
                        ...form.getFieldValue("delivery"),
                        consigneeName: address.name, // Apply name from address
                        consigneePhone: address.phone, // Apply phone from address
                        provinceId: address.provinceId,
                        districtId: address.districtId,
                        communeId: address.communeId,
                        consigneeAddress: address.address,
                        consigneeLat: address.lat,
                        consigneeLong: address.long
                    }
                });

                showMessage("success", "Đã áp dụng địa chỉ mặc định");
            } else {
                showMessage("warning", "Khách hàng chưa có địa chỉ mặc định");
                setUseDefaultAddress(false);
            }
        } catch (error) {
            console.error("Error fetching default address:", error);
            showMessage("error", "Lỗi khi tải địa chỉ mặc định");
            setUseDefaultAddress(false);
        } finally {
            setSpinning(false);
        }
    };

    // Reset address fields
    const resetAddressFields = () => {
        // Get current name and phone values to preserve them if needed
        const currentDelivery = form.getFieldValue("delivery");

        form.setFieldsValue({
            delivery: {
                ...currentDelivery,
                consigneeName: undefined,
                consigneePhone: undefined,
                provinceId: undefined,
                districtId: undefined,
                communeId: undefined,
                consigneeAddress: undefined,
                consigneeLat: undefined,
                consigneeLong: undefined,
                consigneeNote: undefined
            }
        });

        // Reset location data arrays
        setDistricts([]);
        setCommunes([]);
    };

    // Watch for changes in customerId
    useEffect(() => {
        if (useDefaultAddress && selectedCustomerId) {
            fetchAndApplyDefaultAddress();
        }
    }, [selectedCustomerId]);

    // Product search dropdown content
    const productSearchDropdown = (
        <div
            id="product-search-dropdown"
            style={{
                width: 460,
                maxHeight: 244,
                overflow: "auto",
                background: "white",
                boxShadow: "0 3px 6px -4px rgba(0, 0, 0, 0.12), 0 6px 16px 0 rgba(0, 0, 0, 0.08), 0 9px 28px 8px rgba(0, 0, 0, 0.05)",
                borderRadius: 8
            }}
            onScroll={handleProductScroll}
        >
            <List
                itemLayout="horizontal"
                dataSource={searchResults}
                loading={searchingProduct}
                renderItem={(item) => (
                    <List.Item
                        key={`${item.productId}-${item.comboId}`}
                        onClick={() => addProductFromSearch(item)}
                        style={{ cursor: "pointer", padding: "8px 12px" }}
                        className={styles["product-item-hover"]}
                    >
                        <List.Item.Meta
                            avatar={
                                <div style={{ width: 60, height: 40, display: "flex", alignItems: "center", justifyContent: "center" }}>
                                    <img
                                        src={`${pathCDN}${item.thumbnail}`}
                                        alt={item.name}
                                        style={{
                                            maxWidth: "100%",
                                            maxHeight: "100%",
                                            objectFit: "contain",
                                            border: "1px solid #22313F1A",
                                            borderRadius: "4px"
                                        }}
                                    />
                                </div>
                            }
                            title={
                                <div style={{ display: "flex", justifyContent: "space-between" }}>
                                    <span className="txt-14-n-500">{item.name}</span>
                                    <span className="txt-14-n-500" style={{ color: "#FF3D32" }}>
                                        {(item.price - (item.price * (item.discountPercent || 0)) / 100).toLocaleString("vi-VN")}đ
                                    </span>
                                </div>
                            }
                            description={
                                <div style={{ display: "flex", flexDirection: "column", fontSize: 12 }}>
                                    <div style={{ display: "flex", justifyContent: "space-between" }}>
                                        <span className="txt-14-n-400">Mã SP: {item.sku || "N/A"}</span>
                                        {item.discountPercent > 0 && (
                                            <span
                                                className="txt-13-n-400"
                                                style={{ textDecoration: "line-through", color: "rgba(34, 49, 63, 0.70)" }}
                                            >
                                                {item.price.toLocaleString("vi-VN")}đ
                                            </span>
                                        )}
                                    </div>
                                    {item.barcode && (
                                        <div style={{ marginTop: 4 }}>
                                            <span className="txt-14-n-400">Barcode: {item.barcode}</span>
                                        </div>
                                    )}
                                </div>
                            }
                        />
                    </List.Item>
                )}
                locale={{
                    emptyText: <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} description="Không tìm thấy sản phẩm" />
                }}
            />
        </div>
    );

    // Add this helper function near other handlers
    const addProductToTable = (product: ProductSearchItem) => {
        const existingProductIndex = products.findIndex((p) => p.productId === product.productId && p.comboId === product.comboId);

        if (existingProductIndex >= 0) {
            // Product already exists, increment quantity
            const updatedProducts = [...products];
            updatedProducts[existingProductIndex].quantity += 1;
            setProducts(updatedProducts);
            showMessage("success", `Đã thêm sản phẩm "${product.name}" (số lượng: ${updatedProducts[existingProductIndex].quantity})`);
        } else {
            // Add new product with quantity 1
            const newProduct: OrderProductItem = {
                ...product,
                quantity: 1
            };
            setProducts([newProduct, ...products]);
            showMessage("success", `Đã thêm sản phẩm "${product.name}"`);
        }
    };

    // Add this new function before the return statement
    const handleRemoveSelectedProducts = () => {
        if (selectedProducts.length === 0) return;

        const updatedProducts = products.filter(
            (product) => !selectedProducts.some((p) => p.productId === product.productId && p.comboId === product.comboId)
        );
        setProducts(updatedProducts);
        setSelectedProducts([]);
        showMessage("success", "Đã xóa sản phẩm thành công");
    };

    const handleAddCustomer = async (name: string, phone: string, createAccount: boolean) => {
        try {
            setSpinning(true);
            const requestCreateCustomer: CreateCustomerRequest = {
                name,
                phone,
                createAccount
            };
            const response = await fetch("/api/order-at-counter/customer/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(requestCreateCustomer)
            });
            if (response.status === 400) {
                const result = await response.json();
                showMessage("error", result.message || "Lỗi khi tạo khách hàng");
                return;
            }
            if (!response.ok) {
                showMessage("error", "Lỗi khi tạo khách hàng");
                return;
            }

            const result: ApiResponse<CreateCustomerResponse> = await response.json();

            if (result.status === 1 && result.data) {
                showMessage("success", "Tạo khách hàng thành công");

                // Create customer object from API response
                const newCustomer = {
                    id: result.data.id,
                    name: result.data.name,
                    phone: result.data.phone,
                    userId: result.data.userId || -1,
                    remainingPoint: 0,
                    membershipId: result.data.membershipId || null,
                    membershipName: result.data.membershipName,
                    membershipDiscount: result.data.membershipDiscount,
                    membershipMinTotalAmount: result.data.membershipMinTotalAmount,
                    membershipOtherProgram: result.data.membershipOtherProgram
                };

                // Update customers list with new customer
                setCustomers((prev) => [newCustomer, ...prev]);

                // Set the new customer as selected
                setSelectedCustomer(newCustomer);

                // Set the customer ID in the form
                form.setFieldValue("customerId", newCustomer.id);

                // Close the modal
                setAddCustomerModalVisible(false);
            } else {
                showMessage("error", result.message || "Lỗi khi tạo khách hàng");
            }
        } catch (error) {
            console.error("Error creating customer:", error);
            showMessage("error", "Lỗi khi tạo khách hàng");
        } finally {
            setSpinning(false);
        }
    };

    // Add these helper functions before the return statement
    const calculateSubTotal = useCallback(() => {
        return products.reduce((sum, product) => {
            const price = product.price;
            const discount = product.discountPercent || 0;
            const discountedPrice = price - (price * discount) / 100;

            let totalPrice = 0;

            if (product.promotedQuantity && product.promotedQuantity > 0) {
                // Calculate price for promoted quantity
                totalPrice += discountedPrice * product.promotedQuantity;

                // Add price for non-promoted quantity if exists
                if (product.nonPromotedQuantity && product.nonPromotedQuantity > 0) {
                    totalPrice += price * product.nonPromotedQuantity;
                }
            } else {
                // No promotion split, calculate normally
                totalPrice = discountedPrice * product.quantity;
            }
            return sum + totalPrice;
        }, 0);
    }, [products]);

    useEffect(() => {
        if (!discountCodeResponse) return;
        if (discountCodeResponse.status === 1 && discountCodeResponse.data) {
            const discountCodeData = discountCodeResponse.data;
            const amount = calculateSubTotal();
            if (discountCodeData.minOder > amount) {
                setDiscountCodeError("Đơn hàng chưa đạt giá trị tối thiểu để sử dụng mã giảm giá");
                setDiscountCodeAmount(0);
                return;
            }
            if (discountCodeData.type == 0) {
                // giảm giá theo %
                let discountAmount = (amount * discountCodeData.discount) / 100;
                if (discountCodeData.maxDiscount && discountAmount > discountCodeData.maxDiscount) {
                    discountAmount = discountCodeData.maxDiscount;
                }
                const roundedThousands = Math.round(discountAmount / 100) * 100;
                setDiscountCodeAmount(roundedThousands);
            } else {
                // giảm giá theo số tiền
                const roundedThousands = Math.round(Math.min(discountCodeData.discount, amount) / 100) * 100;
                setDiscountCodeAmount(roundedThousands);
            }
            setDiscountCodeError(null);
        } else {
            if (discountCodeResponse.message === "NOT_FOUND" || discountCodeResponse.message === "EXPIRED") {
                setDiscountCodeError("Không tìm thấy mã giảm giá này. Vui lòng kiểm tra lại mã và hạn sử dụng");
            } else if (discountCodeResponse.message === "OUT_OF_USE") {
                setDiscountCodeError("Hết lượt sử dụng mã giảm giá");
            } else {
                setDiscountCodeError(discountCodeResponse.message || "Lỗi khi áp dụng mã giảm giá");
            }
        }
    }, [calculateSubTotal, discountCodeResponse]);

    const calcPointCanUse = useCallback(() => {
        if (selectedCustomer) {
            return Math.min(
                selectedCustomer.remainingPoint,
                Math.round((calculateSubTotal() + (Number(shippingFee) || 0) - minusMembership - discountCodeAmount) / amountPayRate)
            );
        }
        return 0;
    }, [selectedCustomer, calculateSubTotal, shippingFee, minusMembership, amountPayRate, discountCodeAmount]);

    const calcPointUse = useCallback(() => {
        if (usePoint && selectedCustomer) {
            return Math.min(
                selectedCustomer.remainingPoint,
                Math.round((calculateSubTotal() + (Number(shippingFee) || 0) - minusMembership - discountCodeAmount) / amountPayRate)
            );
        }
        return 0;
    }, [usePoint, selectedCustomer, calculateSubTotal, shippingFee, minusMembership, amountPayRate, discountCodeAmount]);

    const calculateTotal = useCallback(() => {
        const subTotal = calculateSubTotal();
        const pointsValue = calcPointUse() * amountPayRate;
        return subTotal + Number(shippingFee) - minusMembership - pointsValue - discountCodeAmount;
    }, [calculateSubTotal, calcPointUse, shippingFee, minusMembership, amountPayRate, discountCodeAmount]);

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
            // Cancel any previous request
            if (abortControllerRef.current) {
                abortControllerRef.current.abort();
            }

            // Create new AbortController for this request
            abortControllerRef.current = new AbortController();

            try {
                if (orderType === 2 && !selectedCustomer) {
                    setOrderReceipt(null);
                    return;
                }
                if (products.length === 0) {
                    setOrderReceipt(null);
                    return;
                }
                const deliveryInfo = form.getFieldValue("delivery");
                const addressParts = [
                    deliveryInfo?.consigneeAddress,
                    deliveryInfo?.communeId ? communes.find((c) => c.id === deliveryInfo?.communeId)?.name : "",
                    deliveryInfo?.districtId ? districts.find((d) => d.id === deliveryInfo?.districtId)?.name : "",
                    deliveryInfo?.provinceId ? provinces.find((p) => p.id === deliveryInfo?.provinceId)?.name : ""
                ].filter(Boolean);
                let customerAddress = addressParts.length > 0 ? addressParts.join(", ") : null;

                const total = calculateTotal();
                const pointReceive = Math.round(((total - Number(shippingFee)) * ratePurchase) / 100);

                let qrBase64Image = null;
                if (bankInfo && total > 0) {
                    // Pass the signal to fetch
                    const response = await fetch(
                        `https://img.vietqr.io/image/${bankInfo.bin}-${
                            bankInfo.accountNumber
                        }-qr_only.png?amount=${total}&addInfo=Thanh toán đơn hàng ${detailsItem?.orderCode || ""}`,
                        { signal: abortControllerRef.current.signal }
                    );
                    const blob = await response.blob();
                    qrBase64Image = await blobToBase64(blob);
                }
                // Check if request was aborted before setting state
                if (!abortControllerRef.current.signal.aborted) {
                    setOrderReceipt({
                        orderCode: detailsItem?.orderCode,
                        orderTime: form.getFieldValue("orderTime"),
                        customerName: form.getFieldValue(["delivery", "consigneeName"]) || selectedCustomer?.name,
                        customerPhone: form.getFieldValue(["delivery", "consigneePhone"]) || selectedCustomer?.phone,
                        products: products.map((p) => ({
                            id: `${p.productId}-${p.comboId}`,
                            sku: p.sku || "",
                            name: p.name,
                            price: p.price,
                            quantity: p.quantity,
                            unitName: p.unitName || "",
                            discountPercent: p.discountPercent || 0,
                            promotedQuantity: p.promotedQuantity || 0,
                            nonPromotedQuantity: p.nonPromotedQuantity || 0
                        })),
                        shippingFee: Number(shippingFee),
                        minusPoints: calcPointUse(),
                        minusPointsValue: calcPointUse() * amountPayRate,
                        membershipValue: minusMembership,
                        discountValue: discountCodeAmount,
                        pointReceive: pointReceive,
                        total: total,
                        consigneeAddress: customerAddress || undefined,
                        userName: user?.name || "",
                        qrBase64Image: qrBase64Image || undefined
                    });
                }
            } catch (error) {
                // Only log error if it's not an abort error
                if (error instanceof Error && error.name !== "AbortError") {
                    console.error("Error in fetchOrderReceipt:", error);
                }
            }
        };

        fetchOrderReceipt();

        // Cleanup function to abort any pending request when dependencies change
        return () => {
            if (abortControllerRef.current) {
                abortControllerRef.current.abort();
                abortControllerRef.current = null;
            }
        };
    }, [
        orderType,
        selectedCustomer,
        products,
        communes,
        districts,
        provinces,
        shippingFee,
        ratePurchase,
        bankInfo,
        detailsItem,
        form,
        calculateTotal,
        getBillQrBase64Image,
        calcPointUse,
        amountPayRate,
        minusMembership,
        user?.name,
        discountCodeAmount
    ]);

    useEffect(() => {
        const canMinusMembershipDiscount = (total: number) => {
            if (!selectedCustomer || !selectedCustomer.membershipId) return false;
            if (selectedCustomer.membershipOtherProgram) return total >= selectedCustomer.membershipMinTotalAmount;
            if (products.some((product) => !!product.promotionId)) return false;
            if (total < selectedCustomer.membershipMinTotalAmount) return false;
            return true;
        };
        const total = calculateSubTotal() - discountCodeAmount;
        if (selectedCustomer && canMinusMembershipDiscount(total)) {
            setMinusMembership(Math.round((total * selectedCustomer.membershipDiscount) / 100));
        } else {
            setMinusMembership(0);
        }
    }, [selectedCustomer, calculateSubTotal, discountCodeAmount, products]);

    useEffect(() => {
        setUsePoint(usePoint && !!selectedCustomer);
    }, [selectedCustomer]);

    const handleShippingFeeChange = (value: string) => {
        // Remove non-numeric characters and leading zeros
        const numericValue = value.replace(/[^0-9]/g, "").replace(/^0+/, "") || "";
        setShippingFee(numericValue);
    };

    // Update the initialization effect
    useEffect(() => {
        if (detailsItem) {
            form.setFieldsValue({
                type: detailsItem.type,
                orderTime: dayjs(detailsItem.orderTime),
                customerId: detailsItem.customer?.id,
                delivery: {
                    consigneeName: detailsItem.delivery.consigneeName || "",
                    consigneePhone: detailsItem.delivery.consigneePhone || "",
                    provinceId: detailsItem.delivery.provinceId || undefined,
                    districtId: detailsItem.delivery.districtId || undefined,
                    communeId: detailsItem.delivery.communeId || undefined,
                    consigneeLat: detailsItem.delivery.consigneeLat?.toString(),
                    consigneeLong: detailsItem.delivery.consigneeLong?.toString(),
                    consigneeAddress: detailsItem.delivery.consigneeAddress || "",
                    consigneeNote: detailsItem.delivery.consigneeNote || ""
                }
            });

            // Initialize location data if available
            if (detailsItem.delivery.provinceId) {
                fetchProvinces("", 1, false, detailsItem.delivery.provinceId).then(() => {
                    if (detailsItem.delivery.districtId) {
                        fetchDistricts(detailsItem.delivery.provinceId || undefined).then(() => {
                            if (detailsItem.delivery.communeId) {
                                fetchCommunes(detailsItem.delivery.districtId || undefined);
                            }
                        });
                    }
                });
            }

            // Initialize customer data
            if (detailsItem.customer) {
                const customer = {
                    id: detailsItem.customer.id,
                    name: detailsItem.customer.name,
                    phone: detailsItem.customer.phone,
                    userId: detailsItem.customer.userId || -1,
                    remainingPoint: detailsItem.customer.remainingPoint || 0,
                    membershipId: detailsItem.customer.membershipId || null,
                    membershipName: detailsItem.customer.membershipName,
                    membershipDiscount: detailsItem.customer.membershipDiscount || 0,
                    membershipMinTotalAmount: detailsItem.customer.membershipMinTotalAmount || 0,
                    membershipOtherProgram: detailsItem.customer.membershipOtherProgram
                };
                setCustomers([customer]);
                setSelectedCustomer(customer);
            }

            // Set shipping fee and points if available
            setShippingFee(detailsItem.shippingFee.toString());
            if (detailsItem.minusPoints) {
                setUsePoint(true);
            }
            if (detailsItem.discountCode) {
                setDiscountCode(detailsItem.discountCode.code);
                setDiscountCodeResponse({
                    status: 1,
                    data: {
                        id: detailsItem.discountCode.id,
                        code: detailsItem.discountCode.code,
                        type: detailsItem.discountCode.type,
                        discount: detailsItem.discountCode.discount,
                        minOder: detailsItem.discountCode.minOder,
                        maxDiscount: detailsItem.discountCode.maxDiscount || undefined
                    }
                });
            }
            if (detailsItem.isHasChanges) {
                showConfirmModal({
                    title: "Thông báo",
                    content: "Dữ liệu sản phẩm đã bị thay đổi (có thể do thay đổi giá, hết hàng, sản phẩm không tồn tại,..), vui lòng kiểm tra lại",
                    disableCancel: true
                });
            }
        }
    }, [detailsItem]);
    return (
        <div className={`content-background ${styles["layout-container"]}`}>
            <Container fluid className="">
                <Breadcrumb
                    className={`${styles["bread-crumb"]}`}
                    items={[
                        {
                            title: "Quản lý hệ thống"
                        },
                        {
                            title: <a className="current">{!!detailsItem ? "Cập nhật đơn hàng" : "Thêm mới đơn hàng"}</a>
                        }
                    ]}
                />
                <div className="group-breadcumb">
                    <Button className={`btn-reset w-100px`} icon={<img src="/images/actions/ic-close.svg" />} onClick={handleCancel}>
                        Hủy
                    </Button>
                    <Button onClick={() => handleSave(true)} className={`btn-reset`} icon={<img src="/images/actions/photo_album.svg" />}>
                        Lưu tạm
                    </Button>
                    <Button onClick={() => handleSave(false)} className={`btn-save`} icon={<img src="/images/actions/icon-check.svg" />}>
                        Lưu
                    </Button>
                    {hasPermission("ORDER:POS_ORDER:PRINT") && (
                        <Button className={`btn-reset`} icon={<img src="/images/actions/print_connect.svg" />} onClick={handlePrintOrder}>
                            In đơn
                        </Button>
                    )}
                    <Button className={`btn-back`} icon={<img src="/images/actions/icon-back.svg" />} onClick={handleGoBack}>
                        Quay lại
                    </Button>
                </div>

                <Form
                    form={form}
                    layout="vertical"
                    requiredMark={formRequiredMark}
                    initialValues={{
                        type: detailsItem?.type || 0, // Default to "Giao trực tiếp"
                        orderTime: currentDateTime,
                        customerId: detailsItem?.customer?.id || undefined,
                        delivery: {
                            consigneeName: detailsItem?.delivery.consigneeName || "",
                            consigneePhone: detailsItem?.delivery.consigneePhone || "",
                            provinceId: detailsItem?.delivery.provinceId || undefined,
                            districtId: detailsItem?.delivery.districtId || undefined,
                            communeId: detailsItem?.delivery.communeId || undefined,
                            consigneeLat: detailsItem?.delivery.consigneeLat?.toString(),
                            consigneeLong: detailsItem?.delivery.consigneeLong?.toString(),
                            consigneeAddress: detailsItem?.delivery.consigneeAddress || "",
                            consigneeNote: detailsItem?.delivery.consigneeNote || ""
                        }
                    }}
                >
                    <div
                        style={{
                            padding: 16,
                            background: "white",
                            borderRadius: 16,
                            marginBottom: 16
                        }}
                    >
                        <div style={{ marginBottom: 16 }}>
                            <div className="txt-18-n-600" style={{ marginBottom: 8 }}>
                                Thông tin chung
                            </div>
                        </div>

                        <Container fluid className="px-0">
                            <Row className="g-4">
                                <Col lg={4}>
                                    <Form.Item
                                        label="Loại"
                                        name="type"
                                        rules={[{ required: true, message: "Trường dữ liệu này không được để trống" }]}
                                    >
                                        <Select options={typeOptions} showSearch onChange={handleTypeChange} />
                                    </Form.Item>
                                </Col>

                                <Col lg={4}>
                                    <Form.Item
                                        label="Thời gian đặt hàng"
                                        name="orderTime"
                                        rules={[
                                            { required: true, message: "Trường dữ liệu này không được để trống" },
                                            {
                                                validator: async (_, value) => {
                                                    if (value && value.isAfter(dayjs())) {
                                                        throw new Error("Thời gian không được lớn hơn hiện tại");
                                                    }
                                                }
                                            }
                                        ]}
                                    >
                                        <DatePicker
                                            showTime
                                            format="HH:mm - DD/MM/YYYY"
                                            style={{ width: "100%" }}
                                            disabledDate={(current) => current && current.isAfter(dayjs())}
                                            disabledTime={(current) => {
                                                if (current && current.isSame(dayjs(), "date")) {
                                                    const currentHour = dayjs().hour();
                                                    const currentMinute = dayjs().minute();

                                                    return {
                                                        disabledHours: () => Array.from({ length: 24 }, (_, i) => i).filter((h) => h > currentHour),
                                                        disabledMinutes: (selectedHour) =>
                                                            selectedHour === currentHour
                                                                ? Array.from({ length: 60 }, (_, i) => i).filter((m) => m > currentMinute)
                                                                : []
                                                    };
                                                }
                                                return {};
                                            }}
                                        />
                                    </Form.Item>
                                </Col>

                                <Col lg={4}>
                                    <Form.Item label="Tên khách hàng" required={orderType === 2}>
                                        <div style={{ display: "flex", gap: 4 }}>
                                            <Form.Item
                                                name="customerId"
                                                rules={[{ required: orderType === 2, message: "Trường dữ liệu này không được để trống" }]}
                                                noStyle
                                            >
                                                <Select
                                                    options={customers.map((c) => ({
                                                        value: c.id,
                                                        label: formatCustomerLabel(c)
                                                    }))}
                                                    style={{ flex: 1 }}
                                                    showSearch
                                                    filterOption={false}
                                                    onSearch={debouncedCustomerSearch}
                                                    onPopupScroll={handleCustomerScroll}
                                                    loading={customersLoading}
                                                    placeholder="Chọn khách hàng..."
                                                    notFoundContent={
                                                        customersLoading ? (
                                                            <Spin size="small" />
                                                        ) : (
                                                            <div style={{ textAlign: "center", padding: "8px 0" }}>Không có dữ liệu phù hợp</div>
                                                        )
                                                    }
                                                    onChange={(value) => {
                                                        // Find the selected customer
                                                        const customer = customers.find((c) => c.id === value);
                                                        setSelectedCustomer(customer || null);

                                                        // Reset default address checkbox if customer has no account
                                                        if (customer && customer.userId === -1) {
                                                            setUseDefaultAddress(false);
                                                        }
                                                    }}
                                                    allowClear
                                                />
                                            </Form.Item>
                                            <img src="/images/actions/add-gray.svg" onClick={handleAddNewCustomer} />
                                        </div>
                                    </Form.Item>
                                </Col>
                            </Row>
                        </Container>
                    </div>

                    {/* DELIVERY INFORMATION */}
                    {orderType === 2 && (
                        <div
                            style={{
                                padding: 16,
                                background: "white",
                                borderRadius: 16,
                                marginBottom: 16
                            }}
                        >
                            <div style={{ display: "flex", alignItems: "center", marginBottom: 16 }}>
                                <div className="txt-18-n-600">Thông tin nhận hàng</div>
                                <Checkbox
                                    style={{ marginLeft: 16 }}
                                    checked={useDefaultAddress}
                                    onChange={handleDefaultAddressChange}
                                    disabled={!selectedCustomer || selectedCustomer.userId === -1}
                                >
                                    <span className="txt-16-n-400" style={{ color: selectedCustomer?.userId === -1 ? "#999" : "#22313F" }}>
                                        Giao đến địa chỉ mặc định
                                    </span>
                                </Checkbox>
                            </div>

                            <Container fluid className="px-0">
                                <Row className="g-4 mb-4" style={{ marginBottom: "16px" }}>
                                    <Col lg={4}>
                                        <Form.Item
                                            label="Tên người nhận"
                                            name={["delivery", "consigneeName"]}
                                            rules={[{ required: true, message: "Trường dữ liệu này không được để trống" }]}
                                        >
                                            <Input placeholder="Nhập" maxLength={255} />
                                        </Form.Item>
                                    </Col>

                                    <Col lg={4}>
                                        <Form.Item
                                            label="Số điện thoại"
                                            name={["delivery", "consigneePhone"]}
                                            rules={[
                                                { required: true, message: "Trường dữ liệu này không được để trống" },
                                                { pattern: /^0\d{9,10}$/, message: "Sai định dạng" }
                                            ]}
                                        >
                                            <Input placeholder="Nhập" />
                                        </Form.Item>
                                    </Col>
                                </Row>

                                <Row className="g-4" style={{ marginBottom: "16px" }}>
                                    <Col lg={4}>
                                        <Form.Item
                                            label="Tỉnh/Thành phố"
                                            name={["delivery", "provinceId"]}
                                            rules={orderType === 2 ? [{ required: true, message: `Trường dữ liệu này không được để trống` }] : []}
                                        >
                                            <Select
                                                options={provinces.map((p) => ({ value: p.id, label: p.name }))}
                                                placeholder="Chọn tỉnh/thành phố..."
                                                showSearch
                                                filterOption={false}
                                                onSearch={debouncedProvinceSearch}
                                                onPopupScroll={handleProvinceScroll}
                                                onChange={handleProvinceChange}
                                                loading={provincesLoading}
                                                notFoundContent={
                                                    provincesLoading ? (
                                                        <Spin size="small" />
                                                    ) : (
                                                        <div style={{ textAlign: "center", padding: "8px 0" }}>Không có dữ liệu phù hợp</div>
                                                    )
                                                }
                                                allowClear
                                                onClear={() => {
                                                    fetchProvinces();
                                                }}
                                            />
                                        </Form.Item>
                                    </Col>

                                    <Col lg={4}>
                                        <Form.Item
                                            label="Quận/Huyện"
                                            name={["delivery", "districtId"]}
                                            rules={orderType === 2 ? [{ required: true, message: `Trường dữ liệu này không được để trống` }] : []}
                                        >
                                            <Select
                                                options={districts.map((d) => ({ value: d.id, label: d.name }))}
                                                placeholder={"Chọn quận/huyện..."}
                                                disabled={!selectedProvinceId}
                                                showSearch
                                                filterOption={false}
                                                onSearch={debouncedDistrictSearch}
                                                onChange={handleDistrictChange}
                                                loading={districtsLoading}
                                                notFoundContent={
                                                    districtsLoading ? (
                                                        <Spin size="small" />
                                                    ) : (
                                                        <div style={{ textAlign: "center", padding: "8px 0" }}>Không có dữ liệu phù hợp</div>
                                                    )
                                                }
                                                allowClear
                                            />
                                        </Form.Item>
                                    </Col>

                                    <Col lg={4}>
                                        <Form.Item
                                            label="Phường/Xã"
                                            name={["delivery", "communeId"]}
                                            rules={orderType === 2 ? [{ required: true, message: `Trường dữ liệu này không được để trống` }] : []}
                                        >
                                            <Select
                                                options={communes.map((c) => ({ value: c.id, label: c.name }))}
                                                placeholder={"Chọn phường/xã..."}
                                                disabled={!selectedDistrictId || !selectedProvinceId}
                                                showSearch
                                                filterOption={false}
                                                onSearch={debouncedCommuneSearch}
                                                loading={communesLoading}
                                                notFoundContent={
                                                    communesLoading ? (
                                                        <Spin size="small" />
                                                    ) : (
                                                        <div style={{ textAlign: "center", padding: "8px 0" }}>Không có dữ liệu phù hợp</div>
                                                    )
                                                }
                                                allowClear
                                            />
                                        </Form.Item>
                                    </Col>
                                </Row>

                                <Row className="g-4">
                                    <Col lg={8}>
                                        <Form.Item
                                            label="Địa chỉ chi tiết"
                                            name={["delivery", "consigneeAddress"]}
                                            rules={orderType === 2 ? [{ required: true, message: `Trường dữ liệu này không được để trống` }] : []}
                                        >
                                            <Input placeholder="Nhập" maxLength={200} />
                                        </Form.Item>
                                    </Col>
                                    <Col lg={4}>
                                        <Form.Item label="Ghi chú" name={["delivery", "consigneeNote"]}>
                                            <Input placeholder="Nhập" maxLength={500} />
                                        </Form.Item>
                                    </Col>
                                </Row>
                            </Container>
                        </div>
                    )}

                    <div
                        style={{
                            display: "flex",
                            // paddingLeft: 16,
                            // paddingRight: 16,
                            marginBottom: 8,
                            gap: 16
                        }}
                    >
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexGrow: 1 }}>
                            <div style={{ display: "flex", alignItems: "center", gap: 16, paddingLeft: 8 }}>
                                {selectedProducts.length > 0 && (
                                    <Tooltip title="Xóa sản phẩm đã chọn">
                                        <img
                                            src="/images/actions/delete-action-2.svg"
                                            alt="Xóa"
                                            style={{ cursor: "pointer" }}
                                            onClick={handleRemoveSelectedProducts}
                                        />
                                    </Tooltip>
                                )}
                                <span className="txt-18-n-500">Danh sách sản phẩm ({products.length})</span>
                                <Popover
                                    arrow={false}
                                    content={productSearchDropdown}
                                    open={searchDropdownVisible}
                                    trigger="click"
                                    placement="bottom"
                                    styles={{ root: { padding: 0 } }}
                                    // overlayStyle={{ padding: 0 }}
                                    destroyTooltipOnHide
                                >
                                    <div className={styles["search-input"]}>
                                        <Search
                                            id="product-search-input"
                                            placeholder="Nhập barcode, mã sản phẩm, tên sản phẩm để tìm kiếm"
                                            value={productSearchKeyword}
                                            onChange={(e) => {
                                                setProductSearchKeyword(e.target.value);
                                                debouncedProductSearch(e.target.value); // Search on typing
                                            }}
                                            onClick={() => {
                                                // Always show dropdown when clicking search
                                                setSearchDropdownVisible(true);
                                                // Trigger search if we have a keyword
                                                if (productSearchKeyword.trim()) {
                                                    debouncedProductSearch(productSearchKeyword);
                                                }
                                                // Load initial data if we haven't searched yet
                                                if (!searchResults.length && !searchingProduct) {
                                                    setProductPage(1);
                                                    fetchProducts("", 1); // Empty search to get initial products
                                                }
                                            }}
                                            onSearch={(value) => {
                                                if (value.trim()) {
                                                    // Try exact match first
                                                    fetch("/api/order-at-counter/product/search", {
                                                        method: "POST",
                                                        headers: {
                                                            "Content-Type": "application/json"
                                                        },
                                                        body: JSON.stringify({
                                                            keyword: value.trim(),
                                                            pageIndex: 0,
                                                            pageSize: 10
                                                        })
                                                    })
                                                        .then((response) => response.json())
                                                        .then((result) => {
                                                            if (result.status === 1 && result.data.items.length > 0) {
                                                                const searchValue = value.trim().toLowerCase();
                                                                const exactMatch = result.data.items.find(
                                                                    (item: ProductSearchItem) =>
                                                                        item.sku?.toLowerCase() === searchValue ||
                                                                        item.barcode?.toLowerCase() === searchValue
                                                                );

                                                                if (exactMatch) {
                                                                    // Found exact match, add to table
                                                                    addProductToTable(exactMatch);
                                                                    setProductSearchKeyword("");
                                                                    setSearchDropdownVisible(false);
                                                                    return;
                                                                }
                                                            }
                                                            // No exact match, show search results
                                                            debouncedProductSearch(value);
                                                            setSearchDropdownVisible(true);
                                                        })
                                                        .catch((error) => {
                                                            console.error("Error searching product:", error);
                                                            showMessage("error", "Lỗi khi tìm sản phẩm");
                                                        });
                                                }
                                            }}
                                            onClear={() => {
                                                setProductSearchKeyword("");
                                                setSearchResults([]);
                                                setSearchDropdownVisible(false);
                                            }}
                                            loading={searchingProduct}
                                            className={styles["custom-search"]}
                                            style={{
                                                width: 480
                                            }}
                                        />
                                    </div>
                                </Popover>
                            </div>
                        </div>
                        <div
                            style={{
                                width: 368,
                                height: 36
                            }}
                        ></div>
                    </div>

                    {/* PRODUCT LIST */}
                    <div
                        style={{
                            display: "flex",
                            alignItems: "start",
                            // paddingLeft: 16,
                            // paddingRight: 16,
                            marginBottom: 8,
                            gap: 16
                        }}
                    >
                        <div className={styles.productList}>
                            <DataTable
                                value={products}
                                emptyMessage={<Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />}
                                stripedRows
                                rowHover
                                style={{ width: "100%" }}
                                selectionMode="checkbox"
                                selection={selectedProducts}
                                onSelectionChange={(e) => setSelectedProducts(e.value)}
                                dataKey="id"
                            >
                                <Column selectionMode="multiple" headerStyle={{ width: "40px" }} />
                                <Column field="sku" header="Mã sản phẩm" body={skuTemplate} style={{ width: "136px" }} />
                                <Column header="Ảnh sản phẩm" body={imageTemplate} style={{ width: "116px" }} />
                                <Column field="name" header="Tên sản phẩm" />
                                <Column field="unitName" header="Đơn vị tính" align={"right"} style={{ width: "116px" }} />
                                <Column field="price" header="Đơn giá (đ)" body={priceTemplate} style={{ width: "116px" }} align={"right"} />
                                <Column field="quantity" header="Số lượng" body={quantityTemplate} align={"center"} style={{ width: "116px" }} />
                                <Column
                                    field="totalPrice"
                                    header="Thành tiền (đ)"
                                    body={totalPriceTemplate}
                                    align={"right"}
                                    style={{ width: "116px" }}
                                />
                                <Column header="Thao tác" body={actionTemplate} align={"right"} style={{ width: "80px" }} />
                            </DataTable>
                        </div>

                        <div className={styles.summary}>
                            <div style={{ alignSelf: "stretch", justifyContent: "flex-end", alignItems: "center", gap: 16, display: "inline-flex" }}>
                                <div className="txt-14-n-400" style={{ flex: "1 1 0", color: "#22313F" }}>
                                    Tổng tiền
                                </div>
                                <div className="txt-14-n-500" style={{ textAlign: "right", color: "#22313F" }}>
                                    {calculateSubTotal().toLocaleString("vi-VN")} đ
                                </div>
                            </div>
                            <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />

                            <div
                                className={styles.shippingFee}
                                style={{
                                    alignSelf: "stretch",
                                    justifyContent: "flex-end",
                                    alignItems: "center",
                                    gap: 16,
                                    display: "inline-flex"
                                }}
                            >
                                <div className="txt-14-n-400" style={{ flex: "1 1 0", color: "#22313F" }}>
                                    Phí vận chuyển
                                </div>
                                {orderType === 2 ? (
                                    <CurrencyInput
                                        placeholder="Nhập..."
                                        maxLength={13}
                                        value={shippingFee}
                                        onChange={(value) => handleShippingFeeChange(value)}
                                        style={{ width: 160 }}
                                    />
                                ) : (
                                    <div className="txt-14-n-500" style={{ textAlign: "right", color: "#22313F" }}>
                                        {Number(shippingFee).toLocaleString("vi-VN")} đ
                                    </div>
                                )}
                            </div>
                            <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />
                            <div className="txt-14-n-500" style={{ flex: "1 1 0", color: "#22313F" }}>
                                Mã giảm giá
                            </div>
                            <div className={styles.couponCode}>
                                <Input
                                    value={discountCode}
                                    placeholder="Nhập mã giảm giá"
                                    maxLength={20}
                                    allowClear
                                    onChange={(e) => {
                                        if (discountCodeResponse) {
                                            return;
                                        }
                                        setDiscountCode(e.target.value.replace(/[^A-Z0-9]/gi, "").toUpperCase());
                                    }}
                                    onClear={() => {
                                        setDiscountCodeResponse(null);
                                        setDiscountCode("");
                                        setDiscountCodeError(null);
                                        setDiscountCodeAmount(0);
                                    }}
                                />
                                <Button
                                    className={styles.btnApply}
                                    disabled={discountCode.length === 0 || !!discountCodeResponse}
                                    onClick={checkDiscountCode}
                                >
                                    Áp dụng
                                </Button>
                            </div>
                            {discountCodeError && (
                                <div className="txt-12-i-400" style={{ color: "#FF3D32" }}>
                                    {discountCodeError}
                                </div>
                            )}
                            {discountCodeAmount > 0 && (
                                <>
                                    <DiscountMiniSummaryView
                                        discountCodeAmount={discountCodeAmount}
                                        onClear={() => {
                                            setDiscountCodeResponse(null);
                                            setDiscountCode("");
                                            setDiscountCodeError(null);
                                            setDiscountCodeAmount(0);
                                        }}
                                    />
                                    <div
                                        style={{
                                            alignSelf: "stretch",
                                            justifyContent: "flex-end",
                                            alignItems: "center",
                                            gap: 16,
                                            display: "inline-flex"
                                        }}
                                    >
                                        <div className="txt-14-n-400" style={{ flex: "1 1 0", color: "#22313F" }}>
                                            Giảm giá/ tổng đơn hàng:
                                        </div>
                                        <div className="txt-14-n-500" style={{ textAlign: "right", color: "#FF3D32" }}>
                                            -{discountCodeAmount.toLocaleString("vi-VN")} đ
                                        </div>
                                    </div>
                                </>
                            )}

                            {/* <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />
                            <div
                                style={{
                                    display: "flex",
                                    flexDirection: "column",
                                    gap: 8,
                                    alignItems: "start"
                                }}
                            >
                                <div>Mã giảm giá</div>
                                <div style={{ color: "#008847", cursor: "pointer" }} onClick={() => setSelectCouponModalVisible(true)}>
                                    Chọn mã giảm giá
                                </div>
                            </div> */}
                            {selectedCustomer && minusMembership > 0 && (
                                <>
                                    <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />
                                    <div
                                        style={{
                                            alignSelf: "stretch",
                                            justifyContent: "flex-end",
                                            alignItems: "center",
                                            gap: 16,
                                            display: "inline-flex"
                                        }}
                                    >
                                        <div className="txt-14-n-400" style={{ flex: "1 1 0", color: "#22313F" }}>
                                            Hạng {selectedCustomer.membershipName}
                                            <img
                                                style={{ width: 20, height: 20, marginLeft: 10 }}
                                                src={mapMembershipIcon.get(selectedCustomer.membershipName)}
                                            />
                                        </div>
                                        <div className="txt-14-n-500" style={{ textAlign: "right", color: "#FF3D32" }}>
                                            -{minusMembership.toLocaleString("vi-VN")} ₫
                                        </div>
                                    </div>
                                </>
                            )}
                            {selectedCustomer && selectedCustomer.remainingPoint > 0 && calcPointCanUse() > 0 && (
                                <>
                                    <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />
                                    <div
                                        style={{
                                            alignSelf: "stretch",
                                            justifyContent: "flex-end",
                                            alignItems: "center",
                                            gap: 16,
                                            display: "inline-flex"
                                        }}
                                    >
                                        <div className="txt-14-n-400" style={{ flex: "1 1 0", color: "#22313F" }}>
                                            {"Dùng "}
                                            <span className="txt-14-n-500" style={{ color: "#FF3D32" }}>
                                                {Math.min(
                                                    selectedCustomer.remainingPoint,
                                                    Math.round((calculateSubTotal() + (Number(shippingFee) || 0) - minusMembership) / rateAmountPay)
                                                ).toLocaleString("vi-VN")}
                                            </span>
                                            {" điểm"}
                                            <Switch checked={usePoint} onChange={(checked) => setUsePoint(checked)} style={{ marginLeft: 4 }} />
                                        </div>
                                        <div className="txt-14-n-500" style={{ textAlign: "right", color: "#FF3D32" }}>
                                            -{(calcPointUse() * amountPayRate).toLocaleString("vi-VN")} ₫
                                        </div>
                                    </div>
                                </>
                            )}

                            <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />

                            <div style={{ alignSelf: "stretch", justifyContent: "flex-end", alignItems: "center", gap: 16, display: "inline-flex" }}>
                                <div style={{ flex: "1 1 0", color: "#22313F", fontSize: 14 }}>Thành tiền</div>
                                <div style={{ textAlign: "right", color: "#22313F", fontSize: 14, fontWeight: "500" }}>
                                    {calculateTotal().toLocaleString("vi-VN")} đ
                                </div>
                            </div>

                            <div style={{ width: "100%", height: 1, background: "#E9EAEC" }} />

                            <div style={{ alignSelf: "stretch", flexDirection: "column", gap: 16, display: "flex" }}>
                                <div style={{ color: "#22313F", fontSize: 14 }}>Hình thức thanh toán</div>
                                <Radio.Group
                                    value={paymentType}
                                    onChange={(e) => setPaymentType(e.target.value)}
                                    style={{ display: "flex", gap: 16 }}
                                >
                                    <Radio.Button value={0} style={{ flex: 1, textAlign: "center" }}>
                                        Tiền mặt
                                    </Radio.Button>
                                    <Radio.Button value={1} style={{ flex: 1, textAlign: "center" }}>
                                        Chuyển khoản
                                    </Radio.Button>
                                </Radio.Group>
                            </div>
                        </div>
                    </div>
                </Form>
            </Container>
            <AddCustomerModal
                isVisible={addCustomerModalVisible}
                onClose={handleCloseAddCustomerModal}
                setLoading={setSpinning}
                onCreated={handleAddCustomer}
                showMessage={showMessage}
            />
            {/* <SelectCouponModal isVisible={selectCouponModalVisible} onClose={handleCloseSelectCouponModal} /> */}
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
        </div>
    );
};
