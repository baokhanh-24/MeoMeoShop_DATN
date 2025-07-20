import { Button, Modal } from "antd";
import styles from "./SelectCouponModal.module.scss";
import { useState } from "react";
import Coupon from "./Coupon";
interface SelectCouponModalProps {
    isVisible: boolean;
    onClose: () => void;
}

const SelectCouponModal: React.FC<SelectCouponModalProps> = ({ isVisible, onClose }) => {
    const [selected, setSelected] = useState<number | null>(null);

    const coupons = [0, 1, 2, 3]; // Example coupons

    const handleSelect = (id: number) => {
        setSelected(selected === id ? null : id); // Toggle selection
    };

    return (
        <Modal
            open={isVisible}
            onCancel={onClose}
            footer={[
                <Button key={"btn-cancel"} className={`btn-cancel`} icon={<img src="/images/actions/ic-close.svg" />} onClick={onClose}>
                    Hủy
                </Button>,
                <Button key={"btn-create"} className={`btn-create`} icon={<img src="/images/actions/icon-check.svg" />} onClick={onClose}>
                    Áp dụng
                </Button>
            ]}
        >
            <span className={`txt-18-n-600`}>Mã giảm giá</span>
            <div className="d-flex flex-row justify-content-between mt-5">
                <p>Mã giảm giá/tổng đơn hàng</p>
                <span>Chọn tối đa 1 mã</span>
            </div>
            <div className={styles.couponList}>
                {coupons.map((id) => (
                    <Coupon key={id} selected={selected === id} onClick={() => handleSelect(id)} />
                ))}
            </div>
        </Modal>
    );
};

export default SelectCouponModal;
