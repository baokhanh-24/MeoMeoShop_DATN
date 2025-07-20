import { Flex } from "antd";
import Image from "next/image";

interface DeliveryInfoViewProps {
    customerName: string;
    phoneNumber: string;
    address: string;
    note?: string;
}

const DeliveryInfoView = ({ customerName, phoneNumber, address, note }: DeliveryInfoViewProps) => {
    return (
        <Flex
            style={{
                padding: 16,
                background: "white",
                borderRadius: 16
            }}
            vertical
            gap={16}
        >
            <Flex align="center">
                <div className="txt-16-n-500">Thông tin nhận hàng</div>
            </Flex>

            <Flex vertical>
                <Flex justify="space-between" align="center" style={{ minHeight: 60 }}>
                    <div className="txt-14-n-400" style={{ width: 80 }}>
                        Họ tên
                    </div>
                    <div className="txt-14-n-500">{customerName}</div>
                </Flex>
                <div style={{ borderBottom: "1px solid #E9EAEC" }} />

                <Flex justify="space-between" align="center" style={{ minHeight: 60 }}>
                    <div className="txt-14-n-400" style={{ width: 80 }}>
                        Điện thoại
                    </div>
                    <div className="txt-14-n-500">{phoneNumber}</div>
                </Flex>
                <div style={{ borderBottom: "1px solid #E9EAEC" }} />

                <Flex justify="space-between" align="center" style={{ minHeight: 60 }}>
                    <div className="txt-14-n-400" style={{ width: 80, minWidth: 80 }}>
                        Địa chỉ
                    </div>
                    <div className="txt-14-n-500" style={{ textAlign: "right" }}>
                        {address}
                    </div>
                </Flex>
                <div style={{ borderBottom: "1px solid #E9EAEC" }} />

                <Flex justify="space-between" align="center" style={{ paddingTop: 16 }}>
                    <div className="txt-14-n-400" style={{ width: 80, minWidth: 80 }}>
                        Ghi chú
                    </div>
                    <div className="txt-14-n-500" style={{ width: 360, textAlign: "right" }}>
                        {note || ""}
                    </div>
                </Flex>
            </Flex>
        </Flex>
    );
};

export default DeliveryInfoView;
