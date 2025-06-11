import { Routes, BrowserRouter, Route } from "react-router-dom";
import OrderForm from "./orderForm/OrderForm";
import Dashboard from "./dashboard/Dashboard";
import './index.css';
import Layout from "./components/Layout";
export default function App() {
  
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />} >
          <Route path="/order-form" element={<OrderForm />} />
          <Route index element={<Dashboard />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
} 
