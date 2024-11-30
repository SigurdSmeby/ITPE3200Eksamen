import axios from 'axios';

// Set the base URL for the API
const API_URL = 'http://localhost:5229/api/Users'; 

// Function to handle user login
export const login = async (username, password) => {
    try {
        // Send a POST request with username and password
        const response = await axios.post(`${API_URL}/login`, {
            username,
            password,
        });
        // Return the token from the response
        return { token: response.data.token };
    } catch (err) {
        // Handle errors, throwing a meaningful error message
        if (err.response && err.response.data) {
            throw err.response.data;
        } else {
            throw new Error('An error occurred during login.');
        }
    }
};

// Function to handle user registration
export const register = async (username, email, password) => {
    try {
        // Send a POST request with registration details
        const response = await axios.post(`${API_URL}/register`, {
            username,
            email,
            password,
        });
        // Return the response data indicating success
        return response.data;
    } catch (err) {
        // Handle errors, throwing a meaningful error message
        if (err.response && err.response.data) {
            throw err.response.data;
        } else {
            throw new Error('An error occurred during registration.');
        }
    }
};
