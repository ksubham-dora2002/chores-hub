import { createShopProduct, fetchShopProductsByUserEmail, updateShopProduct, deleteShopProduct } from '../_store/shopping.slice';
import { toast } from 'react-toastify';
import { useDispatch, useSelector } from 'react-redux';
import { toastSuccessWithDelay } from '../utils/toast';
import { trimAndValidateContent } from '../utils/accountEntryFormUtils';
import { useState, useEffect } from 'react';
import { useEditState } from "../utils/useEditState";


export const ShoppingForm = () => {

  const dispatch = useDispatch();
  const currentUserId = useSelector((state) => state.auth.id);
  const { shopProductListByEmail, pageSize } = useSelector((state) => state.shopping);

  const [content, setContent] = useState("");
  const { editId, editContent, 
    setEditContent, setEditId, 
    handleEdit, resetEdit } = useEditState();

  useEffect(() => {
    if (currentUserId) {
      dispatch(fetchShopProductsByUserEmail({ pageSize }));
    }
  }, [dispatch, pageSize]);

  // CREATE HANDLER
  const handleCreate = async (e) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(content, 150);

    const result = await dispatch(createShopProduct({
      content: trimmedContent,
      date: currentDate,
      isBought: false,
      userId: currentUserId
    }));

    if (createShopProduct.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Shopping entry created!");
      setContent("");
      dispatch(fetchShopProductsByUserEmail({ pageSize }));
    } else if (createShopProduct.rejected.match(result)) {
      toast.error(result.payload || "Failed to create shopping entry");
    }
  };

  // UPDATE HANDLER
  const handleUpdate = async (e, id) => {
    e.preventDefault();
    const currentDate = new Date().toISOString();
    const trimmedContent = trimAndValidateContent(editContent, 150);

    const result = await dispatch(updateShopProduct({
      id,
      content: trimmedContent,
      date: currentDate,
      userId: currentUserId,
      isBought: false
    }));

    if (updateShopProduct.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Shopping entry updated!");
      resetEdit();
      dispatch(fetchShopProductsByUserEmail({ pageSize }));
    } else if (updateShopProduct.rejected.match(result)) {
      toast.error(result.payload || "Failed to update shopping entry");
    }
  };

  // DELETE HANDLER
  const handleDelete = async (e, id) => {
    e.preventDefault();
    const result = await dispatch(deleteShopProduct(id));

    if (deleteShopProduct.fulfilled.match(result)) {
      toastSuccessWithDelay(50, "Shopping entry deleted!");
      dispatch(fetchShopProductsByUserEmail({ pageSize }));
    } else if (deleteShopProduct.rejected.match(result)) {
      toast.error(result.payload || "Failed to delete shopping entry");
    }
  };

  return (
    <>
      <div className="entry">
        <div className="entry__create-card">
          <h3 className="entry__title">Shopping Item(s)</h3>
          <form className="entry__form" onSubmit={handleCreate}>
            <div className="entry__form-box--ver">
              <label className="entry__form-label " htmlFor="shopEntityTxt">Enter Shopping Item(s)</label>
              <textarea name="shopEntityTxt" id="shopEntityTxt"
                value={content}
                className="entry__textarea"
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
          <h3 className="entry__list-title">Shopping Entries</h3>
          <ul className="entry__list">
            {shopProductListByEmail && shopProductListByEmail.length > 0 ? (
              shopProductListByEmail.map((shoppingEntry) => (
                <li className="entry__item" key={shoppingEntry.id}>
                  <div className="entry__item-body">
                    <div className="entry__item-status">
                      {shoppingEntry.isBought ? (
                        <>
                          <span className="material-symbols-rounded entry__item-mark-icon entry__item-icon">
                            check_circle
                          </span>
                          <span className="entry__item-status-text">Bought</span>
                        </>
                      ) : (
                        <>
                          <span className="material-symbols-rounded entry__item-mark-icon entry__item-icon">
                            radio_button_unchecked
                          </span>
                          <span className='entry__item-status-text' >Not Bought</span>
                        </>
                      )}
                    </div>
                    <div className="entry__item-content">
                      <p className="entry__item-text">{shoppingEntry.content}</p>
                      <span className="material-symbols-rounded entry__item-edit-icon" onClick={() => handleEdit(shoppingEntry.id, shoppingEntry.content)}>
                        edit
                      </span>
                    </div>

                  </div>
                  {editId === shoppingEntry.id ? (
                    <form className=" entry__item-form" onSubmit={(e) => handleUpdate(e, shoppingEntry.id)}>
                      <input
                        className="entry__item-input"
                        type="text"
                        name='updateShoppingInp'
                        value={editContent}
                        onChange={(e) => setEditContent(e.target.value)}
                        required
                      />
                      <button className="entry__item-btn--update entry__item-btn btn-stand" type="submit">Update</button>
                      <button
                        className="entry__item-btn btn-stand"
                        type="button"
                        onClick={() => setEditId(null)}
                      >Cancel</button>
                    </form>
                  ) : (
                    <form className="entry__item-form--delete entry__item-form" onSubmit={(e) => handleDelete(e, shoppingEntry.id)}>
                      <button
                        className="entry__item-btn entry__item-btn--delete btn-stand"
                        type="submit"
                      >Delete</button>
                    </form>
                  )}
                </li>
              ))
            ) : (
              <li className='entry__item-warning'>No shoping entries found.</li>
            )}
          </ul>
        </div>
      </div>
    </>
  )
}
