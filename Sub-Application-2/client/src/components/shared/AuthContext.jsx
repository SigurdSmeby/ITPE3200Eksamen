import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext({
    isLoggedIn: false,
    username: null,
    login: () => {},
    logout: () => {},
    deleteAccount: () => {}, // Added deleteAccount method to the context
});

let logoutTimer;

export const AuthProvider = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(
        !!localStorage.getItem('jwtToken')
    );
    const [username, setUsername] = useState(
        localStorage.getItem('username')
    );

    // Define logout first
    const logout = useCallback(() => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
        setIsLoggedIn(false);
        setUsername(null);

        // Clear the logout timer if it exists
        if (logoutTimer) clearTimeout(logoutTimer);
    }, []);

    // Then define setLogoutTimer
    const setLogoutTimer = useCallback((expirationTime) => {
        const currentTime = Date.now();
        const timeUntilExpiration = expirationTime * 1000 - currentTime; // Convert to milliseconds

        // Clear any existing timer
        if (logoutTimer) clearTimeout(logoutTimer);

        // Set a new timer to log out the user when the token expires
        logoutTimer = setTimeout(() => {
            logout(); // Now logout is defined and accessible
            alert('Your session has expired. Please log in again.');
        }, timeUntilExpiration);
    }, [logout]);

    const login = (token, username) => {
        localStorage.setItem('jwtToken', token);
        localStorage.setItem('username', username);
        setIsLoggedIn(true);
        setUsername(username);

        // Decode token to get expiration time and set auto-logout timer
        const decodedToken = jwtDecode(token);
        if (decodedToken.exp) {
            setLogoutTimer(decodedToken.exp);
        }
    };

    const deleteAccount = () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
    
        if (logoutTimer) clearTimeout(logoutTimer);
    
        setIsLoggedIn(false);
        setUsername(null); 
    };

    useEffect(() => {
        // Check if a token exists on app load and set the logout timer if so
        const token = localStorage.getItem('jwtToken');
        if (token) {
            const decodedToken = jwtDecode(token);
            if (decodedToken.exp && Date.now() / 1000 < decodedToken.exp) {
                setLogoutTimer(decodedToken.exp);
            } else {
                logout(); // Token has expired, so log the user out
            }
        }
    }, [setLogoutTimer, logout]); // Include setLogoutTimer and logout

    return (
        <AuthContext.Provider
            value={{ isLoggedIn, username, login, logout, deleteAccount }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
