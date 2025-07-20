import { useMainLayout } from "@/layouts/MainLayout";
import { Button, Form, Input, Modal } from "antd";

interface AddCustomerModalProps {
    isVisible: boolean;
    setLoading: (loading: boolean) => void;
    onCreated: (name: string, phone: string, createAccount: boolean) => void;
    onClose: () => void;
    showMessage: (type: any, message: string) => void;
}

type FormDataType = {
    name: string;
    phone: string;
};

const AddCustomerModal: React.FC<AddCustomerModalProps> = ({ isVisible, setLoading, onCreated, onClose, showMessage }) => {
    const { formRequiredMark } = useMainLayout();
    const handleSave = async (createAccount: boolean) => {
        const values = await form.validateFields();
        onCreated(values.name, values.phone, createAccount);
    };
    const [form] = Form.useForm<FormDataType>();

    const handleBlurFormInfo = (name: any, value: string) => {
        const trimmedValue = value.trim();
        form.setFieldValue(name, trimmedValue);
    };
    return (
        <Modal
            key={isVisible.toString()}
            open={isVisible}
            onCancel={onClose}
            // destroyOnClose
            centered
            width={"31.25%"}
            styles={{
                content: {
                    backgroundColor: "white",
                    borderRadius: "16px"
                }
            }}
            footer={[
                <Button key={"btn-cancel"} className={`btn-cancel`} icon={<img src="/images/actions/ic-close.svg" />} onClick={onClose}>
                    Hủy
                </Button>,
                <Button
                    key={"btn-save"}
                    className={`btn-save`}
                    icon={<img src="/images/actions/icon-check.svg" />}
                    onClick={() => {
                        handleSave(false);
                    }}
                >
                    Lưu
                </Button>,
                <Button
                    key={"btn-save"}
                    className={`btn-save`}
                    icon={<img src="/images/actions/add.svg" />}
                    onClick={() => {
                        handleSave(true);
                    }}
                >
                    Tạo tài khoản
                </Button>
            ]}
        >
            <div className="d-flex flex-column gap-4">
                <span className={`txt-20-n-600`}>Thêm mới khách hàng</span>
                <Form form={form} layout="vertical" requiredMark={formRequiredMark}>
                    <Form.Item
                        label="Họ tên"
                        name="name"
                        rules={[
                            {
                                required: true,
                                message: "Vui lòng nhập tên khách hàng"
                            }
                        ]}
                    >
                        <Input onBlur={(e) => handleBlurFormInfo("name", e.target.value)} placeholder="Nhập..." maxLength={255} />
                    </Form.Item>
                    <Form.Item
                        label="Điện thoại"
                        name="phone"
                        rules={[
                            {
                                required: true,
                                message: "Vui lòng nhập số điện thoại"
                            },
                            {
                                pattern: /^0\d{9,10}$/,
                                message: "Sai định dạng"
                            }
                        ]}
                    >
                        <Input onBlur={(e) => handleBlurFormInfo("phone", e.target.value)} placeholder="Nhập..." maxLength={255} />
                    </Form.Item>
                </Form>
            </div>
        </Modal>
    );
};

export default AddCustomerModal;
