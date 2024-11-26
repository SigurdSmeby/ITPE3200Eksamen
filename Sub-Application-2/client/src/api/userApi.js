import axios from 'axios';

// Create an axios instance with common configurations
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api',
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`,
        'Content-Type': 'multipart/form-data', // Optional: Can add other common headers
    },
});
// Add a request interceptor to include the token dynamically
axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Get the current user profile
export const getUserProfile = async () => {
    const response = await axiosInstance.get('/Users/profile');
    return response;
};

// Update the current user profile
export const updateUserProfile = async (data) => {
    const response = await axiosInstance.put('/Users/profile', data);
    return response;
};

// Change the user's password
export const changeUserPassword = async (passwordData) => {
    const response = await axiosInstance.put('/Users/change-password', passwordData, {
        headers: {
            'Content-Type': 'application/json', // Explicitly set the Content-Type header
        },
    });
    return response;
};


// Delete the user's account
export const deleteUserAccount = async () => {
    const response = await axiosInstance.delete('/Users/delete-account');
    return response;
};