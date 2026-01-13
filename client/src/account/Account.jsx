import { useEffect } from 'react'
import './Account.css'
import { Link, Outlet, useNavigate } from 'react-router-dom'
import { logoutAsync, resetAuthState } from '../_store/auth.slice';
import { fetchTasks } from '../_store/task.slice';
import { fetchNotifications } from '../_store/notification.slice';
import { fetchShopProducts } from '../_store/shopping.slice';
import { useDispatch, useSelector } from 'react-redux';
import { clearUserState, fetchUserById } from '../_store/user.slice';
import { toast } from 'react-toastify';
const def_avatar = "/def_avatar.svg";


export const Account = () => {

    const { fullName, profilePicture, status } = useSelector((state) => state.user);
    const currentUserId = useSelector((state) => state.auth.id);

    const pageSizeTask = useSelector((state) => state.task.pageSize);
    const pageSizeNotification = useSelector((state) => state.notification.pageSize);
    const pageSizeShopping = useSelector((state) => state.shopping.pageSize);
    const maxPageSize = Math.max(pageSizeTask, pageSizeNotification, pageSizeShopping);

    const dispatch = useDispatch();
    const navigate = useNavigate();

    const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
    const logoutStatus = useSelector((state) => state.auth.logoutStatus);
    useEffect(() => {
        if (isAuthenticated && status === 'idle' && currentUserId) {
            dispatch(fetchUserById(currentUserId));
        }
    }, [dispatch, isAuthenticated, currentUserId, status]);

    useEffect(() => {
        if (logoutStatus === "loading") {
            toast.info("Logout in progress. Please wait.");
        }
    }, [logoutStatus]);

    // LOGOUT HANDLER
    const handleLogout = async () => {
        if (logoutStatus === "loading") {
            toast.info("Logout in progress. Please wait.");
            return;
        }
        const result = await dispatch(logoutAsync());
        if (logoutAsync.fulfilled.match(result)) {
            dispatch(resetAuthState());
            dispatch(clearUserState());
            navigate("/");
            setTimeout(() => {
                toast.success("Logout successful!");
            }, 250)

        } else if (logoutAsync.rejected.match(result)) {
            toast.error(result.payload || "Logout failed!");
        }

    }

    // NAVIGATION HANDLER
    const handleNavigationToHome = (path) => {
        dispatch(fetchTasks({ pageSize: maxPageSize }));
        dispatch(fetchNotifications({ pageSize: maxPageSize }));
        dispatch(fetchShopProducts({ pageSize: maxPageSize }));
        setTimeout(() => {
            navigate(path);
        }, 1000);
    }

    return (
        <>
            <section className="account">
                <div className='account__sidebar'>
                    <div className="account__user">
                        <div className="account__avatar-box">
                            <img src={profilePicture ? profilePicture : def_avatar} alt="" className="account__avatar" />
                        </div>
                        <h3 className="account__user-name">{fullName}</h3>
                    </div>
                    <ul className="account__nav">
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                account_circle
                            </span>
                            <Link to="update-profile" className='account__nav-link'>Account</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                task_alt
                            </span>
                            <Link to="task" className='account__nav-link'>Enter Task</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                notifications
                            </span>
                            <Link to="notification" className='account__nav-link'>Enter Notification</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                shopping_cart
                            </span>
                            <Link to="shopping" className='account__nav-link'>Enter Shopping Item(s)</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                home
                            </span>
                            <a to="/home" className='account__nav-link' onClick={() => handleNavigationToHome("/home")}>Home</a>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                delete
                            </span>
                            <Link to="delete-account" className='account__nav-link'>Delete Account</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                key
                            </span>
                            <Link to="change-password" className='account__nav-link'>Change Password</Link>
                        </li>
                        <li className="account__nav-item">
                            <span className="material-symbols-rounded" id='account__nav-icon'>
                                logout
                            </span>
                            <button onClick={handleLogout} className="account__nav-link btn-link">
                                Logout
                            </button>
                        </li>
                    </ul>
                </div>
                <Outlet />
            </section>
        </>
    )
}
