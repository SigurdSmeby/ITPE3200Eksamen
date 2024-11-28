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
    const [user, setUser] = useState({
        username: '',
        email: '',
        bio: '',
    });

    const [profilePicture, setProfilePicture] = useState(null); // State to handle file upload
    const [previewUrl, setPreviewUrl] = useState(''); // State for image preview

    const [passwords, setPasswords] = useState({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
    });

    const profileSuccess = () => toast.success("Profile updated successfully!");
    const [profileError, setProfileError] = useState('');

    const passwordSuccess = () => toast.success("Password updated successfully!");
    const [passwordError, setPasswordError] = useState('');

    const notifyDeleteSucsess = () => toast.success("Account deleted successfully!");
    const { deleteAccount } = useAuth(); 
    const navigate = useNavigate();

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
                console.error('Error fetching user data:', error);
            }
        };
        fetchUser();
    }, []);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setUser({ ...user, [name]: value });
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        setProfilePicture(file);
        setPreviewUrl(URL.createObjectURL(file)); // Set preview image
    };

    const handleProfileSubmit = async (e) => {
        e.preventDefault();
        setProfileError('');

        try {
            const formData = new FormData();
            formData.append('username', user.username);
            formData.append('email', user.email);
            formData.append('bio', user.bio);
            if (profilePicture) {
                formData.append('profilePicture', profilePicture); // Append the file
            }

            await updateUserProfile(formData);
            profileSuccess();
            localStorage.setItem('username', user.username);
        } catch (error) {
            setProfileError(error.response.data);
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
            passwordSuccess();
            setPasswords({
                currentPassword: '',
                newPassword: '',
                confirmPassword: '',
            });
        } catch (error) {
            setPasswordError(error.response.data);
            console.error('Error updating password:', error);
        }
    };
    const handleDeleteAccount = async () => {
        const confirmDelete = window.confirm(
            'Are you sure you want to delete your account? This action cannot be undone.',
        );

        if (confirmDelete) {
            try {
                await deleteUserAccount().then(() =>
                    deleteAccount(),
                    notifyDeleteSucsess(),
                    navigate(`/`)
                );
                
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
                    <Form.Label column sm="2">
                        Username
                    </Form.Label>
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
                    <Form.Label column sm="2">
                        Email
                    </Form.Label>
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

                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formProfilePicture">
                    <Form.Label column sm="2">
                        Profile Picture
                    </Form.Label>
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
                    <Form.Label column sm="2">
                        Bio
                    </Form.Label>
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
