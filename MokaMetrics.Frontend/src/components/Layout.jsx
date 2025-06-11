import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";

const Layout = ({ children }) => {
    return (
        <>
        <div className="flex flex-row">
            
       <Sidebar/>
       {children}
       <Outlet/>
       </div>
       
       </>
    );
}

export default Layout;