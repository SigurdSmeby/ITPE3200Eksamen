import axios from 'axios';
import log from '../logger';

// Create a reusable Axios instance with a base URL and default headers
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api', // API base URL
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`, // JWT token from localStorage
        'Content-Type': 'application/json', // Default content type for requests
    },
});

// Add an interceptor to include the token in every request if it exists
axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`; // Attach token to request headers
        }
        return config;
    },
    (error) => {
        log.error('Request Interceptor Error:', error);
        return Promise.reject(error);
    },
);

// Function to create a new comment
export const createComment = async (commentData) => {
    log.info('API Call: Create Comment', { commentData });
    try {
        // Send a POST request to create a comment
        const response = await axiosInstance.post('/Comments', commentData);
        log.info('API Success: Create Comment', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Create Comment', { error });
        throw error.response?.data || 'Failed to send comment';
    }
};

// Function to fetch comments for a specific post
export const fetchCommentsForPost = async (postId) => {
    log.info('API Call: Fetch Comments for Post', { postId });
    try {
        // Use a GET request to fetch comments for a given post ID
        const response = await axios.get(
            `http://localhost:5229/api/Comments/post/${postId}`,
        );
        log.info('API Success: Fetch Comments for Post', {
            postId,
            data: response.data,
        });
        return response.data;
    } catch (error) {
        log.error('API Error: Fetch Comments for Post', { postId, error });
        throw error;
    }
};

// Function to delete a specific comment
export const deleteComment = async (commentId) => {
    log.info('API Call: Delete Comment', { commentId });
    try {
        // Send a DELETE request to remove the comment by ID
        const response = await axiosInstance.delete(`/Comments/${commentId}`);
        log.info('API Success: Delete Comment', { commentId });
        return response.data;
    } catch (error) {
        log.error('API Error: Delete Comment', { commentId, error });
        throw error.response?.data || 'Failed to delete comment';
    }
};
