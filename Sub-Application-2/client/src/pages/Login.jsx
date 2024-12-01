import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { login as loginApi } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/shared/AuthContext';
import { toast } from 'react-toastify';
import log from '../logger';

const LoginUser = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const { login } = useAuth(); // Get login function from AuthContext
    const navigate = useNavigate();
    const notifyLoginSucsess = () => toast.success('Login successful!'); // Notify user of successful login

    const handleSubmit = async (event) => {
        event.preventDefault(); // Prevent form's default submission behavior
        try {
            setError('');
            const { token: jwtToken } = await loginApi(username, password);
            login(jwtToken, username); // Update auth context with new token and username

            // Clear the form fields and display success message
            setUsername('');
            setPassword('');
            notifyLoginSucsess();

            navigate(`/`); // Redirect to home page
        } catch (err) {
            setError(err);
        }
    };

    return (
        <div className="container mt-4">
            <h2>Login</h2>
            {error && <Alert variant="danger">{error}</Alert>}{' '}
            {/* Display error message if any */}
            <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3" controlId="formUsername">
                    <Form.Label>Username</Form.Label>
                    {/* input for username */}
                    <Form.Control
                        type="text"
                        placeholder="Enter username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                </Form.Group>

                <Form.Group className="mb-3" controlId="formPassword">
                    <Form.Label>Password</Form.Label>
                    {/* input for password */}
                    <Form.Control
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </Form.Group>

                {/* Login button and new user navigation */}
                <Button variant="primary" type="submit">
                    Login
                </Button>
                <Button
                    variant="light"
                    onClick={() => {
                        log.info('Navigating to register page.');
                        navigate('/register');
                    }}>
                    New User?
                </Button>
            </Form>
        </div>
    );
};

export default LoginUser;
