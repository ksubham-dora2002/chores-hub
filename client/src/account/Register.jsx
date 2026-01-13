import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from "react-router-dom";
import { createUser } from '../_store/user.slice';
import { toast } from 'react-toastify';
import { checkPasswordStrength } from '../utils/passwordCheck';
import { useState, useEffect } from 'react'
import './Register.css'
const register = "/register.svg";


export const Register = () => {

  const dispatch = useDispatch();
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const registerStatus = useSelector((state) => state.auth.registerStatus);

  useEffect(() => {
    if (registerStatus === "loading") {
      toast.info("Registration in progress. Please wait.");
    }
  }, [registerStatus]);

  //  REGISTER HANDLER
  const handleRegister = async (e) => {
    e.preventDefault();

    if (registerStatus === "loading") return;

    if (e.target.password.value !== e.target.passwordAgain.value) {
      toast.error("Passwords do not match.");
      return;
    }
    const trimmedEmail = email.trim().toLowerCase();
    const trimmedPassword = checkPasswordStrength(password);

    const result = await dispatch(createUser({ fullName, email: trimmedEmail, password: trimmedPassword }));

    if (createUser.fulfilled.match(result)) {
      toast.success("Register successful!");
      setTimeout(() => navigate("/home"), 750);
    } else if (createUser.rejected.match(result)) {
      toast.error(result.payload || "Register failed!");
    }
  }

  return (
    <>
      <section className="auth auth--register">
        <div className='auth__card auth__card--register'>
          <img className='auth__logo auth__logo--register' src={register} alt="Chores Hub" />
          <form className='auth__form auth__form--register' onSubmit={handleRegister} autoComplete='off'>
            <div className="form-box">
              <label className="auth__form-label form-label" htmlFor="fullName">Full name</label>
              <input className='form-input' id='fullName' type="text" placeholder='full name'
                value={fullName} onChange={(e) => setFullName(e.target.value)} required
              />
            </div>
            <div className="form-box">
              <label className="auth__form-label form-label" htmlFor="email">Email</label>
              <input className='form-input' type="email" id='email' name='email' placeholder='email'
                value={email} onChange={(e) => setEmail(e.target.value.toLowerCase())} required
              />
            </div>
            <div className="form-box">
              <label className="auth__form-label form-label" htmlFor="password">Password</label>
              <input className='form-input' type="password" id='password' name='password' placeholder='password'
                value={password} onChange={(e) => setPassword(e.target.value)
                }
                required
              />
            </div>
            <div className="form-box">
              <label className="auth__form-label form-label" htmlFor="passwordAgain">Password Again</label>
              <input className='form-input' type="password" id='passwordAgain' name='passwordAgain' placeholder='password again' required
              />
            </div>
            <button className='btn-stand auth__form-btn auth__form-btn--register' type="submit"
            >Register</button>
          </form>
        </div>
      </section>
    </>
  )
}
