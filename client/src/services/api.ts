import axios from 'axios';

const API_URL = 'http://localhost:5229/api'; // Bytt til riktig API-url

export const getPosts = async () => {
  try {
    const response = await axios.get(`${API_URL}/posts`);
    return response.data;
  } catch (error) {
    console.error("Error fetching posts", error);
    throw error; // re-throw the error to handle it appropriately
  }
}

export const getUserProfile = async (id: number) => {
  const response = await axios.get(`${API_URL}/users/${id}`);
  return response.data;
};

export const createPost = async (post: any) => {
  const response = await axios.post(`${API_URL}/posts`, post);
  return response.data;
};

export const loginUser = async (email: string, password: string) => {
  try {
    const response = await axios.post(`${API_URL}/Users/login`, { email, password });
    localStorage.setItem('token', response.data.token); // Lagre JWT-token
    return true;
  } catch {
    return false;
  }
};

export const registerUser = async (username: string, email: string, password: string) => {
  try {
    await axios.post(`${API_URL}/Users/register`, { username, email, password });
    return true;
  } catch {
    return false;
  }
};
