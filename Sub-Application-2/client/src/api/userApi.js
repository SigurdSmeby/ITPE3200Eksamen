import axios from 'axios';

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
    (error) => Promise.reject(error)
);

// Get the current user profile
export const getUserProfile = async () => {
    return await axiosInstance.get('/Users/profile');
};

// Update the current user profile
export const updateUserProfile = async (data) => {
    return await axiosInstance.put('/Users/profile', data);
};

// Change the user's password
export const changeUserPassword = async (passwordData) => {
    return await axiosInstance.put('/Users/change-password', passwordData, {
        headers: {
            'Content-Type': 'application/json', // Explicitly set the Content-Type header
        },
    });
};

// Delete the user's account
export const deleteUserAccount = async () => {
    return await axiosInstance.delete('/Users/delete-account');
};
