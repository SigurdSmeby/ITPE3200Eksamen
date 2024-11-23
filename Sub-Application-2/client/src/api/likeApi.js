import axios from 'axios';

// Create an axios instance with common configurations
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api/Likes/', // Replace with your backend URL
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`,
        'Content-Type': 'application/json',
    },
});

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

// Create a like
export const createLike = async (postId) => {
    try {
        const response = await axiosInstance.post('like/'+postId);
        return response.data; // Returns server response (e.g., success message)
    } catch (error) {
        throw error.response?.data || 'Failed to send comment'; // Throw error to handle in UI
    }
};

// Create a like
export const unLike = async (postId) => {
    try {
        const response = await axiosInstance.delete('unlike/'+postId);
        return response.data; // Returns server response (e.g., success message)
    } catch (error) {
        throw error.response?.data || 'Failed to send unlike'; // Throw error to handle in UI
    }
};

export const checkIfUserHasLikedPost = async (postId) => {
    try {
        const response = await axiosInstance.get('hasLiked/'+postId);
        console.log('Check liked status response:', response.data, postId);
        return response.data;
    } catch (error) {
        throw error.response?.data || 'Failed to check like status';
    }
};