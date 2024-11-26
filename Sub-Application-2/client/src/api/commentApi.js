import axios from 'axios';

// Create an axios instance with common configurations
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api', // Replace with your backend URL
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`,
        'Content-Type': 'application/json',
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

// Create a new comment
export const createComment = async (commentData) => {
    try {
        const response = await axiosInstance.post('/Comments', commentData);
        return response.data; // Returns server response (e.g., success message)
    } catch (error) {
        throw error.response?.data || 'Failed to send comment'; // Throw error to handle in UI
    }
};

export const fetchCommentsForPost = async (postId) => {
    try {
        const response = await axios.get(
            `http://localhost:5229/api/Comments/post/${postId}`,
        );
        return response.data; // Directly return the array of comments
    } catch (error) {
        console.error('Error fetching comments:', error);
        throw error;
    }
};

export const deleteComment = async (commentId) => {
    try {
        const response = await axiosInstance.delete(`/Comments/${commentId}`);
        return response.data; // Return server response
    } catch (error) {
        throw error.response?.data || 'Failed to delete comment';
    }
};