import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiResponseInterceptor from '../_services/apiResponseInterceptor';

// GET SHOP PRODUCT BY ID
export const fetchShopProductById = createAsyncThunk('shopping/fetchShopProductById', async (id, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.get(`/shopping/${id}`, { withCredentials: true });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Failed to fetch shop product by ID");
    }
});

// GET ALL SHOP PRODUCTS
export const fetchShopProducts = createAsyncThunk("shopping/fetchShopProducts", async ({ pageSize }, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.get('/shopping', {
            params: { pageSize },
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Failed to fetch shop products");
    }
});
// GET ALL SHOP PRODUCTS BY USER EMAIL
export const fetchShopProductsByUserEmail = createAsyncThunk("shopping/fetchShopProductsByUserEmail", async ({ pageSize }, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.get('/shopping/all-by-email', {
            params: { pageSize },
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Failed to fetch shop products");
    }
});


// DELETE SHOP PRODUCT
export const deleteShopProduct = createAsyncThunk('shopping/deleteShopProduct', async (id, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.delete(`/shopping/${id}`, { withCredentials: true });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Failed to delete shop product");
    }
});

// CREATE SHOP PRODUCT
export const createShopProduct = createAsyncThunk('shopping/createShopProduct', async (productData, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.post('/shopping', productData, { withCredentials: true });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data || "Failed to create shop product");
    }
});

// UPDATE SHOP PRODUCT
export const updateShopProduct = createAsyncThunk('shopping/updateShopProduct', async (productData, { rejectWithValue }) => {
    try {
        const response = await apiResponseInterceptor.put('/shopping/update', productData, { withCredentials: true });
        return response.data;
    } catch (error) {
        return rejectWithValue(error.response?.data?.title || "Failed to update shop product");
    }
});

const normalizeShopProduct = (p) => ({
    ...p,
    id: p.id,
    content: p.content,
    createdAt: p.createdAt,
    isBought: p.isBought,
    userId: p.userId,
    userName: p.userName,
    userPicture: p.userPicture,
});

const shoppingSlice = createSlice({
    name: "shopping",
    initialState: {
        shopProductList: [],
        shopProductListByEmail: [],
        pageSize: 10,
        status: "idle",
        error: null,
    },
    reducers: {},
    extraReducers: (builder) => {
        // FETCH SHOP PRODUCT BY ID
        builder.addCase(fetchShopProductById.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchShopProductById.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.shopProductList = [normalizeShopProduct(action.payload)];
        }).addCase(fetchShopProductById.rejected, (state) => {
            state.status = "failed";
            state.error = action.payload;
        });
        // FETCH ALL SHOP PRODUCTS
        builder.addCase(fetchShopProducts.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchShopProducts.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.shopProductList = action.payload.map(normalizeShopProduct)
        }).addCase(fetchShopProducts.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        });
        // FETCH ALL SHOP PRODUCTS BY USER EMAIL
        builder.addCase(fetchShopProductsByUserEmail.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(fetchShopProductsByUserEmail.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.shopProductListByEmail = action.payload.map(normalizeShopProduct);
        }).addCase(fetchShopProductsByUserEmail.rejected, (state, action) => {
            state.status = "failed";
            state.error = action.payload;
        });
        // DELETE SHOP PRODUCT
        builder.addCase(deleteShopProduct.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(deleteShopProduct.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.shopProductList = state.shopProductList.filter(product => product.id !== action.payload.id);
            state.shopProductListByEmail = state.shopProductListByEmail.filter(product => product.id !== action.payload.id);
        }).addCase(deleteShopProduct.rejected, (state) => {
            state.status = "failed";
            state.error = action.payload;
        });
        // CREATE SHOP PRODUCT
        builder.addCase(createShopProduct.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(createShopProduct.fulfilled, (state, action) => {
            state.status = "succeeded";
            state.shopProductList.push(normalizeShopProduct(action.payload));
        }).addCase(createShopProduct.rejected, (state) => {
            state.status = "failed";
            state.error = action.payload;
        });
        // UPDATE SHOP PRODUCT
        builder.addCase(updateShopProduct.pending, (state) => {
            state.status = "loading";
            state.error = null;
        }).addCase(updateShopProduct.fulfilled, (state, action) => {
            state.status = "succeeded";
            const updated = normalizeShopProduct(action.payload);
            const index = state.shopProductList.findIndex(product => product.id === updated.id);
            if (index !== -1) {
                state.shopProductList[index] = updated;
            }
        }).addCase(updateShopProduct.rejected, (state) => {
            state.status = "failed";
            state.error = action.payload;
        });
    },
});

export default shoppingSlice.reducer;
