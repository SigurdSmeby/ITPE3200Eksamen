import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../components/shared/AuthContext'; 
import {
    getUserProfile,
    updateUserProfile,
    changeUserPassword,
    deleteUserAccount,
} from '../api/userApi';

const UserSettings = () => {
    const navigate = useNavigate();
    const { deleteAccount } = useAuth(); // Access account deletion from AuthContext

    // State for user profile data and image upload
    const [user, setUser] = useState({ username: '', email: '', bio: '' });
    const [profilePicture, setProfilePicture] = useState(null);
    const [previewUrl, setPreviewUrl] = useState('');

    // State for password changes
    const [passwords, setPasswords] = useState({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
    });

    // State for error handling and success notifications
    const [profileError, setProfileError] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const profileSuccess = () => toast.success("Profile updated successfully!");
    const passwordSuccess = () => toast.success("Password updated successfully!");
    const notifyDeleteSucsess = () => toast.success("Account deleted successfully!");

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
                setPreviewUrl('http://localhost:5229' + response.data.profilePictureUrl);
            } catch (error) {
                setProfileError('Error fetching user data');
                console.error('Error fetching user data:', error);
            }
        };
        fetchUser();
    }, []);

    // Handle input changes for profile and passwords
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setUser({ ...user, [name]: value });
    };

    const handlePasswordChange = (e) => {
        const { name, value } = e.target;
        setPasswords({ ...passwords, [name]: value });
    };

    // Handle file input for profile picture
    const handleFileChange = (e) => {
        const file = e.target.files[0];
        setProfilePicture(file);
        setPreviewUrl(URL.createObjectURL(file)); // Set preview image URL
    };

    // Submit updated profile
    const handleProfileSubmit = async (e) => {
        e.preventDefault();
        setProfileError('');
        try {
            const formData = new FormData();
            formData.append('username', user.username);
            formData.append('email', user.email);
            formData.append('bio', user.bio);
            if (profilePicture) formData.append('profilePicture', profilePicture);

            await updateUserProfile(formData);
            profileSuccess();
            localStorage.setItem('username', user.username); // Update local username
        } catch (error) {
            setProfileError(error.response.data || 'Error updating profile');
            console.error('Error updating profile:', error);
        }
    };

    // Submit password change
    const handlePasswordSubmit = async (e) => {
        e.preventDefault();
        setPasswordError('');
        if (passwords.newPassword !== passwords.confirmPassword) {
            setPasswordError('New password and confirmation do not match');
            return;
        }

        try {
            await changeUserPassword({
                currentPassword: passwords.currentPassword,
                newPassword: passwords.newPassword,
            });
            passwordSuccess();
            setPasswords({ currentPassword: '', newPassword: '', confirmPassword: '' });
        } catch (error) {
            setPasswordError(error.response.data || 'Error updating password');
            console.error('Error updating password:', error);
        }
    };

    // Handle account deletion
    const handleDeleteAccount = async () => {
        if (window.confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
            try {
                await deleteUserAccount();
                deleteAccount();
                notifyDeleteSucsess();
                navigate(`/`);
            } catch (error) {
                console.error('Error deleting account:', error);
            }
        }
    };

    return (
        <Container>
            <h2 className="my-4">User Settings</h2>

            {/* Profile Settings Form */}
            {profileError && <Alert variant="danger">{profileError}</Alert>}
            <Form onSubmit={handleProfileSubmit}>
                <Form.Group as={Row} className="mb-3" controlId="formUsername">
                    <Form.Label column sm="2">Username</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="text"
                            name="username"
                            value={user.username}
                            onChange={handleInputChange}
                            maxLength={50}
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formEmail">
                    <Form.Label column sm="2">Email</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="email"
                            name="email"
                            value={user.email}
                            onChange={handleInputChange}
                            maxLength={100}
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formProfilePicture">
                    <Form.Label column sm="2">Profile Picture</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="file"
                            accept="image/*"
                            onChange={handleFileChange}
                        />
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

                <Form.Group as={Row} className="mb-3" controlId="formBio">
                    <Form.Label column sm="2">Bio</Form.Label>
                    <Col sm="10">
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

                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="primary">Save Profile Changes</Button>
                    </Col>
                </Form.Group>
            </Form>

            {/* Change Password Form */}
            <h4 className="mt-4">Change Password</h4>
            {passwordError && <Alert variant="danger">{passwordError}</Alert>}
            <Form onSubmit={handlePasswordSubmit}>
                <Form.Group as={Row} className="mb-3" controlId="formCurrentPassword">
                    <Form.Label column sm="2">Current Password</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="password"
                            name="currentPassword"
                            value={passwords.currentPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formNewPassword">
                    <Form.Label column sm="2">New Password</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="password"
                            name="newPassword"
                            value={passwords.newPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formConfirmPassword">
                    <Form.Label column sm="2">Confirm New Password</Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="password"
                            name="confirmPassword"
                            value={passwords.confirmPassword}
                            onChange={handlePasswordChange}
                            required
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="secondary">Update Password</Button>
                    </Col>
                </Form.Group>
            </Form>

            {/* Delete Account Button */}
            <div className="text-end">
                <Button variant="danger" onClick={handleDeleteAccount}>Delete Account</Button>
            </div>
        </Container>
    );
};

export default UserSettings;
