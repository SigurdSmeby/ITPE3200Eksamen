import axios from 'axios';

const API_URL = 'http://localhost:5229/api/';

// Get the current user profile
export const getUserProfile = () => {
    const token = localStorage.getItem('jwtToken');
    return axios
        .get(`${API_URL}Users/profile`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
        .catch((error) => {
            console.error('Error fetching user profile:', error);
            throw error;
        });
};

// Update the current user profile
export const updateUserProfile = (data) => {
    const token = localStorage.getItem('jwtToken');
    return axios
        .put(`${API_URL}Users/profile`, data, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
        .catch((error) => {
            console.error('Error updating profile:', error);
            throw error;
        });
};

// Change the user's password
export const changeUserPassword = (data) => {
    const token = localStorage.getItem('jwtToken');
    return axios
        .put(`${API_URL}Users/change-password`, data, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
        .catch((error) => {
            console.error('Error changing password:', error);
            throw error.response.data;
        });
};
