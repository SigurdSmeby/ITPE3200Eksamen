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
    const { login } = useAuth(); // Use login function from AuthContext
    const navigate = useNavigate();
    const notifyLoginSucsess = () => toast.success("Login successful!");

    const handleSubmit = async (event) => {
        //hindrer default behavior til formen
        event.preventDefault();

        try {
            setError('');

            const { token: jwtToken } = await loginApi(username, password);

            // Call the login function from AuthContext to set state, localStorage, and timer
            login(jwtToken, username);

            setUsername('');
            setPassword('');

            notifyLoginSucsess();

            // Redirect to home page after a short delay
            navigate(`/`);
            
        } catch (err) {
            setError(err);
        }
    };

    return (
        <div className="container mt-4">
            <h2>Login</h2>

            {error && <Alert variant="danger">{error}</Alert>}

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
                <a href="/register" className="btn btn-light">
                    New User?
                </a>
            </Form>
        </div>
    );
};

export default LoginUser;
