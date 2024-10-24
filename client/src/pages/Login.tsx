import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { login } from '../api/authApi';

const LoginUser = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [token, setToken] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setError('');
      setSuccess('');

      // Call the login function and get both the token and message
      const { token: jwtToken } = await login(username, password);

      // Set the success message (if provided by the backend)
      setSuccess('Login successful!');

      // Store the token in local storage and state
      localStorage.setItem('jwtToken', jwtToken);
      setToken(jwtToken);

      // Clear form fields
      setUsername('');
      setPassword('');
    } catch (err) {
      setError(err);
    }
  };


  return (
    <div className="container mt-4">
      <h2>Login</h2>

      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

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

        <Button variant="primary" type="submit">Login</Button>
        <a href='' className='btn btn-light'>Forgot password?</a> 
        <a href="/register" className='btn btn-light'>New User?</a>
        <a href="/test">test</a>
      </Form>

      {token && (
        <div className="mt-3">
          <h5>Your JWT Token:</h5>
          <code>{token}</code>
        </div>
      )}
    </div>
  );
};

export default LoginUser;
