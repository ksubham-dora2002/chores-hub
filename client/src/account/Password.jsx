import { toast } from 'react-toastify';
import { useDispatch } from 'react-redux';
import { requestPasswordReset, confirmPasswordReset } from "../_store/user.slice";
import { useNavigate, useSearchParams } from 'react-router-dom';
import { checkPasswordStrength } from '../utils/passwordCheck';
import './Password.css';
const resetpwd = "/resetpwd.svg";

export const Password = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    const tokenParam = searchParams.get("token");
    const isConfirmMode = Boolean(tokenParam);

    // REQUEST PASSWORD RESET HANDLER
    const handleRequestReset = async (e) => {
        e.preventDefault();
        const email = e.target.email.value;

        const result = await dispatch(requestPasswordReset(email));
        if (result.meta.requestStatus === "fulfilled") {
            toast.success("Reset link sent. Check your email.");
        } else {
            toast.error(`${result.payload}`);
        }
        e.target.reset();
    };

    // CONFIRM PASSWORD RESET HANDLER
    const handleConfirmReset = async (e) => {
        e.preventDefault();
        if (e.target.newPassword.value !== e.target.newPasswordAgain.value) {
            toast.error("Passwords do not match.");
            return;
        }

        const pwd = checkPasswordStrength(e.target.newPassword.value);
        if (!pwd) return;

        const result = await dispatch(confirmPasswordReset({
            token: tokenParam,
            newPassword: pwd
        }));

        if (result.meta.requestStatus === "fulfilled") {
            toast.success("Password updated successfully!");
            navigate("/");
        } else {
            toast.error(`${result.payload}`);
        }
        e.target.reset();
    };

    return (
        <section className="auth">
            <div className='auth__card auth__card--pwd'>
                <img className='auth__logo auth__logo--pwd' src={resetpwd} alt="Chores Hub" />

                {!isConfirmMode && (
                    <form className='auth__form auth__form--pwd' onSubmit={handleRequestReset} autoComplete='off'>
                        <div className="form-box">
                            <label className="form-label auth__form-label" htmlFor="email">Email</label>
                            <input
                                className='form-input'
                                type="email"
                                id='email'
                                name='email'
                                placeholder='email'
                                required
                            />
                        </div>
                        <button className='btn-stand auth__form-btn auth__form-btn--pwd' type="submit">
                            Send Link
                        </button>
                    </form>
                )}

                {isConfirmMode && (
                    <form className='auth__form--pwd auth__form' onSubmit={handleConfirmReset} autoComplete='off'>
                        <div className="form-box">
                            <label className="form-label auth__form-label" htmlFor="newPassword">New Password</label>
                            <input
                                className='form-input'
                                type="password"
                                id='newPassword'
                                name='newPassword'
                                placeholder='new password'
                                required
                            />
                        </div>
                        <div className="form-box">
                            <label className="form-label auth__form-label" htmlFor="newPasswordAgain">New Password</label>
                            <input
                                className='form-input'
                                type="password"
                                id='newPasswordAgain'
                                name='newPasswordAgain'
                                placeholder='new password again'
                                required
                            />
                        </div>
                        <button className='btn-stand auth__form-btn auth__form-btn--pwd--reset' type="submit">
                            Reset
                        </button>
                    </form>
                )}
            </div>
        </section>
    );
};
