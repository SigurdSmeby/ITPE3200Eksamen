// PrivateRoute.tsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './shared/AuthContext';

const PrivateRoute = ({ children }) => {
    const { isLoggedIn } = useAuth();

    if (!isLoggedIn) {
        // If not logged in, redirect to login page
        return <Navigate to="/login" />;
    }

    // Otherwise, render the children (the protected component)
    return children;
};

export default PrivateRoute;
