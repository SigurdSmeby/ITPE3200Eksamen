import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import {
    getUserProfile,
    updateUserProfile,
    changeUserPassword,
} from '../api/userApi';

const UserSettings = () => {
    const [user, setUser] = useState({
        username: '',
        email: '',
        profilePictureUrl: 'default_profile_pic.jpg',
        bio: '',
    });

    const [passwords, setPasswords] = useState({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
    });

    // State for profile form messages
    const [profileSuccess, setProfileSuccess] = useState('');
    const [profileError, setProfileError] = useState('');

    // State for password form messages
    const [passwordSuccess, setPasswordSuccess] = useState('');
    const [passwordError, setPasswordError] = useState('');

    useEffect(() => {
        const fetchUser = async () => {
            try {
                const response = await getUserProfile();
                setUser({
                    username: response.data.username,
                    email: response.data.email,
                    profilePictureUrl:
                        response.data.profilePictureUrl ||
                        'default_profile_pic.jpg',
                    bio: response.data.bio,
                });
            } catch (error) {
                setProfileError('Error fetching user data');
                console.error('Error fetching user data:', error);
            }
        };
        fetchUser();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setUser({ ...user, [name]: value });
    };

    const handleProfileSubmit = async (e) => {
        e.preventDefault();
        setProfileError('');
        setProfileSuccess('');

        try {
            const data = {
                username: user.username,
                email: user.email,
                profilePictureUrl: user.profilePictureUrl,
                bio: user.bio,
            };

            await updateUserProfile(data);
            setProfileSuccess('Profile updated successfully');
        } catch (error) {
            setProfileError('An error occurred while updating the profile');
            console.error('Error updating profile:', error);
        }
    };

    const handlePasswordChange = (e) => {
        const { name, value } = e.target;
        setPasswords({ ...passwords, [name]: value });
    };

    const handlePasswordSubmit = async (e) => {
        e.preventDefault();
        setPasswordError('');
        setPasswordSuccess('');

        if (passwords.newPassword !== passwords.confirmPassword) {
            setPasswordError('New password and confirmation do not match');
            return;
        }

        try {
            const data = {
                currentPassword: passwords.currentPassword,
                newPassword: passwords.newPassword,
            };

            await changeUserPassword(data);
            setPasswordSuccess('Password updated successfully');
            setPasswords({
                currentPassword: '',
                newPassword: '',
                confirmPassword: '',
            });
        } catch (error) {
            setPasswordError(error);
            console.error('Error updating password:', error);
        }
    };

    return (
        <Container>
            <h2 className="my-4">User Settings</h2>

            {/* Profile Settings Form */}
            {profileError && <Alert variant="danger">{profileError}</Alert>}
            {profileSuccess && (
                <Alert variant="success">{profileSuccess}</Alert>
            )}
            <Form onSubmit={handleProfileSubmit}>
                <Form.Group as={Row} className="mb-3" controlId="formUsername">
                    <Form.Label column sm="2">
                        Username
                    </Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="text"
                            name="username"
                            value={user.username}
                            onChange={handleChange}
                            maxLength={50}
                        />
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formEmail">
                    <Form.Label column sm="2">
                        Email
                    </Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="email"
                            name="email"
                            value={user.email}
                            onChange={handleChange}
                            maxLength={100}
                        />
                    </Col>
                </Form.Group>

                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formProfilePictureUrl">
                    <Form.Label column sm="2">
                        Profile Picture URL
                    </Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="text"
                            name="profilePictureUrl"
                            value={user.profilePictureUrl}
                            onChange={handleChange}
                            placeholder="Enter the URL of your profile picture"
                        />
                        {user.profilePictureUrl && (
                            <img
                                src={user.profilePictureUrl}
                                alt="Profile"
                                className="mt-2"
                                width="100"
                                height="100"
                            />
                        )}
                    </Col>
                </Form.Group>

                <Form.Group as={Row} className="mb-3" controlId="formBio">
                    <Form.Label column sm="2">
                        Bio
                    </Form.Label>
                    <Col sm="10">
                        <Form.Control
                            as="textarea"
                            name="bio"
                            value={user.bio}
                            onChange={handleChange}
                            maxLength={500}
                            rows={3}
                        />
                    </Col>
                </Form.Group>

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
            {passwordSuccess && (
                <Alert variant="success">{passwordSuccess}</Alert>
            )}
            <Form onSubmit={handlePasswordSubmit}>
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formCurrentPassword">
                    <Form.Label column sm="2">
                        Current Password
                    </Form.Label>
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

                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formNewPassword">
                    <Form.Label column sm="2">
                        New Password
                    </Form.Label>
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

                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formConfirmPassword">
                    <Form.Label column sm="2">
                        Confirm New Password
                    </Form.Label>
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
                        <Button type="submit" variant="secondary">
                            Update Password
                        </Button>
                    </Col>
                </Form.Group>
            </Form>
        </Container>
    );
};

export default UserSettings;
