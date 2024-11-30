import axios from 'axios';

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
        return Promise.reject(error);
    }
);

// Function to create a new comment
export const createComment = async (commentData) => {
    try {
        // Send a POST request to create a comment
        const response = await axiosInstance.post('/Comments', commentData); 
        return response.data;
    } catch (error) {
        console.error('Error creating comment:', error);
        throw error.response?.data || 'Failed to send comment'; 
    }
};

// Function to fetch comments for a specific post
export const fetchCommentsForPost = async (postId) => {
    try {
        // Use a GET request to fetch comments for a given post ID
        const response = await axios.get(
            `http://localhost:5229/api/Comments/post/${postId}`, 
        );
        return response.data;
    } catch (error) {
        console.error('Error fetching comments:', error); 
        throw error; 
    }
};

// Function to delete a specific comment
export const deleteComment = async (commentId) => {
    try {
        // Send a DELETE request to remove the comment by ID
        const response = await axiosInstance.delete(`/Comments/${commentId}`);
        return response.data;
    } catch (error) {
        throw error.response?.data || 'Failed to delete comment';
    }
};
