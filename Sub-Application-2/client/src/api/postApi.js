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
    },
);

// Fetch all users
export const getAllUsers = async () => {
    try {
        const response = await axiosInstance.get('/Users/all');
        return response.data; // Returns the array of users
    } catch (error) {
        console.error('Error fetching users:', error);
        throw error;
    }
};


// Get paginated posts
export const getPosts = async (pageNumber, pageSize) => {
    const response = await axiosInstance.get('/Posts', {
        params: { pageNumber, pageSize }, // Use 'pageNumber' to match the backend
    });
    return response.data;
};

// Get all posts by a user
export const getUserPosts = async (username, pageNumber = 1, pageSize = 10) => {
    const response = await axiosInstance.get(`/Posts/user/${username}`, {
        params: { pageNumber, pageSize },
    });
    return response.data;
};


// Create a new post
export const createPost = async (postData) => {
    const response = await axiosInstance.post('/Posts', postData); // Reuse the axios instance
    return response.data;
};

// Delete a post
export const deletePost = async (postId) => {
    const response = await axiosInstance.delete(`/Posts/${postId}`); // Reuse the axios instance
    return response.data;
};

// Update a post
export const updatePost = async (postId, postData) => {
    const response = await axiosInstance.put(`/Posts/${postId}`, postData); // Reuse the axios instance
    return response.data;
};

// Get a single post
export const getPost = async (postId) => {
    const response = await axiosInstance.get(`/Posts/${postId}`); // Reuse the axios instance
    return response.data;
};
