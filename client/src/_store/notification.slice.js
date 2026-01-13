import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiResponseInterceptor from '../_services/apiResponseInterceptor';

// GET NOTIFICATION BY ID
export const fetchNotificationById = createAsyncThunk(
    'notification/fetchNotificationById',
    async (id, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/notification/${id}`, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch notification by ID");
        }
    }
);

// GET ALL NOTIFICATIONS
export const fetchNotifications = createAsyncThunk(
    "notification/fetchNotifications",
    async ({ pageSize }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/notification`, {
                params: { pageSize },
                withCredentials: true,
            });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch notifications");
        }
    }
);
// GET ALL NOTIFICATIONS BY EMAIL
export const fetchNotificationsByEmail = createAsyncThunk(
    'notification/fetchNotificationsByEmail',
    async ({ pageSize }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/notification/all-by-email`, {
                params: { pageSize },
                withCredentials: true,
            });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch notifications");
        }
    }
);

// DELETE NOTIFICATION
export const deleteNotification = createAsyncThunk(
    'notification/deleteNotification',
    async (id, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.delete(`/notification/${id}`, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to delete notification");
        }
    }
);

// CREATE NOTIFICATION
export const createNotification = createAsyncThunk(
    'notification/createNotification',
    async (notificationData, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post('/notification', notificationData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to create notification");
        }
    }
);

// UPDATE NOTIFICATION
export const updateNotification = createAsyncThunk(
    'notification/updateNotification',
    async (notificationData, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.put('/notification/update', notificationData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data?.title || "Failed to update notification");

        }
    }
);


const normalizeNotification = (n) => ({
    ...n,
    id: n.id,
    content: n.content,
    createdAt: n.createdAt,
    isSeen: n.isSeen,
    userId: n.userId,
    userName: n.userName,
    userPicture: n.userPicture,
});


const notificationSlice = createSlice({
    name: "notification",
    initialState: {
        notificationList: [],
        notificationListByEmail: [],
        pageSize: 10, 
        status: "idle", 
        error: null, 
    },
    reducers: {},
    extraReducers: (builder) => {
        // FETCH NOTIFICATION BY ID
        builder.addCase(fetchNotificationById.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchNotificationById.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.notificationList = [normalizeNotification(action.payload)]; 
        }).addCase(fetchNotificationById.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        })
        // FETCH ALL NOTIFICATIONS
        builder.addCase(fetchNotifications.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchNotifications.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.notificationList = action.payload.map(normalizeNotification);
        }).addCase(fetchNotifications.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        })
        // FETCH ALL NOTIFICATIONS BY USER EMAIL
        builder.addCase(fetchNotificationsByEmail.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchNotificationsByEmail.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.notificationListByEmail = action.payload.map(normalizeNotification);
        }).addCase(fetchNotificationsByEmail.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        })
        // DELETE NOTIFICATION
        builder.addCase(deleteNotification.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(deleteNotification.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.notificationList = state.notificationList.filter(
                notification => notification.id !== action.payload.id
            );
            state.notificationListByEmail = state.notificationListByEmail.filter(
                notification => notification.id !== action.payload.id
            ); 
        }).addCase(deleteNotification.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        })
        // CREATE NOTIFICATION
        builder.addCase(createNotification.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(createNotification.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.notificationList.push(normalizeNotification(action.payload));
        }).addCase(createNotification.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        })
        // UPDATE NOTIFICATION
        builder.addCase(updateNotification.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(updateNotification.fulfilled, (state, action) => {
            state.status = "succeeded";
            const updated = normalizeNotification(action.payload);
            const index = state.notificationList.findIndex(
                notification => notification.id === updated.id
            );
            if (index !== -1) state.notificationList[index] = updated;
        }).addCase(updateNotification.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        });
    },
});

export default notificationSlice.reducer;

