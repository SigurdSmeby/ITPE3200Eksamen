import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/shared/AuthContext';
import log from '../logger';
import {
    getUserProfile,
    updateUserProfile,
    changeUserPassword,
    deleteUserAccount,
} from '../api/userApi';

const UserSettings = () => {
    // State for user profile data and image upload
    const [user, setUser] = useState({ username: '', email: '', bio: '' });
    const [profilePicture, setProfilePicture] = useState(null);
    const [previewUrl, setPreviewUrl] = useState('');

    // State for error handling and success notifications
    const [profileError, setProfileError] = useState('');
    const [passwordError, setPasswordError] = useState('');

    const navigate = useNavigate();
    const { deleteAccount, login } = useAuth();
    const profileSuccess = () => toast.success('Profile updated successfully!');
    const passwordSuccess = () =>
        toast.success('Password updated successfully!');
    const notifyDeleteSucsess = () =>
        toast.success('Account deleted successfully!');
    // State for password changes
    const [passwords, setPasswords] = useState({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
    });

    // Fetch user data on component mount
    useEffect(() => {
        const fetchUser = async () => {
            try {
                const response = await getUserProfile();
                setUser({
                    username: response.data.username,
                    email: response.data.email,
                    bio: response.data.bio,
                });
                setPreviewUrl(
                    'http://localhost:5229' + response.data.profilePictureUrl,
                );
            } catch (error) {
                setProfileError('Error fetching user data');
            }
        };
        fetchUser();
    }, []);

    // Handle input changes for profile and passwords
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setUser({ ...user, [name]: value });
    };

    // Handle password changes
    const handlePasswordChange = (e) => {
        const { name, value } = e.target;
        setPasswords({ ...passwords, [name]: value });
    };

    // Handle file input for profile picture
    const handleFileChange = (e) => {
        const file = e.target.files[0];
        setProfilePicture(file);
        setPreviewUrl(URL.createObjectURL(file)); // Set preview image URL
        log.info('Profile Picture changed ready for upload');
    };

    // Submit updated profile
    const handleProfileSubmit = async (e) => {
        e.preventDefault(); // Prevent form submission
        setProfileError('');
        try {
            // Create form data object
            const formData = new FormData();
            formData.append('username', user.username);
            formData.append('email', user.email);
            formData.append('bio', user.bio);
            if (profilePicture) {
                formData.append('profilePicture', profilePicture);
            }
            await updateUserProfile(formData); // Update user profile
            profileSuccess(); // Display success message

            const token = localStorage.getItem('jwtToken');
            login(token, user.username); // Update username in AuthContext
            log.info('Username updated successfully:', user.username);
        } catch (error) {
            setProfileError(error.response.data || 'Error updating profile');
        }
    };

    // Submit password change
    const handlePasswordSubmit = async (e) => {
        e.preventDefault(); // Prevent form submission
        setPasswordError('');

        // Validate new password and confirmation match
        if (passwords.newPassword !== passwords.confirmPassword) {
            log.error('New password and confirmation do not match');
            setPasswordError('New password and confirmation do not match');
            return;
        }

        try {
            // Send request to change password
            await changeUserPassword({
                currentPassword: passwords.currentPassword,
                newPassword: passwords.newPassword,
            });
            passwordSuccess(); // Display success message
            setPasswords({
                currentPassword: '',
                newPassword: '',
                confirmPassword: '',
            });
        } catch (error) {
            log.error('Error updating password: ', error.response.data);
            setPasswordError(error.response.data || 'Error updating password');
        }
    };

    // Handle account deletion
    const handleDeleteAccount = async () => {
        // Confirm account deletion
        if (
            window.confirm(
                'Are you sure you want to delete your account? This action cannot be undone.',
            )
        ) {
            await deleteUserAccount();
            deleteAccount();
            notifyDeleteSucsess(); // Display success message
            navigate(`/`); // Redirect to home
        }
    };

    return (
        <Container>
            <h2 className="my-4">User Settings</h2>

            {/* Profile Settings Form */}
            {profileError && <Alert variant="danger">{profileError}</Alert>}
            <Form onSubmit={handleProfileSubmit}>
                <Form.Group as={Row} className="mb-3" controlId="formUsername">
                    <Form.Label column sm="2">
                        Username
                    </Form.Label>
                    <Col sm="10">
                        {/* input for username */}
                        <Form.Control
                            type="text"
                            name="username"
                            value={user.username}
                            onChange={handleInputChange}
                            maxLength={50}
                        />
                    </Col>
                </Form.Group>

                {/* Email Field */}
                <Form.Group as={Row} className="mb-3" controlId="formEmail">
                    <Form.Label column sm="2">
                        Email
                    </Form.Label>
                    <Col sm="10">
                        {/* input for email */}
                        <Form.Control
                            type="email"
                            name="email"
                            value={user.email}
                            onChange={handleInputChange}
                            maxLength={100}
                        />
                    </Col>
                </Form.Group>

                {/* Profile Picture Field */}
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formProfilePicture">
                    <Form.Label column sm="2">
                        Profile Picture
                    </Form.Label>
                    <Col sm="10">
                        {/* input for profile picture */}
                        <Form.Control
                            type="file"
                            accept="image/*"
                            onChange={handleFileChange}
                        />
                        {/* Display image preview */}
                        {previewUrl && (
                            <img
                                src={previewUrl}
                                alt="Profile Preview"
                                className="mt-3"
                                width="100"
                                height="100"
                            />
                        )}
                    </Col>
                </Form.Group>

                {/* Bio Field */}
                <Form.Group as={Row} className="mb-3" controlId="formBio">
                    <Form.Label column sm="2">
                        Bio
                    </Form.Label>
                    <Col sm="10">
                        {/* input for bio */}
                        <Form.Control
                            as="textarea"
                            name="bio"
                            value={user.bio}
                            onChange={handleInputChange}
                            maxLength={500}
                            rows={3}
                        />
                    </Col>
                </Form.Group>

                {/* Save Profile Button */}
                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="primary">
                            Save Profile Changes
                        </Button>
                    </Col>
                </Form.Group>
            </Form>

            {/* Change Password Form */}
            <h4 className="mt-4">Change Password</h4>
            {passwordError && <Alert variant="danger">{passwordError}</Alert>}
            <Form onSubmit={handlePasswordSubmit}>
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formCurrentPassword">
                    <Form.Label column sm="2">
                        Current Password
                    </Form.Label>
                    <Col sm="10">
                        {/* input for current password */}
                        <Form.Control
                            type="password"
                            name="currentPassword"
                            value={passwords.currentPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formNewPassword">
                    <Form.Label column sm="2">
                        New Password
                    </Form.Label>
                    <Col sm="10">
                        {/* input for new password */}
                        <Form.Control
                            type="password"
                            name="newPassword"
                            value={passwords.newPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formConfirmPassword">
                    <Form.Label column sm="2">
                        Confirm New Password
                    </Form.Label>
                    <Col sm="10">
                        {/* input for password confirmation */}
                        <Form.Control
                            type="password"
                            name="confirmPassword"
                            value={passwords.confirmPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>

                {/* Update Password Button */}
                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="secondary">
                            Update Password
                        </Button>
                    </Col>
                </Form.Group>
            </Form>

            {/* Delete Account Button */}
            <div className="text-end">
                <Button variant="danger" onClick={handleDeleteAccount}>
                    Delete Account
                </Button>
            </div>
        </Container>
    );
};

export default UserSettings;
