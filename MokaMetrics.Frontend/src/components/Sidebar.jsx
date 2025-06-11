import { DashboardOutlined, OrderedListOutlined, MenuUnfoldOutlined, MenuFoldOutlined } from '@ant-design/icons';
import { Menu, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';

const Sidebar = () => {
    const [collapsed, setCollapsed] = useState(false);

    const navigate = useNavigate();

    const items = [
        {
            key: '',
            label: 'Dashboard',
            type: 'item',
            icon: <DashboardOutlined />,
        },
        {
            key: 'order-form',
            label: 'Order Form',
            type: 'item',
            icon: <OrderedListOutlined />,
        },
    ];

    const toggleCollapsed = () => {
        setCollapsed(!collapsed);
    };


    return (
        <div className={"flex flex-col bg-zinc-900"}>
            <button className='m-2 bg-white rounded p-1 hover:border-teal-400 border transition-all hover:tracking-normal'  type="" onClick={toggleCollapsed}>
                {collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            </button>
            <Menu
                style={{ backgroundColor: '#1b1718' }}
                inlineCollapsed={collapsed}
                mode="inline"
                theme='dark'
                items={items}
                onClick={({ key }) => navigate(key)}
            />
        </div>
    );
}

export default Sidebar;