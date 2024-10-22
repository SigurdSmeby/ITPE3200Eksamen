import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import axios from 'axios';

const LoginUser = () => {
  // Form state
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [token, setToken] = useState('');

  // Handle form submission
  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      // Reset error and success messages
      setError('');
      setSuccess('');

      // API request to log in the user
      const response = await axios.post('http://localhost:5229/api/Users/login', {
        username,
        password,
      });

      // Handle success
      if (response.status === 200) {
        setSuccess('Login successful!');
        
        // Extract token from response
        const jwtToken = response.data.token;

        // Save the token in localStorage for future requests
        localStorage.setItem('jwtToken', jwtToken);

        // Save token to state for display (optional)
        setToken(jwtToken);

        // Clear form fields
        setUsername('');
        setPassword('');
      }
    } catch (err) {
      // Handle error
      if (err.response && err.response.data) {
        setError(err.response.data);
      } else {
        setError('An error occurred during login.');
      }
    }
  };

  return (
    <div className="container mt-4">
      <h2>Login</h2>

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

        {/* Submit Button */}
        <Button variant="primary" type="submit">
          Login
        </Button>
        <a href='' className='btn btn-light'>Forgot password?</a> 
        <a href="/register" className='btn btn-light'>New User?</a>
        <a href="/test">test</a>
      </Form>

      {/* Display Token (For debugging or further use) */}
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
