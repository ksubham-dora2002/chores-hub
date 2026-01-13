import { createNotification, fetchNotificationsByEmail, updateNotification, deleteNotification } from '../_store/notification.slice';
import { toast } from 'react-toastify';
import { useDispatch, useSelector } from 'react-redux';
import { toastSuccessWithDelay } from '../utils/toast';
import { trimAndValidateContent } from '../utils/accountEntryFormUtils';
import { useState, useEffect } from 'react';
import { useEditState } from "../utils/useEditState";

export const NotificationForm = () => {

  const dispatch = useDispatch();
  const currentUserId = useSelector((state) => state.auth.id);
  const { notificationListByEmail, pageSize } = useSelector((state) => state.notification);

  const [content, setContent] = useState("");
  const { editId, editContent,
    setEditContent, setEditId,
    handleEdit, resetEdit } = useEditState();


  useEffect(() => {
    if (currentUserId) {
      dispatch(fetchNotificationsByEmail({ pageSize }));
    }
  }, [dispatch, pageSize]);

  // CREATE HANDLER
  const handleCreate = async (e) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(content, 150);

    const result = await dispatch(createNotification({
      content: trimmedContent,
      date: currentDate,
      userId: currentUserId,
      isSeen: false
    }));

    if (createNotification.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Notification entry created!");
      setContent("");
      dispatch(fetchNotificationsByEmail({ pageSize }));
    } else if (createNotification.rejected.match(result)) {
      toast.error(result.payload || "Failed to create notification entry");
    }
  };

  // UPDATE HANDLER
  const handleUpdate = async (e, id) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(editContent, 150);

    const result = await dispatch(updateNotification({
      id, content: trimmedContent,
      date: currentDate,
      userId: currentUserId,
      isSeen: false
    }));

    if (updateNotification.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Notification entry updated!");
      resetEdit();
      dispatch(fetchNotificationsByEmail({ pageSize }));
    } else if (updateNotification.rejected.match(result)) {
      toast.error(result.payload || "Failed to update notification entry");
    }
  };

  // DELETE HANDLER
  const handleDelete = async (e, id) => {
    e.preventDefault();
    const result = await dispatch(deleteNotification(id));
    
    if (deleteNotification.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Notification entry deleted!");
      dispatch(fetchNotificationsByEmail({ pageSize }));
    } else if (deleteNotification.rejected.match(result)) {
      toast.error(result.payload || "Failed to delete notification entry");
    }
  };

  return (
    <>
      <div className="entry">
        <div className="entry__create-card">
          <h3 className="entry__title">Notification</h3>
          <form className="entry__form" onSubmit={handleCreate}>
            <div className="entry__form-box--ver">
              <label className="entry__form-label" htmlFor="notifyEntityTxt">Enter Notification</label>
              <textarea
                name="notifyEntityTxt"
                id="notifyEntityTxt"
                className="entry__textarea"
                value={content}
                onChange={(e) => setContent(e.target.value.replace(/[\r\n]+/g, ' '))}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') e.preventDefault();
                }}
                required
              ></textarea>
            </div>
            <button className="entry__form-btn btn-stand" type="submit">Create</button>
          </form>
        </div>
        <div className="entry__list-card">
          <h3 className="entry__list-title">Notification Entries</h3>
          <ul className="entry__list">
            {notificationListByEmail && notificationListByEmail.length > 0 ? (
              notificationListByEmail.map((notify) => (
                <li className="entry__item" key={notify.id}>
                  <div className="entry__item-body">
                    <div className="entry__item-status">
                      {notify.isSeen ? (
                        <>
                          <span className="material-symbols-rounded entry__item-icon">
                            visibility
                          </span>
                          <span className="entry__item-status-text">Seen</span>
                        </>
                      ) : (
                        <>
                          <span className="material-symbols-rounded entry__item-icon">
                            visibility_off
                          </span>
                          <span className="entry__item-status-text">Unseen</span>
                        </>
                      )}
                    </div>
                    <div className="entry__item-content">
                      <p className="entry__item-text">{notify.content}</p>
                      <span className="material-symbols-rounded entry__item-edit-icon" onClick={() => handleEdit(notify.id, notify.content)}>
                        edit
                      </span>
                    </div>
                  </div>
                  {editId === notify.id ? (
                    <form className="entry__item-form" onSubmit={(e) => handleUpdate(e, notify.id)}>
                      <input
                        className="entry__item-input"
                        type="text"
                        name='updateNotificationInp'
                        value={editContent}
                        onChange={(e) => setEditContent(e.target.value)}
                        required
                      />
                      <button className="entry__item-btn--update entry__item-btn btn-stand " type="submit">Update</button>
                      <button
                        className="entry__item-btn btn-stand"
                        type="button"
                        onClick={() => setEditId(null)}
                      >Cancel</button>
                    </form>
                  ) : (
                    <form className="entry__item-form--delete entry__item-form" onSubmit={(e) => handleDelete(e, notify.id)}>
                      <button
                        className="entry__item-btn entry__item-btn--delete btn-stand"
                        type="submit"
                      >Delete</button>
                    </form>
                  )}
                </li>
              ))
            ) : (
              <li className='entry__item-warning'>No notification entries found.</li>
            )}
          </ul>
        </div>
      </div>
    </>
  )
}