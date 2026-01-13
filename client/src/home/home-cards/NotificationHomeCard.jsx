import { fetchNotifications, updateNotification } from '../../_store/notification.slice';
import { useSelector, useDispatch } from 'react-redux';
import { toast } from 'react-toastify';
import { homeCardStatusRenderer, formatDateTime } from '../../utils/homeCardUtils';
const def_avatar = "/def_avatar.svg";
import { useEffect } from 'react';


export const NotificationHomeCard = () => {

    const dispatch = useDispatch();
    const { notificationList, status, pageSize, error } = useSelector((state) => state.notification);

    useEffect(() => {
        if (status === 'idle') {
            dispatch(fetchNotifications({ pageSize }));
        }
    }, [dispatch, pageSize]);

    homeCardStatusRenderer(status, error, 'Loading notifications...', 'notifications');

    // UPDATE HANDLER
    const handleUpdatingNotificationSeen = async (notification) => {
        const response = await dispatch(updateNotification({
            ...notification,
            isSeen: true,
            userPicture: notification.userPicture || def_avatar,
        }));
        if (updateNotification.fulfilled.match(response)) {
            await dispatch(fetchNotifications({ pageSize }));
            toast.success("Notification marked as seen.");
        } else if (updateNotification.rejected.match(response)) {
            toast.error(response.payload || "Failed to mark notification as seen");
        }

    };

    return (
        <>
            {notificationList.length > 0 ? (
                notificationList.map((notification) => (
                    <div key={notification.id} className="home-card home-card--notification">
                        <div className="home-card__header">
                            <h4 className="home-card__title home-card__title--notification">Notification</h4>
                        </div>
                        <div className="home-card__body">
                            <div className="home-card__user">
                                <img className="home-card__avatar" src={notification.userPicture || def_avatar} alt={notification.userName || 'User'} />
                                <span className="home-card__user-name">{notification.userName || 'Unknown User'}</span>
                            </div>
                            <div className="home-card__content">
                                <p className="home-card__content-text">{notification.content}</p>
                                <time className="home-card__date" dateTime={notification.createdAt}>
                                    {formatDateTime(notification.createdAt)}
                                </time>
                            </div>
                            <button className="btn-stand home-card__action-btn"
                                onClick={() => handleUpdatingNotificationSeen(notification)}
                            >Seen</button>
                        </div>
                    </div>
                ))
            ) : (<p>No notifications available.</p>)}
        </>
    )
}
