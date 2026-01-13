import { toast } from 'react-toastify';

export function toastSuccessWithDelay(delay, message) {
 setTimeout(() => {
    toast.success(message);
 }, delay);
}