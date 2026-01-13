import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import './Navbar.css'
const logo_dr = "/logo_dr.svg";


export const Navbar = () => {

  const navigate = useNavigate();
  const isAuthenticated = useSelector((state) => state.auth.isAuthenticated);
  const handleNavigation = (path) => {
    setTimeout(() => { navigate(path); }, 750);
  };

  return (
    <nav className='nav'>
      <a className='nav__logo-link' to={isAuthenticated ? "/home" : "/"} onClick={() => handleNavigation(isAuthenticated ? "/home" : "/")}>
        <img className='nav__logo' src={logo_dr} alt="Chores Hub" />
      </a>
      {isAuthenticated && (<button
        className="nav__account-btn btn-stand"
        onClick={() => handleNavigation(`/account`)}
      >
        Account
      </button>)
      }
    </nav>
  )
}
