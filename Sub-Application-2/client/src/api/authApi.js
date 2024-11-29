import axios from 'axios';


const API_URL = 'http://localhost:5229/api/Users'; // Base URL for the API

// Login function
export const login = async (username, password) => {
    try {
        const response = await axios.post(`${API_URL}/login`, {
            username,
            password,
        });
        return { token: response.data.token };
    } catch (err) {
        if (err.response && err.response.data) {
            throw err.response.data;
        } else {
            throw new Error('An error occurred during login.');
        }
    }
};

// Register function
export const register = async (username, email, password) => {
    try {
        const response = await axios.post(`${API_URL}/register`, {
            username,
            email,
            password,
        });
        return response.data;
    } catch (err) {
        if (err.response && err.response.data) {
            throw err.response.data;
        } else {
            throw new Error('An error occurred during registration.');
        }
    }
};
