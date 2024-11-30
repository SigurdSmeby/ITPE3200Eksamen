import axios from 'axios';

// Create a reusable Axios instance for handling likes with base URL and default headers
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api/Likes/', // API base URL for likes
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`, // JWT token for authentication
        'Content-Type': 'application/json', // Content type for JSON requests
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

// Function to like a post
export const createLike = async (postId) => {
    try {
        // Send a POST request to like a post by ID
        const response = await axiosInstance.post('like/' + postId);
        return response.data; 
    } catch (error) {
        throw error.response?.data || 'Failed to like the post';
    }
};

// Function to unlike a post
export const unLike = async (postId) => {
    try {
        // Send a DELETE request to unlike a post by ID
        const response = await axiosInstance.delete('unlike/' + postId);
        return response.data;
    } catch (error) {
        throw error.response?.data || 'Failed to unlike the post';
    }
};

// Function to check if the user has liked a specific post
export const checkIfUserHasLikedPost = async (postId) => {
    try {
        // Use a GET request to check if the user has liked a post by ID
        const response = await axiosInstance.get('hasLiked/' + postId);
        console.log('Check liked status response:', response.data, postId);
        return response.data;
    } catch (error) {
        throw error.response?.data || 'Failed to check like status';
    }
};
