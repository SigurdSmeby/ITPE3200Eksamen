import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { register } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { login as loginApi } from '../api/authApi';
import { useAuth } from '../components/shared/AuthContext';

const RegisterUser = () => {
    // State for handling form data
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
    });
    const [error, setError] = useState('');
    const registerSuccess = () => toast.success("Profile created successfully!");
    const { login } = useAuth(); // Access login method from AuthContext
    const navigate = useNavigate();

    // Password requirements regex
    const passwordRequirements = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

    // Handle form submission
    const handleSubmit = async (event) => {
        event.preventDefault();

        // Validate password requirements
        if (!passwordRequirements.test(formData.password)) {
            setError(
                'Password must be at least 8 characters long, include at least one uppercase letter, one lowercase letter, one number, and one special character.'
            );
            return;
        }

        // Validate that passwords match
        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        try {
            setError('');

            // Register the user
            await register(formData.username, formData.email, formData.password);

            // Log in the user automatically
            const { token: jwtToken } = await loginApi(formData.username, formData.password);
            login(jwtToken, formData.username); // Update auth context with token

            registerSuccess(); // Display success message
            navigate(`/`); // Redirect to home
        } catch (err) {
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
                        onChange={handleChange}
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
