import { Button, Form, InputNumber, DatePicker } from 'antd';
import { useState } from 'react';
import '../index.css'
const { Item } = Form;
const OrderForm = () => {
  const [form] = Form.useForm();
  const [customerId, setCustomerId] = useState(1);

  const onFinish = (values) => {
    const formatted = {
      customerId: customerId,
      quantityMachines: values.quantityMachines,
      requestedDate: values.requestedDate?.toISOString(),
      fullfiledDate: values.fullfiledDate?.toISOString(),
      deadline: values.deadline?.toISOString(),
    };
    console.log("Submitted order:", formatted);
  };

  return (
    <div className="min-h-screen flex items-center w-screen justify-center bg-gray-100 p-6">
      <div className="w-full max-w-xl bg-white rounded-2xl shadow-lg p-8">
        <h2 className="text-2xl font-semibold text-center text-gray-800 mb-6">Order Form</h2>
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{
            quantityMachines: null,
            requestedDate: null,
            fullfiledDate: null,
            deadline: null,
          }}
        >
          <div className="flex flex-col ju">
            <div className="flex w-full ">
              <Item
                label="Quantity Machines"
                name="quantityMachines"
                rules={[{ required: true, message: "Please input quantity" }]}
              >
                <InputNumber className="w-full" />
              </Item>

              <Item
                label="Requested Date"
                name="requestedDate"
                rules={[{ required: true, message: "Please select requested date" }]}
              >

                <DatePicker
                  showTime
                  className="w-full"
                  format="YYYY-MM-DD HH:mm"
                />
              </Item>
            </div>

            <Item label="Fullfiled Date" name="fullfiledDate">
              <DatePicker
                showTime
                className="w-full"
                format="YYYY-MM-DD HH:mm"
              />
            </Item>

            <Item
              label="Deadline"
              name="deadline"
              rules={[{ required: true, message: "Please select deadline" }]}
            >
              <DatePicker
                showTime
                className="w-full"
                format="YYYY-MM-DD HH:mm"
              />
            </Item>

            <Item>
              <Button
                type="primary"
                htmlType="submit"
                className="w-full bg-blue-600 hover:bg-blue-700"
              >
                Submit Order
              </Button>
            </Item>
          </div>
        </Form>
      </div>
    </div>
  );
}

export default OrderForm;