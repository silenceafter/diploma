import { configureStore } from '@reduxjs/toolkit';
import rootReducer from './reducers'; // Ваш корневой редюсер

const store = configureStore({
  reducer: rootReducer
});

export default store;