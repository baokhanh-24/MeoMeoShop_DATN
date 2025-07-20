import { Radio } from "antd";
import styles from "./Coupon.module.scss";

interface CouponProps {
    selected: boolean;
    onClick: () => void;
}

const Coupon = ({ selected, onClick }: CouponProps) => {
    return (
        <div className={`${styles.coupon}`} onClick={onClick}>
            {/* <div className={`${styles.ribbonLeft}`}>
                {Array.from({ length: 10 }).map((_, index) => (
                    <div key={index} className={`${styles.circle}`}></div>
                ))}
            </div> */}

            <div className={styles.left}>
                <div className={styles.logo}>🌸</div>
                <div className={styles.text}>Tất cả sản phẩm</div>
            </div>
            <div className={`${styles.right} ${selected ? styles.selected : ""}`}>
                <div className={styles.content}>
                    <span className={styles.title}>Giảm 50%</span>
                    <span className={styles.description}>Giảm tối đa 300k cho đơn hàng từ 2tr</span>
                </div>
                <span className={styles.expiryDate}>HSD: 20/11/2024 (12:00)</span>
            </div>
            <div className={styles.radio}>
                <Radio checked={selected} />
            </div>
        </div>
    );
};

export default Coupon;
