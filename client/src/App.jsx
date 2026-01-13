import { NotificationForm, ShoppingForm, TaskForm, AccountForm, DeleteAccountForm, ChangePasswordForm } from "@/account-forms/index";
import { Login, Account, Password, Register } from '@/account';
import { refreshTokenAsync } from './_store/auth.slice';
import { useSelector, useDispatch } from "react-redux";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { Routes, Route } from "react-router-dom";
import { SharedLayout } from "@/SharedLayout";
import { useEffect, useRef } from 'react';
import { Error } from "@/_components";
import { Home } from '@/home/Home';
import '@/App.css';



function App() {
  const dispatch = useDispatch();
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const refreshStatus = useSelector((state) => state.auth.refreshStatus);
  const hasRefreshed = useRef(false);

  useEffect(() => {
    if (!hasRefreshed.current) {
      dispatch(refreshTokenAsync());
      hasRefreshed.current = true;
    }
  }, [dispatch]);

  if (refreshStatus === 'loading' && !hasRefreshed.current) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <>
      <div id='app' className={isAuthenticated ? 'authenticated' : 'not-authenticated'}>
        <Routes>
          <Route path="/" element={<SharedLayout />} >
            <Route index element={<Login />} />
            <Route path="home" element={<Home />} />
            <Route path="account" element={<Account />}>
              <Route path="update-profile" element={<AccountForm />} />
              <Route path="task" element={<TaskForm />} />
              <Route path="notification" element={<NotificationForm />} />
              <Route path="shopping" element={<ShoppingForm />} />
              <Route path="delete-account" element={<DeleteAccountForm />} />
              <Route path="change-password" element={<ChangePasswordForm />} />
            </Route>
            <Route path="register" element={<Register />} />
            <Route path="reset-password" element={<Password />} />
            <Route path="*" element={<Error />} />
          </Route>
        </Routes>
        <ToastContainer autoClose={1000} />
      </div >
    </>
  )
}

export default App

