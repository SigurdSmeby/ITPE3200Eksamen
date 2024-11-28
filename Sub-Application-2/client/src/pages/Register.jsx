import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { register } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { login as loginApi } from '../api/authApi';
import { useAuth } from '../components/shared/AuthContext';


const RegisterUser = () => {
    // Form state
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const registerSucsess = () => toast.success("Profile created successfully!");
    const { login } = useAuth(); // Use login function from AuthContext
    const navigate = useNavigate();

    // Handle form submission
    const handleSubmit = async (event) => {
        event.preventDefault();
        if (password !== confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        try {
            // Reset error and success messages
            setError('');

            // API request to register a new user
            const {} = await register(username, email, password);
            const { token: jwtToken } = await loginApi(username, password);

            // Call the login function from AuthContext to set state, localStorage, and timer
            login(jwtToken, username);
            
            registerSucsess();
            navigate(`/`);
        } catch (err) {
            setError(err);
        }
    };

    return (
        <div className="container mt-4">
            <h2>Register</h2>

            {error && <Alert variant="danger">{error}</Alert>}

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
