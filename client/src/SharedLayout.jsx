import { Footer, Navbar } from './_components';
import { Outlet } from 'react-router-dom';

export const SharedLayout = () => {
    return (
        <>
            <Navbar />
            <Outlet />
            <Footer />
        </>
    )
}
