import React, { useState } from 'react';

interface LoginFormData {
  email: string;
  password: string;
}

const Login: React.FC = () => {
  // Define state to manage form inputs
  const [formData, setFormData] = useState<LoginFormData>({
    email: '',
    password: '',
  });

  // Handle input changes
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value, // Dynamically update the correct field (email or password)
    });
  };

  // Handle form submission
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log('Form submitted with data:', formData);
    
    // You can now send `formData` to the server or perform any action you need
    // Example: submit formData to API, perform validation, etc.
  };

  return (
    <>
      <h1>Login her din mongo</h1>

      <form onSubmit={handleSubmit} className='form'>
        {/* Email input */}
        <div className="mb-3">
          <label htmlFor="email" className='form-label'>Email:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>

        {/* Password input */}
        <div className="mb-3">
          <label htmlFor="password" className='form-label'>Password:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>

        {/* Submit button */}
        <button type="submit" className='btn btn-success'>Submit</button>
        <a href='' className='btn btn-light'>Forgot password?</a> 
        <a className='btn btn-light'>New User?</a>
      </form>
    </>
  );
};

export default Login;
