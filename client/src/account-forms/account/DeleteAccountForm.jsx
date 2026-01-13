import { deleteUser, clearUserState } from '../../_store/user.slice';
import { resetAuthState } from '../../_store/auth.slice';
import { toast } from 'react-toastify';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import "./DeleteAccountForm.css";


export const DeleteAccountForm = () => {


    const currentUserId = useSelector((state) => state.auth.id);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    // DELETE HANDLER
    const handleDelete = async (e) => {
        e.preventDefault();
        const password = e.target.password.value;
        if (!password) {
            toast.error("Please enter your password to confirm deletion.");
            return;
        }
        const result = await dispatch(deleteUser({ id: currentUserId, password }));
        if (deleteUser.fulfilled.match(result)) {
            dispatch(clearUserState());
            dispatch(resetAuthState());
            navigate("/");
            setTimeout(() => {
                toast.success("Account deleted successfully!");
            }, 250);
        } else if (deleteUser.rejected.match(result)) {
            toast.error(result.payload || "Failed to delete account. Please try again.");
        }
        e.target.reset();
    }


    return (
        <>
            <div className="account-card account-card--delete">
                <div className="account-card__body">
                    <h3 className="entry__list-title account-card__title">Delete Account</h3>
                    <p className="account-card__warning">Warning! This operation cannot be undone.</p>
                    <p className="account-card__warning">To approve this operation please enter your password.</p>
                    <form className="account-card--delete__form" onSubmit={handleDelete}>
                        <input
                            className='account-card--delete__form-input form-input'
                            type="password"
                            name="password"
                            id="password"
                            placeholder='password'
                        />
                        <button className="account-card--delete__form-btn account-card__form-btn btn-stand" type='submit'>Delete</button>
                    </form>
                </div>
            </div>
        </>
    )

};