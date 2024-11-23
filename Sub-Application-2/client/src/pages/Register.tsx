import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { register } from '../api/authApi';
import { useNavigate } from 'react-router-dom';

const RegisterUser = () => {
    // Form state
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const navigate = useNavigate();

    // Handle form submission
    const handleSubmit = async (event) => {
        event.preventDefault();
        if (password !== confirmPassword) {
            setSuccess('');
            setError('Passwords do not match');
            return;
        }

        try {
            // Reset error and success messages
            setError('');
            setSuccess('');

            // API request to register a new user
            const {} = await register(username, email, password);
            setSuccess('Registration successful!');
            setUsername('');
            setEmail('');
            setPassword('');
            setConfirmPassword('');
            // Redirect to home page after a short delay
            setTimeout(() => {
                navigate(`/login`);
            }, 1000);
        } catch (err) {
            setError(err);
        }
    };

    return (
        <div className="container mt-4">
            <h2>Register</h2>

            {error && <Alert variant="danger">{error}</Alert>}
            {success && <Alert variant="success">{success}</Alert>}

            <Form onSubmit={handleSubmit}>
                {/* Username Field */}
                <Form.Group className="mb-3" controlId="formUsername">
                    <Form.Label>Username</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder="Enter username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                </Form.Group>

                {/* Email Field */}
                <Form.Group className="mb-3" controlId="formEmail">
                    <Form.Label>Email address</Form.Label>
                    <Form.Control
                        type="email"
                        placeholder="Enter email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </Form.Group>

                {/* Password Field */}
                <Form.Group className="mb-3" controlId="formPassword">
                    <Form.Label>Password</Form.Label>
                    <Form.Control
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </Form.Group>
                <Form.Group className="mb-3" controlId="formConfirmPassword">
                    <Form.Label>Confirm Password</Form.Label>
                    <Form.Control
                        type="password"
                        placeholder="Confirm Password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                    />
                </Form.Group>

                {/* Submit Button */}
                <Button variant="primary" type="submit">
                    Register
                </Button>
                <a href="/login" className="btn btn-light">
                    Login
                </a>
            </Form>
        </div>
    );
};

export default RegisterUser;