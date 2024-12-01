import axios from 'axios';
import log from '../logger';

// Axios instance with base URL and headers
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api',
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`,
        'Content-Type': 'multipart/form-data',
    },
});

// Add an interceptor to dynamically set the token in headers
axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        log.error('Request Interceptor Error:', error);
        Promise.reject(error);
    },
);

// Get the current user profile
export const getUserProfile = async () => {
    log.info('API Call: Get User Profile');
    try {
        const response = await axiosInstance.get('/Users/profile');
        log.info('API Success: Get User Profile', response);
        return response;
    } catch (error) {
        log.error('API Error: Get User Profile', { error });
        throw error;
    }
};

// Update the current user profile
export const updateUserProfile = async (data) => {
    log.info('API Call: Update User Profile', { data });
    try {
        const response = await axiosInstance.put('/Users/profile', data);
        log.info('API Success: Update User Profile', response);
        return response;
    } catch (error) {
        log.error('API Error: Update User Profile', { error });
        throw error;
    }
};

// Change the user's password
export const changeUserPassword = async (passwordData) => {
    log.info('API Call: Change User Password');
    try {
        const response = await axiosInstance.put(
            '/Users/change-password',
            passwordData,
            {
                headers: {
                    'Content-Type': 'application/json',
                },
            },
        );
        log.info('API Success: Change User Password', response);
        return response;
    } catch (error) {
        log.error('API Error: Change User Password', { error });
        throw error;
    }
};

// Delete the user's account
export const deleteUserAccount = async () => {
    log.info('API Call: Delete User Account');
    try {
        const response = await axiosInstance.delete('/Users/delete-account');
        log.info('API Success: Delete User Account', response);
        return response;
    } catch (error) {
        log.error('API Error: Delete User Account', { error });
        throw error;
    }
};
