import { changePassword, clearUserState } from '../../_store/user.slice';
import { resetAuthState } from '../../_store/auth.slice';
import { toast } from 'react-toastify';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import "./ChangePasswordForm.css";

export function ChangePasswordForm() {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handlePasswordChange = async (e) => {
    e.preventDefault();
    if (e.target.pwdNew.value !== e.target.pwdNewAgain.value) {
      toast.error("New passwords do not match. Please try again.");
      return;
    }

    const formData = {
      oldPassword: e.target.pwdOld.value,
      newPassword: e.target.pwdNew.value,
    };

    const result = await dispatch(changePassword(formData));

    if (changePassword.fulfilled.match(result)) {
      dispatch(resetAuthState());
      dispatch(clearUserState());
      navigate("/");
      setTimeout(() => {
        toast.success("Password changed successfully! Please log in again.");
      }, 250);
    } else if (changePassword.rejected.match(result)) {
      toast.error(result.payload || "Failed to change password. Please try again.");
    }
    e.target.reset();
  }
  return (
    <>
      <div className="account-card account-card--password">
        <div className="account-card__body account-card__body--password">
          <h3 className="entry__list-title account-card__title account-card__title--password">Change Password</h3>
          <form className="account-card__form--password" onSubmit={handlePasswordChange}>
            <div className="account-card__form--password-box account-card__form-box form-box">
              <label htmlFor="pwdOld" className="account-card__form-label">Old Password</label>
              <input
                className='account-card__form--password-input form-input'
                type="password"
                name="pwdOld"
                id="pwdOld"
                placeholder='old password'
                required
              />
            </div>
            <div className="account-card__form--password-box account-card__form-box form-box">
              <label htmlFor="pwdNew" className="account-card__form-label">New Password</label>
              <input
                className='account-card__form--password-input form-input'
                type="password"
                name="pwdNew"
                id="pwdNew"
                placeholder='new password'
                required
              />
            </div>
            <div className="account-card__form--password-box account-card__form-box form-box">
              <label htmlFor="pwdNewAgain" className="account-card__form-label">New Password</label>
              <input
                className='account-card__form--password-input form-input'
                type="password"
                name="pwdNewAgain"
                id="pwdNewAgain"
                placeholder='new password'
                required
              />
            </div>
            <button className="account-card__form--password-btn account-card__form-btn btn-stand" type='submit'>Change</button>
          </form>
        </div>
      </div>
    </>
  )
}

