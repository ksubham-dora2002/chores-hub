import axios from "axios";
import { refreshTokenAsync, resetAuthState } from "../_store/auth.slice";

const apiResponseInterceptor = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    withCredentials: true, 
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};


export const setupInterceptors = (store) => {
    apiResponseInterceptor.interceptors.response.use(
        (response) => response, 
        async (error) => {
            const originalRequest = error.config;

            // GLOBAL SERVER CRASH HANDLER
            if (!error.response) {
                store.dispatch(resetAuthState()); 

                if (window.location.pathname !== "/") {
                    window.location.href = "http://localhost:3030/"; 
                }
                return Promise.reject(error);
            }

            if (error.response?.status === 401 && !originalRequest._retry) {
                if (isRefreshing) {
                    return new Promise(function (resolve, reject) {
                        failedQueue.push({ resolve, reject });
                    })
                        .then(() => apiResponseInterceptor(originalRequest))
                        .catch((err) => Promise.reject(err)); 
                } 

                originalRequest._retry = true;
                isRefreshing = true;

                try {
                    await store.dispatch(refreshTokenAsync());
                    processQueue(null);
                    isRefreshing = false;
                    return apiResponseInterceptor(originalRequest);
                } catch (refreshError) {
                    processQueue(refreshError, null);
                    isRefreshing = false;
                    store.dispatch(resetAuthState());
                    window.location.href = "http://localhost:3030/";
                    return; 
                }
            }
            return Promise.reject(error);
        }
    );
};


export default apiResponseInterceptor;
