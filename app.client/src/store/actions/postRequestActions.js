import { createAction } from '@reduxjs/toolkit';

export const FETCH_DATA_POST_REQUEST = 'FETCH_DATA_POST_REQUEST';
export const FETCH_DATA_POST_SUCCESS = 'FETCH_DATA_POST_SUCCESS';
export const FETCH_DATA_POST_FAILURE = 'FETCH_DATA_POST_FAILURE';

export const fetchDataPostRequest = createAction(FETCH_DATA_POST_REQUEST);
export const fetchDataPostSuccess = createAction(FETCH_DATA_POST_SUCCESS);
export const fetchDataPostFailure = createAction(FETCH_DATA_POST_FAILURE);