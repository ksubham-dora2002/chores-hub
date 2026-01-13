import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import api from "../_services/api";
import apiResponseInterceptor from "../_services/apiResponseInterceptor";
import { createUser } from "./user.slice";

// LOGIN
export const loginAsync = createAsyncThunk(
    "auth/loginAsync",
    async (loginData, { rejectWithValue }) => {
        try {
            const response = await api.post(`/user/login`, loginData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Login failed");
        }
    }
);

// REFRESH TOKEN
export const refreshTokenAsync = createAsyncThunk(
    "auth/refreshToken",
    async (_, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post(`/user/refresh`, {}, { withCredentials: true });
            return response.data;
        } catch (error) {
            localStorage.removeItem("atk");
            localStorage.removeItem("rtk");
            return rejectWithValue(error.response?.data || "Token refresh failed");
        }
    }
);

// LOGOUT
export const logoutAsync = createAsyncThunk(
    "auth/logout",
    async (_, { rejectWithValue }) => {
        try {
            await apiResponseInterceptor.post(`/user/logout`, {}, { withCredentials: true });
            localStorage.removeItem("atk");
            localStorage.removeItem("rtk");
            return true;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Logout failed");
        }
    }
);

const authSlice = createSlice({
    name: "auth",
    initialState: {
        id: "",
        isAuthenticated: false,
        loginStatus: "idle",
        logoutStatus: "idle",
        refreshStatus: "idle",
        registerStatus: "idle",
        refreshError: null,
        loginError: null,
        logoutError: null,
        registerError: null,
    },
    reducers: {
        resetAuthState: (state) => {
            state.id = "";
            state.isAuthenticated = false;
            state.loginStatus = "idle";
            state.logoutStatus = "idle";
            state.refreshStatus = "idle";
            state.registerStatus = "idle";
            state.loginError = null;
            state.logoutError = null;
            state.registerError = null;
            state.refreshError = null;
        },
    },
    extraReducers: (builder) => {
        // REGISTER (createUser)
        builder
            .addCase(createUser.pending, (state) => {
                state.registerStatus = "loading";
                state.registerError = null;
            })
            .addCase(createUser.fulfilled, (state, action) => {
                state.registerStatus = "succeeded";
                state.id = action.payload.id;
                state.isAuthenticated = true;
            })
            .addCase(createUser.rejected, (state, action) => {
                state.registerStatus = "failed";
                state.isAuthenticated = false;
                state.registerError = action.payload || "Registration failed. Please try again.";
            });

        // LOGIN
        builder
            .addCase(loginAsync.pending, (state) => {
                state.loginStatus = "loading";
                state.loginError = null;
            })
            .addCase(loginAsync.fulfilled, (state, action) => {
                state.loginStatus = "succeeded";
                state.id = action.payload;
                state.isAuthenticated = true;
            })
            .addCase(loginAsync.rejected, (state, action) => {
                state.loginStatus = "failed";
                state.loginError = action.payload;
            });

        // REFRESH TOKEN
        builder
            .addCase(refreshTokenAsync.pending, (state) => {
                state.refreshStatus = "loading";
                state.refreshError = null;
            })
            .addCase(refreshTokenAsync.fulfilled, (state, action) => {
                state.refreshStatus = "succeeded";
                state.id = action.payload;
                state.isAuthenticated = true;
            })
            .addCase(refreshTokenAsync.rejected, (state, action) => {
                state.refreshStatus = "failed";
                state.refreshError = action.payload;
                state.isAuthenticated = false;
            });

        // LOGOUT
        builder
            .addCase(logoutAsync.pending, (state) => {
                state.logoutStatus = "loading";
                state.logoutError = null;
            })
            .addCase(logoutAsync.fulfilled, (state) => {
                state.logoutStatus = "succeeded";
                state.id = "";
                state.isAuthenticated = false;
            })
            .addCase(logoutAsync.rejected, (state, action) => {
                state.logoutStatus = "failed";
                state.logoutError = action.payload;
                state.isAuthenticated = false;
            });
    },
});

export const { resetAuthState } = authSlice.actions;
export default authSlice.reducer;
