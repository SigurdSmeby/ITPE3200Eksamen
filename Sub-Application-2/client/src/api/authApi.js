import axios from 'axios';
import log from '../logger';

// Set the base URL for the API
const API_URL = 'http://localhost:5229/api/Users';

// Function to handle user login
export const login = async (username, password) => {
    log.info('Login API Call initiated.', { username });
    try {
        // Send a POST request with username and password
        const response = await axios.post(`${API_URL}/login`, {
            username,
            password,
        });
        log.info('Login API Call successful.', { username });
        log.debug('Login API Response:', response.data);
        // Return the token from the response
        return { token: response.data.token };
    } catch (err) {
        // Handle errors, throwing a meaningful error message
        if (err.response && err.response.data) {
            log.error('Login API Call failed with server error.', {
                username,
                error: err.response.data,
            });
            throw err.response.data;
        } else {
            log.error('Login API Call failed with network or unknown error.', {
                username,
                error: err.message,
            });
            throw new Error('An error occurred during login.');
        }
    }
};

// Function to handle user registration
export const register = async (username, email, password) => {
    log.info('Register API Call initiated.', { username, email });
    try {
        // Send a POST request with registration details
        const response = await axios.post(`${API_URL}/register`, {
            username,
            email,
            password,
        });
        log.info('Register API Call successful.', { username, email });
        log.debug('Register API Response:', response.data);
        // Return the response data indicating success
        return response.data;
    } catch (err) {
        // Handle errors, throwing a meaningful error message
        if (err.response && err.response.data) {
            log.error('Register API Call failed with server error.', {
                username,
                email,
                error: err.response.data,
            });
            throw err.response.data;
        } else {
            log.error(
                'Register API Call failed with network or unknown error.',
                { username, email, error: err.message },
            );
            throw new Error('An error occurred during registration.');
        }
    }
};
