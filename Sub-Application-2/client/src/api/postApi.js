import axios from 'axios';
import log from '../logger';

// Create a reusable Axios instance with base URL and default headers
const axiosInstance = axios.create({
    baseURL: 'http://localhost:5229/api', // API base URL
    headers: {
        Authorization: `Bearer ${localStorage.getItem('jwtToken')}`, // JWT token from localStorage
        'Content-Type': 'multipart/form-data', // Content type for handling form data
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

// Function to fetch all users
export const getAllUsers = async () => {
    log.info('API Call: Get All Users');
    try {
        // Send a GET request to retrieve all users
        const response = await axiosInstance.get('/Users/all');
        log.info('API Success: Get All Users', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Get All Users', { error });
        throw error;
    }
};

// Function to fetch posts with pagination
export const getPosts = async (pageNumber, pageSize) => {
    log.info('API Call: Get Posts', { pageNumber, pageSize });
    // Send a GET request with pagination parameters
    try {
        const response = await axiosInstance.get('/Posts', {
            params: { pageNumber, pageSize },
        });
        log.info('API Success: Get Posts', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Get Posts', { error });
        throw error;
    }
};

// Function to fetch posts by a specific user with pagination
export const getUserPosts = async (username, pageNumber = 1, pageSize = 10) => {
    log.info('API Call: Get User Posts', { username, pageNumber, pageSize });
    // Send a GET request to fetch posts by username
    try {
        const response = await axiosInstance.get(`/Posts/user/${username}`, {
            params: { pageNumber, pageSize },
        });
        log.info('API Success: Get User Posts', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Get User Posts', { error });
        throw error;
    }
};

// Function to create a new post
export const createPost = async (postData) => {
    log.info('API Call: Create Post', { postData });
    // Send a POST request with post data (e.g., text, image)
    try {
        const response = await axiosInstance.post('/Posts', postData);
        log.info('API Success: Create Post', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Create Post', { error });
        throw error;
    }
};

// Function to delete a post by ID
export const deletePost = async (postId) => {
    log.info('API Call: Delete Post', { postId });
    // Send a DELETE request to remove a post by ID
    try {
        const response = await axiosInstance.delete(`/Posts/${postId}`);
        log.info('API Success: Delete Post', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Delete Post', { error });
        throw error;
    }
};

// Function to update a post by ID
export const updatePost = async (postId, postData) => {
    log.info('API Call: Update Post', { postId, postData });
    // Send a PUT request with updated post data
    try {
        const response = await axiosInstance.put(`/Posts/${postId}`, postData);
        log.info('API Success: Update Post', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Update Post', { error });
        throw error;
    }
};

// Function to fetch a single post by ID
export const getPost = async (postId) => {
    log.info('API Call: Get Post', { postId });
    // Send a GET request to retrieve a post by ID
    try {
        const response = await axiosInstance.get(`/Posts/${postId}`);
        log.info('API Success: Get Post', response.data);
        return response.data;
    } catch (error) {
        log.error('API Error: Get Post', { error });
        throw error;
    }
};
