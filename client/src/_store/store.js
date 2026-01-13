import notificationReducer from './notification.slice';
import { configureStore } from '@reduxjs/toolkit';
import shoppingReducer from './shopping.slice'
import authReducer from './auth.slice';
import userReducer from './user.slice';
import taskReducer from './task.slice';

const store = configureStore({
    reducer: {
        auth: authReducer,
        user: userReducer,
        task: taskReducer,
        notification: notificationReducer,
        shopping: shoppingReducer,
    },
    devTools: process.env.NODE_ENV === "development",
});

export default store;
