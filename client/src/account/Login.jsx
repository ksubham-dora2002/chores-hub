import { useSelector, useDispatch } from "react-redux";
import { Link, useNavigate } from "react-router-dom";
import { loginAsync } from "../_store/auth.slice";
import { toast } from 'react-toastify';
import { useState, useEffect } from "react";
import './Login.css'
const login = "/login.svg";


export const Login = () => {

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { loginStatus } = useSelector((state) => state.auth);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");


  useEffect(() => {
    if (loginStatus === "loading") {
      toast.info("Login in progress. Please wait.");
    }
  }, [loginStatus]);

  // LOGIN HANDLER
  const handleLogin = async (e) => {
    e.preventDefault();

    if (loginStatus === "loading") return;

    const trimmedEmail = email.trim().toLowerCase();
    const trimmedPassword = password.trim();
    if (trimmedPassword.length < 8) {
      toast.error("Password must be at least 8 characters long.");
      return;
    }

    const result = await dispatch(loginAsync({ email: trimmedEmail, password: trimmedPassword }));

    if (loginAsync.fulfilled.match(result)) {
      toast.success("Login successful!");
      setTimeout(() => navigate("/home"), 750);
    } else if (loginAsync.rejected.match(result)) {
      toast.error(result.payload?.message || result.payload || "Login failed!");
    }
  };

  return (
    <>
      <section className="auth auth--login">
        <div className="auth__card">
          <img className='auth__logo' src={login} alt="Chores Hub" />
          <form className='auth__form auth__form--login' onSubmit={handleLogin} autoComplete="off">
            <div className="auth__form-box form-box">
              <label className="auth__form-label form-label" htmlFor="email">Email</label>
              <input
                className='auth__form-input form-input'
                id="email"
                type="email"
                value={email}
                placeholder='email'
                onChange={(e) => setEmail(e.target.value.toLowerCase())}
                required
              />
            </div>
            <div className="auth__form-box form-box">
              <label className="auth__form-label form-label" htmlFor="password">Password</label>
              <input
                className='auth__form-input form-input'
                id="password"
                type="password"
                value={password}
                placeholder='password'
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            <button
              className="auth__form-btn btn-stand"
              type="submit"
            >
              Login
            </button>
          </form>
          <span className='auth__link auth__link--register'>If you don't have an account please{" "}<Link className="auth__link--text" to="/register">
            <strong>register</strong></Link>
          </span>
          <span className='auth__link auth__link--reset'>
            <Link className="auth__link--text" to="/reset-password">
              <strong>Forgotten your password?</strong>
            </Link>
          </span>
        </div>
      </section>
    </>
  )
}
