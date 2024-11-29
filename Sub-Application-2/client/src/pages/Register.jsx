import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { register } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { login as loginApi } from '../api/authApi';
import { useAuth } from '../components/shared/AuthContext';

const RegisterUser = () => {
    // State for managing form inputs
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
    });

    // State for handling errors
    const [error, setError] = useState('');

    const { login } = useAuth(); // Access login method from AuthContext
    const navigate = useNavigate();

    // Success notification
    const registerSuccess = () => toast.success("Profile created successfully!");

    // Handle form submission
    const handleSubmit = async (event) => {
        event.preventDefault();

        // Validate that passwords match
        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        try {
            setError(''); // Clear any previous errors

            // Register the user
            await register(formData.username, formData.email, formData.password);

            // Log in the user automatically
            const { token: jwtToken } = await loginApi(formData.username, formData.password);
            login(jwtToken, formData.username); // Update auth context with token

            registerSuccess(); // Notify success
            navigate(`/`); // Redirect to home
        } catch (err) {
            // Display error message from API or fallback message
            setError(err?.response?.data?.message || 'Registration failed');
        }
    };

    // Handle input changes for form fields
    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    return (
        <div className="container mt-4">
            <h2 className="text-center">Register</h2>

            {/* Display error if present */}
            {error && <Alert variant="danger">{error}</Alert>}

            <Form onSubmit={handleSubmit}>
                {/* Username Field */}
                <Form.Group className="mb-3" controlId="formUsername">
                    <Form.Label>Username</Form.Label>
                    <Form.Control
                        type="text"
                        name="username" // Identifier for handleChange
                        placeholder="Enter username"
                        value={formData.username}
                        onChange={handleChange} // Update state on input
                        required
                    />
                </Form.Group>

                {/* Email Field */}
                <Form.Group className="mb-3" controlId="formEmail">
                    <Form.Label>Email address</Form.Label>
                    <Form.Control
                        type="email"
                        name="email"
                        placeholder="Enter email"
                        value={formData.email}
                        onChange={handleChange}
                        required
                    />
                </Form.Group>

                {/* Password Field */}
                <Form.Group className="mb-3" controlId="formPassword">
                    <Form.Label>Password</Form.Label>
                    <Form.Control
                        type="password"
                        name="password"
                        placeholder="Password"
                        value={formData.password}
                        onChange={handleChange}
                        required
                    />
                </Form.Group>

                {/* Confirm Password Field */}
                <Form.Group className="mb-3" controlId="formConfirmPassword">
                    <Form.Label>Confirm Password</Form.Label>
                    <Form.Control
                        type="password"
                        name="confirmPassword"
                        placeholder="Confirm Password"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        required
                    />
                </Form.Group>

                {/* Submit and Login Buttons */}
                <div className="btn-group mt-3">
                    <Button variant="primary" type="submit">
                        Register
                    </Button>
                    <a href="/login" className="btn btn-light">
                        Login
                    </a>
                </div>
            </Form>
        </div>
    );
};

export default RegisterUser;
