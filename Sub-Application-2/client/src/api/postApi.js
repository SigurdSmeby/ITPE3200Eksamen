import axios from 'axios';

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
        return Promise.reject(error);
    },
);

// Function to fetch all users
export const getAllUsers = async () => {
    try {
        // Send a GET request to retrieve all users
        const response = await axiosInstance.get('/Users/all');
        return response.data;
    } catch (error) {
        console.error('Error fetching users:', error);
        throw error; 
    }
};

// Function to fetch posts with pagination
export const getPosts = async (pageNumber, pageSize) => {
    // Send a GET request with pagination parameters
    const response = await axiosInstance.get('/Posts', {
        params: { pageNumber, pageSize },
    });
    return response.data;
};

// Function to fetch posts by a specific user with pagination
export const getUserPosts = async (username, pageNumber = 1, pageSize = 10) => {
    // Send a GET request to fetch posts by username
    const response = await axiosInstance.get(`/Posts/user/${username}`, {
        params: { pageNumber, pageSize },
    });
    return response.data;
};

// Function to create a new post
export const createPost = async (postData) => {
    // Send a POST request with post data (e.g., text, image)
    const response = await axiosInstance.post('/Posts', postData);
    return response.data;
};

// Function to delete a post by ID
export const deletePost = async (postId) => {
    // Send a DELETE request to remove a post by ID
    const response = await axiosInstance.delete(`/Posts/${postId}`);
    return response.data;
};

// Function to update a post by ID
export const updatePost = async (postId, postData) => {
    // Send a PUT request with updated post data
    const response = await axiosInstance.put(`/Posts/${postId}`, postData);
    return response.data; 
};

// Function to fetch a single post by ID
export const getPost = async (postId) => {
    // Send a GET request to retrieve a post by ID
    const response = await axiosInstance.get(`/Posts/${postId}`);
    return response.data; 
};
