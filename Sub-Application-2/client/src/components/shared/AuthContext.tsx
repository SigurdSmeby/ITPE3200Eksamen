// AuthContext.tsx
import React, { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

interface AuthContextProps {
    isLoggedIn: boolean;
    username: string | null;
    login: (token: string, username: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextProps>({
    isLoggedIn: false,
    username: null,
    login: () => {},
    logout: () => {},
});

let logoutTimer: ReturnType<typeof setTimeout>;

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
    children,
}) => {
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(
        !!localStorage.getItem('jwtToken'),
    );
    const [username, setUsername] = useState<string | null>(
        localStorage.getItem('username'),
    );

    // Helper function to set the auto-logout timer
    const setLogoutTimer = (expirationTime: number) => {
        const currentTime = Date.now();
        const timeUntilExpiration = expirationTime * 1000 - currentTime; // Convert to milliseconds

        // Clear any existing timer
        if (logoutTimer) clearTimeout(logoutTimer);

        // Set a new timer to log out the user when the token expires
        logoutTimer = setTimeout(() => {
            logout();
            alert('Your session has expired. Please log in again.');
        }, timeUntilExpiration);
    };

    const login = (token: string, username: string) => {
        localStorage.setItem('jwtToken', token);
        localStorage.setItem('username', username);
        setIsLoggedIn(true);
        setUsername(username);

        // Decode token to get expiration time and set auto-logout timer
        const decodedToken: any = jwtDecode(token);
        if (decodedToken.exp) {
            setLogoutTimer(decodedToken.exp);
        }
    };

    const logout = () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
        setIsLoggedIn(false);
        setUsername(null);

        // Clear the logout timer if it exists
        if (logoutTimer) clearTimeout(logoutTimer);
    };

    useEffect(() => {
        // Check if a token exists on app load and set the logout timer if so
        const token = localStorage.getItem('jwtToken');
        if (token) {
            const decodedToken: any = jwtDecode(token);
            if (decodedToken.exp && Date.now() / 1000 < decodedToken.exp) {
                setLogoutTimer(decodedToken.exp);
            } else {
                logout(); // Token has expired, so log the user out
            }
        }
    }, []);

    return (
        <AuthContext.Provider value={{ isLoggedIn, username, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
