import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode'; // Fix: Corrected import for jwtDecode

// Create the AuthContext with default values
const AuthContext = createContext({
    isLoggedIn: false,
    username: null,
    login: () => {},
    logout: () => {},
    deleteAccount: () => {},
});

let logoutTimer;

export const AuthProvider = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('jwtToken'));
    const [username, setUsername] = useState(localStorage.getItem('username'));

    // Logout function
    const logout = useCallback(() => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
        setIsLoggedIn(false);
        setUsername(null);

        if (logoutTimer) clearTimeout(logoutTimer); // Clear any existing timer
    }, []);

    // Set logout timer based on token expiration
    const setLogoutTimer = useCallback((expirationTime) => {
        const currentTime = Date.now();
        const timeUntilExpiration = expirationTime * 1000 - currentTime; // Convert expiration time to milliseconds

        if (logoutTimer) clearTimeout(logoutTimer); // Clear any existing timer

        logoutTimer = setTimeout(() => {
            logout(); // Trigger logout when token expires
            alert('Your session has expired. Please log in again.');
        }, timeUntilExpiration);
    }, [logout]);

    // Login function
    const login = (token, username) => {
        localStorage.setItem('jwtToken', token);
        localStorage.setItem('username', username);
        setIsLoggedIn(true);
        setUsername(username);

        // Decode the token to get the expiration time and set a logout timer
        const decodedToken = jwtDecode(token);
        if (decodedToken.exp) {
            setLogoutTimer(decodedToken.exp);
        }
    };

    // Delete account function
    const deleteAccount = () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');

        if (logoutTimer) clearTimeout(logoutTimer);

        setIsLoggedIn(false);
        setUsername(null);
    };

    // Auto-logout if the token is expired or set a logout timer on app load
    useEffect(() => {
        const token = localStorage.getItem('jwtToken');
        if (token) {
            const decodedToken = jwtDecode(token);
            if (decodedToken.exp && Date.now() / 1000 < decodedToken.exp) {
                setLogoutTimer(decodedToken.exp); // Set timer if token is valid
            } else {
                logout(); // Token expired, log the user out
            }
        }
    }, [setLogoutTimer, logout]);

    return (
        <AuthContext.Provider value={{ isLoggedIn, username, login, logout, deleteAccount }}>
            {children}
        </AuthContext.Provider>
    );
};

// Hook to access authentication context
export const useAuth = () => useContext(AuthContext);
