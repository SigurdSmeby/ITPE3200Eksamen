// LoginUser.tsx
import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { login as loginApi } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/shared/AuthContext';
import { toast } from 'react-toastify';

const LoginUser = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const { login } = useAuth(); // Get login function from AuthContext
    const navigate = useNavigate();
    const notifyLoginSucsess = () => toast.success("Login successful!"); // Notify user of successful login

    const handleSubmit = async (event) => {
        event.preventDefault(); // Prevent form's default submission behavior

        try {
            setError('');

            const { token: jwtToken } = await loginApi(username, password);

            login(jwtToken, username); // Update auth context with new token and username

            setUsername('');
            setPassword('');

            notifyLoginSucsess();

            navigate(`/`); // Redirect to home page
        } catch (err) {
            setError(err); // Set error state to display the error message
        }
    };

    return (
        <div className="container mt-4">
            <h2>Login</h2>

            {error && <Alert variant="danger">{error}</Alert>} {/* Display error message if any */}

            <Form onSubmit={handleSubmit}>
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

                <Button variant="primary" type="submit">
                    Login
                </Button>
                <Button variant="light" onClick={() => navigate('/register')}>
                    New User?
                </Button>
            </Form>
        </div>
    );
};

export default LoginUser;
