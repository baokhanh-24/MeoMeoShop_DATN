import styles from "./DiscountMiniSummaryView.module.scss";

interface DiscountMiniSummaryViewProps {
    discountCodeAmount: number;
    onClear: () => void;
}

const DiscountMiniSummaryView = ({ discountCodeAmount, onClear }: DiscountMiniSummaryViewProps) => {
    const getDiscountCodeAmount = () => {
        if (discountCodeAmount > 0) {
            const roundedThousands = Math.round(discountCodeAmount / 100) / 10;
            return `${roundedThousands.toString().replace(".", ",")}k`;
        }
        return "0";
    };

    return (
        <div className={styles.wrapper}>
            <div className={styles.content}>
                <div className={styles.discountCodeAmount}>- Giảm {getDiscountCodeAmount()}</div>
                <img src="/images/actions/ic-close.svg" className={styles.clear} onClick={onClear} />
            </div>
        </div>
    );
};
export default DiscountMiniSummaryView;
