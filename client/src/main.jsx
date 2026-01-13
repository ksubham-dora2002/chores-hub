import { setupInterceptors } from './_services/apiResponseInterceptor.js';
import { BrowserRouter } from "react-router-dom";
import { createRoot } from 'react-dom/client';
import { Provider } from "react-redux";
import store from "./_store/store.js";
import { StrictMode } from 'react';
import App from './App.jsx';
import './index.css';

setupInterceptors(store); 
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </Provider>
  </StrictMode>,
)
