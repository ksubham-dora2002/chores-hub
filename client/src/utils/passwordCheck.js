import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

// PASSWORD STRENGTH CHECKER
export const checkPasswordStrength = (pwd) => {

    const trimmedPassword = pwd.trim();
    const hasUpper = /[A-Z]/.test(trimmedPassword);
    const hasLower = /[a-z]/.test(trimmedPassword);
    const hasNumber = /[0-9]/.test(trimmedPassword);
    const hasSpecial = /[^A-Za-z0-9]/.test(trimmedPassword);

    if (trimmedPassword.length < 8) {
        toast.error("Password must be at least 8 characters long.");
        return null;
    }
    if (!hasUpper || !hasLower || !hasNumber || !hasSpecial) {
        toast.error("Password must contain uppercase, lowercase, number, and special character.");
        return null;
    }
    return trimmedPassword;
}
