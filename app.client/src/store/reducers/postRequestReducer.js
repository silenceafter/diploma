import { FETCH_DATA_POST_REQUEST, FETCH_DATA_POST_SUCCESS, FETCH_DATA_POST_FAILURE } from '../actions/postRequestActions';
import { createReducer } from '@reduxjs/toolkit';

const initialState = {};

const postRequestReducer = createReducer(initialState, (builder) => {
    builder
        .addCase(FETCH_DATA_POST_REQUEST, (state, action) => {
            const { key } = action.payload;
            state[key] = {
              loading: true,
              data: null,
              error: null,
            };
        })
        .addCase(FETCH_DATA_POST_SUCCESS, (state, action) => {
            const { key, data } = action.payload;
            state[key] = {
              loading: false,
              data,
              error: null,
            };
        })
        .addCase(FETCH_DATA_POST_FAILURE, (state, action) => {
            const { key, error } = action.payload;
            state[key] = {
              loading: false,
              data: null,
              error,
            };
        });
});

export default postRequestReducer;