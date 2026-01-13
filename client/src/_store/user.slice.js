import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import apiResponseInterceptor from "../_services/apiResponseInterceptor";
const def_avatar = "/def_avatar.svg";


// FETCH USER BY ID
export const fetchUserById = createAsyncThunk(
    "user/fetchUserById",
    async (id, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/user/${id}`, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch user");
        }
    }
);

// CREATE / REGISTER USER
export const createUser = createAsyncThunk(
    "user/createUser",
    async (userData, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post(`/user/register`, userData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to register user");
        }
    }
);

// UPDATE USER
export const updateUser = createAsyncThunk(
    "user/updateUser",
    async (userData, { rejectWithValue }) => {
        try {
            const formData = new FormData();
            formData.append("id", userData.id);
            formData.append("email", userData.email);
            formData.append("fullName", userData.fullName);
            if (userData.profilePicture && userData.profilePicture instanceof File) {
                formData.append("profilePicture", userData.profilePicture);
            }
            const response = await apiResponseInterceptor.put(
                `/user/update`,
                formData,
                {
                    withCredentials: true,
                    headers: { "Content-Type": "multipart/form-data" }
                }
            );
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to update user");
        }
    }
);

// DELETE USER
export const deleteUser = createAsyncThunk(
    "user/deleteUser",
    async ({ id, password }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.delete(
                `/user`,
                {
                    data: { id, password },
                    withCredentials: true,
                    headers: { "Content-Type": "application/json" }
                }
            );
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to delete user");
        }
    }
);


// CHANGE PASSWORD (AUTHORIZED)
export const changePassword = createAsyncThunk(
    "user/changePassword",
    async ({ oldPassword, newPassword}, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post(
                `/user/change-password`,
                { oldPassword, newPassword},
                { withCredentials: true, headers: { "Content-Type": "application/json" } }
            );
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to change password!");
        }
    }
);

// REQUEST RESET PASSWORD
export const requestPasswordReset = createAsyncThunk(
    "user/requestPasswordReset",
    async (email, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post(
                `/user/request-reset-password`,
                { email },
                { withCredentials: true, headers: { "Content-Type": "application/json" } }
            );
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to request password reset!");
        }
    }
);

// CONFIRM RESET PASSWORD
export const confirmPasswordReset = createAsyncThunk(
    "user/confirmPasswordReset",
    async ({ token, newPassword }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post(
                `/user/confirm-reset-password`,
                { token, newPassword },
                { withCredentials: true, headers: { "Content-Type": "application/json" } }
            );
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to reset password!");
        }
    }
);


const userSlice = createSlice({
    name: "user",
    initialState: {
        id: null,
        fullName: null,
        email: null,
        isLoading: false,
        profilePicture: def_avatar,
        success: false,
        status: "idle",
        error: null,
    },
    reducers: {
        updateProfilePictureStart: (state) => {
            state.isLoading = true;
            state.error = null;
            state.success = false;
        },
        updateProfilePictureSuccess: (state, action) => {
            state.isLoading = false;
            state.success = true;
            state.profilePicture = action.payload;
        },
        updateProfilePictureFailure: (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        },
        clearUserState: (state) => {
            state.id = null;
            state.fullName = null;
            state.email = null;
            state.profilePicture = def_avatar;
            state.isLoading = false;
            state.success = false;
            state.status = "idle";
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        // FETCH USER BY ID
        builder.addCase(fetchUserById.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(fetchUserById.fulfilled, (state, action) => {
            state.isLoading = false;
            state.fullName = action.payload.fullName;
            state.email = action.payload.email;
            state.id = action.payload.id;
            state.profilePicture = action.payload.profilePictureUrl || def_avatar;
        }).addCase(fetchUserById.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        })
        // CREATE / REGISTER USER
        builder.addCase(createUser.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(createUser.fulfilled, (state, action) => {
            state.isLoading = false;
            state.id = action.payload.id;
            state.fullName = action.payload.fullName;
            state.email = action.payload.email;
            state.profilePicture = action.payload.profilePicture || def_avatar;
            state.success = true;
            state.status = "succeeded";
        }).addCase(createUser.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload || "Failed to register user";
            state.success = false;
            state.status = "failed";
        });
        // UPDATE USER
        builder.addCase(updateUser.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(updateUser.fulfilled, (state, action) => {
            state.isLoading = false;
            state.id = action.payload.id;
            state.fullName = action.payload.fullName;
            state.email = action.payload.email;
            state.profilePicture = action.payload.profilePicture;
        }).addCase(updateUser.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        })
        // DELETE USER
        builder.addCase(deleteUser.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(deleteUser.fulfilled, (state) => {
            state.isLoading = false;
            state.fullName = null;
            state.email = null;
            state.profilePicture = def_avatar;
        }).addCase(deleteUser.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        });
        // REQUEST RESET PASSWORD
        builder.addCase(requestPasswordReset.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(requestPasswordReset.fulfilled, (state) => {
            state.isLoading = false;
        }).addCase(requestPasswordReset.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        });

        // CONFIRM RESET PASSWORD
        builder.addCase(confirmPasswordReset.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(confirmPasswordReset.fulfilled, (state) => {
            state.isLoading = false;
        }).addCase(confirmPasswordReset.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        });

        // CHANGE PASSWORD
        builder.addCase(changePassword.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        }).addCase(changePassword.fulfilled, (state) => {
            state.isLoading = false;
        }).addCase(changePassword.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        });

    },
});

export const {
    updateProfilePictureStart,
    updateProfilePictureSuccess,
    updateProfilePictureFailure,
    clearUserState,
} = userSlice.actions;

export default userSlice.reducer;
