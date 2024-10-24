import axios from 'axios';

// Create an axios instance with common configurations
const axiosInstance = axios.create({
  baseURL: 'http://localhost:5229/api',
  headers: {
    Authorization: `Bearer ${localStorage.getItem('jwtToken')}`,
    'Content-Type': 'application/json', // Optional: Can add other common headers
  },
});

// Get all posts
export const getPosts = async () => {
  const response = await axiosInstance.get('/Posts'); // Reuse the axios instance
  return response.data;
};

// get all posts by a user api/Posts/user/{userId}
// returns all posts, username and profile picture of the user
export const getUserPosts = async (userId) => {
  const response = await axiosInstance.get(`/Posts/user/${userId}`); // Reuse the axios instance
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
