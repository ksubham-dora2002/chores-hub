import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiResponseInterceptor from '../_services/apiResponseInterceptor';

// FETCH TASK BY ID
export const fetchTaskById = createAsyncThunk(
    'task/fetchTaskById',
    async (id, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/task/${id}`, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch task by ID");
        }
    }
);

// FETCH ALL TASKS   
export const fetchTasks = createAsyncThunk(
    'task/fetchTasks',
    async ({ pageSize }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/task`, {
                params: { pageSize },
                withCredentials: true,
            });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch tasks");
        }
    }
);
// FETCH ALL TASKS BY USER EMAIL
export const fetchTasksByUserEmail = createAsyncThunk(
    'task/fetchTasksByUserEmail',
    async ({ pageSize }, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.get(`/task/all-by-email`, {
                params: { pageSize },
                withCredentials: true,
            });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to fetch tasks");
        }
    }
);

// DELETE TASK
export const deleteTask = createAsyncThunk(
    'task/deleteTask',
    async (id, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.delete(`/task/${id}`, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to delete task");
        }
    }
);

// CREATE TASK   
export const createTask = createAsyncThunk(
    'task/createTask',
    async (taskData, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.post('/task', taskData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to create task");
        }
    }
);

// UPDATE TASK
export const updateTask = createAsyncThunk(
    'task/updateTask',
    async (taskData, { rejectWithValue }) => {
        try {
            const response = await apiResponseInterceptor.put(`/task/update`, taskData, { withCredentials: true });
            return response.data;
        } catch (error) {
            return rejectWithValue(error.response?.data || "Failed to update task");
        }
    }
);

const normalizeTask = (t) => ({
    ...t,
    id: t.id,
    content: t.content,
    createdAt: t.createdAt,
    isDone: t.isDone,
    userId: t.userId,
    userName: t.userName,
    userPicture: t.userPicture,
});


const taskSlice = createSlice({
    name: "task",
    initialState: {
        taskList: [],
        taskListByEmail: [],
        pageSize: 10, 
        status: "idle",
        error: null,
    },
    reducers: {},
    extraReducers: (builder) => {
        // FETCH TASK BY ID
        builder.addCase(fetchTaskById.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(fetchTaskById.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.taskList = [normalizeTask(action.payload)];
        }).addCase(fetchTaskById.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        })
        // FETCH ALL TASKS
        builder.addCase(fetchTasks.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(fetchTasks.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.taskList = action.payload.map(normalizeTask);
        }).addCase(fetchTasks.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        })
        // FETCH ALL TASKS BY USER EMAIL
        builder.addCase(fetchTasksByUserEmail.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(fetchTasksByUserEmail.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.taskListByEmail = action.payload.map(normalizeTask);
        }).addCase(fetchTasksByUserEmail.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        })
        // DELETE TASK
        builder.addCase(deleteTask.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(deleteTask.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.taskList = state.taskList.filter(task => task.id !== action.payload.id);
            state.taskListByEmail = state.taskListByEmail.filter(task => task.id !== action.payload.id);
        }).addCase(deleteTask.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        })
        // CREATE TASK
        builder.addCase(createTask.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(createTask.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.taskList.push(normalizeTask(action.payload));
        }).addCase(createTask.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        })
        // UPDATE TASK
        builder.addCase(updateTask.pending, (state) => {
            state.status = 'loading';
            state.error = null;
        }).addCase(updateTask.fulfilled, (state, action) => {
            state.status = 'succeeded';
            const updated= normalizeTask(action.payload);
            const index = state.taskList.findIndex(task => task.id === updated.id);
            if (index !== -1) {
                state.taskList[index] = updated; 
            }
        }).addCase(updateTask.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.payload;
        });
    }

});


export default taskSlice.reducer;