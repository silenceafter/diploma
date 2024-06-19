import {
    fetchDataGetRequest,
    fetchDataGetSuccess,
    fetchDataGetFailure,
  } from '../actions/getRequestActions';
import {
    fetchDataPostRequest,
    fetchDataPostSuccess,
    fetchDataPostFailure
} from '../actions/postRequestActions';

export const fetchDataGet = (token, url, key) => async (dispatch) => {
    dispatch(fetchDataGetRequest({ key }));
    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',    
            }
        });//'https://localhost:7086/account/get-current-user'
        
        if (!response.ok) {
            const error = new Error('HTTP error');
            error.code = response.status;
            throw error;
        }
        //
        const data = await response.json();
        dispatch(fetchDataGetSuccess({ key, data }));
    } catch (error) {
        dispatch(fetchDataGetFailure({ key, error: error.code }));
    }
};

export const fetchDataPost = (token, url, key, data) => async (dispatch) => {
    dispatch(fetchDataPostRequest({ key }));
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',    
            },
            body: JSON.stringify(data)
        });
        
        if (!response.ok) {
            const error = new Error('HTTP error');
            error.code = response.status;
            throw error;
        }
        //
        const data = await response.json();
        dispatch(fetchDataPostSuccess({ key, data }));
    } catch (error) {
        dispatch(fetchDataPostFailure({ key, error: error.code }));
    }
};