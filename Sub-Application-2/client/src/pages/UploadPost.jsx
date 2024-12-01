import React, { useState } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { createPost } from '../api/postApi';
import { toast } from 'react-toastify';
import log from '../logger';

const UploadPost = () => {
    // State for managing post data
    const [post, setPost] = useState({
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
    });
    const [imageFile, setImageFile] = useState(null);
    const [isImagePost, setIsImagePost] = useState(true);
    const [error, setError] = useState('');
    const uploadSuccess = () => toast.success('Post created successfully!');
    const navigate = useNavigate();

    // Handle text input changes
    const handleChange = (e) => {
        const { name, value } = e.target;
        setPost({ ...post, [name]: value });
    };

    // Handle image file selection
    const handleFileChange = (e) => {
        const file = e.target.files[0];

        // Validate file size (10MB max)
        if (file?.size > 10 * 1024 * 1024) {
            setError('File size exceeds 10MB, please upload a smaller file.');
            log.error('File size exceeds 10MB');
            e.target.value = null;
            return;
        }

        setImageFile(file);
        log.info(
            `Image file selected: ${file.name} and ready for upload. file size: ${file.size}`,
        );
        setError('');

        // Generate a preview of the image
        if (file) {
            const reader = new FileReader();
            reader.onload = () => {
                setPost((prev) => ({
                    ...prev,
                    previewUrl: reader.result,
                }));
            };
            reader.readAsDataURL(file);
        } else {
            setPost((prev) => ({ ...prev, previewUrl: null }));
        }
    };

    // Toggle between image and text post
    const handleTogglePostType = () => {
        setIsImagePost((prev) => !prev);
        log.info(`Post type toggled to ${isImagePost ? 'text' : 'image'}`);
        setError('');

        // Reset respective state based on post type
        if (isImagePost) {
            setPost({
                textContent: '',
                fontSize: 16,
                textColor: '#000000',
                backgroundColor: '#FFFFFF',
            });
        } else {
            setImageFile(null);
        }
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        // Validate inputs based on post type
        if (isImagePost && !imageFile) {
            setError('Please upload an image for an image post.');
            log.error('Please upload an image for an image post.');
            return;
        }
        if (!isImagePost && !post.textContent.trim()) {
            setError('Text content cannot be empty for a text post.');
            log.error('Text content cannot be empty for a text post.');
            return;
        }

        // Prepare form data for API call
        const formData = new FormData();
        if (isImagePost) {
            formData.append('imageFile', imageFile);
        } else {
            formData.append('textContent', post.textContent);
            formData.append('fontSize', post.fontSize);
            formData.append('textColor', post.textColor);
            formData.append('backgroundColor', post.backgroundColor);
        }

        try {
            await createPost(formData); // API call to create post
            uploadSuccess();
            navigate('/'); // Redirect to home page
        } catch {
            setError('Failed to create post. Please try again.');
        }
    };

    return (
        <Container>
            <h2 className="my-4">Upload New Post</h2>

            {/* Display error messages */}
            {error && <Alert variant="danger">{error}</Alert>}

            {/* Toggle Post Type */}
            <div className="mb-4">
                <Button
                    variant={isImagePost ? 'primary' : 'secondary'}
                    onClick={handleTogglePostType}>
                    Switch to {isImagePost ? 'Text Post' : 'Image Post'}
                </Button>
            </div>

            {/* Post Form */}
            <Form onSubmit={handleSubmit}>
                {/* Image Post */}
                {isImagePost && (
                    <Form.Group
                        as={Row}
                        className="mb-3"
                        controlId="formPostImageFile">
                        <Form.Label column sm="2">
                            Upload Image
                        </Form.Label>
                        <Col sm="10">
                            <Form.Control
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                required={isImagePost}
                            />
                            {/* Display image preview */}
                            {imageFile && (
                                <>
                                    <p className="mt-2">
                                        Selected file: {imageFile.name}
                                    </p>
                                    <img
                                        src={post.previewUrl}
                                        alt="Preview"
                                        className="mt-2"
                                        style={{
                                            maxHeight: '200px',
                                            maxWidth: '200px',
                                        }}
                                    />
                                </>
                            )}
                        </Col>
                    </Form.Group>
                )}

                {/* Text Post */}
                {!isImagePost && (
                    <>
                        <Form.Group
                            as={Row}
                            className="mb-3"
                            controlId="formPostTextContent">
                            <Form.Label column sm="2">
                                Text Content
                            </Form.Label>
                            <Col sm="10">
                                {/* Text content input */}
                                <Form.Control
                                    as="textarea"
                                    name="textContent"
                                    value={post.textContent}
                                    onChange={handleChange}
                                    rows={5}
                                    placeholder="Enter your text content"
                                    required={!isImagePost}
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group
                            as={Row}
                            className="mb-3"
                            controlId="formPostFontSize">
                            <Form.Label column sm="2">
                                Font Size
                            </Form.Label>
                            <Col sm="10">
                                {/* Font size input */}
                                <Form.Control
                                    type="number"
                                    name="fontSize"
                                    value={post.fontSize}
                                    onChange={handleChange}
                                    min="10"
                                    max="72"
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group
                            as={Row}
                            className="mb-3"
                            controlId="formPostTextColor">
                            <Form.Label column sm="2">
                                Text Color
                            </Form.Label>
                            <Col sm="10">
                                {/* Text color input */}
                                <Form.Control
                                    type="color"
                                    name="textColor"
                                    value={post.textColor}
                                    onChange={handleChange}
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group
                            as={Row}
                            className="mb-3"
                            controlId="formPostBackgroundColor">
                            <Form.Label column sm="2">
                                Background Color
                            </Form.Label>
                            <Col sm="10">
                                {/* Background color input */}
                                <Form.Control
                                    type="color"
                                    name="backgroundColor"
                                    value={post.backgroundColor}
                                    onChange={handleChange}
                                />
                            </Col>
                        </Form.Group>
                    </>
                )}

                {/* Submit Button */}
                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="primary">
                            Upload Post
                        </Button>
                    </Col>
                </Form.Group>
            </Form>
        </Container>
    );
};

export default UploadPost;
