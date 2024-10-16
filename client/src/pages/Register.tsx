import React, { useState } from 'react';


interface RegisterFormData {
    name: string;
    surname: string;
    email: string;
    phonenumber: string;
    password: string;
    confirmpassword: string;
}

const Register: React.FC = () => {
    // Define state to manage form inputs
    const [formData, setFormData] = useState<RegisterFormData>({
      name: '',
      surname: '',
      email: '',
      phonenumber: '',
      password: '',
      confirmpassword: '',
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value} = e.target;
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
    <h1>Register new account</h1>

    <form onSubmit={handleSubmit} className='form'>
        {/* Name input */}
        <div className="mb-3">
          <label htmlFor="name" className='form-label'>Name:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>

        {/* Surname input */}
        <div className="mb-3">
          <label htmlFor="surname" className='form-label'>Surname:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="text"
            id="surname"
            name="surname"
            value={formData.surname}
            onChange={handleChange}
            required
          />
        </div>

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

        {/* Phonenumber input */}
        <div className="mb-3">
          <label htmlFor="phonenumber" className='form-label'>Phonenumber:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="phonenumber"
            id="phonenumber"
            name="phonenumber"
            value={formData.phonenumber}
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

        {/* Confirm Password input */}
        <div className="mb-3">
          <label htmlFor="confirmpassword" className='form-label'>Confirm password:</label>
          <input
            style={{ maxWidth: '300px', width: '100%' }}
            className="form-control"
            type="password"
            id="confirmpassword"
            name="confirmpassword"
            value={formData.confirmpassword}
            onChange={handleChange}
            required
          />
        </div>
    {/* Register button */}
    <button type="submit" className='btn btn-success'>Register</button>
      </form>
    </>
  );
};

export default Register;
