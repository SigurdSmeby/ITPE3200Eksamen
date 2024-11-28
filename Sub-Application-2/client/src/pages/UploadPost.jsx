import React, { useState } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { createPost } from '../api/postApi'; // Add a createPost method to handle file uploads
import { toast } from 'react-toastify';

const UploadPost = () => {
    const navigate = useNavigate();
    const [post, setPost] = useState({
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
    });
    const [imageFile, setImageFile] = useState(null); // State to hold the uploaded file
    const [isImagePost, setIsImagePost] = useState(true); // Toggle between image and text post
    const [error, setError] = useState('');
    const UploadSucsess = () => toast.success("Post created successfully!");

    const handleChange = (e) => {
        const { name, value } = e.target;
        setPost({ ...post, [name]: value });
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file.size > 10 * 1024 * 1024) {
            setError('File size exceeds 10MB, please upload a smaller file.');
            e.target.value = null;
            return;
        }
        setImageFile(file); // Capture the uploaded file
        setError(''); // Reset error message

        if (file) {
            const reader = new FileReader();
            reader.onload = () => {
                setPost((prev) => ({
                    ...prev,
                    previewUrl: reader.result, // Set the preview URL
                }));
            };
            reader.readAsDataURL(file);
        } else {
            setPost((prev) => ({ ...prev, previewUrl: null }));
        }
    };

    const handleTogglePostType = () => {
        setIsImagePost((prev) => !prev);
        // Reset the state for the other post type
        if (isImagePost) {
            setError('');
            setPost({
                textContent: '',
                fontSize: 16,
                textColor: '#000000',
                backgroundColor: '#FFFFFF',
            });
        } else {
            setError('');
            setImageFile(null);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        // Validation: Ensure either an image or text content is provided
        if (isImagePost && !imageFile) {
            setError('Please upload an image for an image post.');
            return;
        }
        if (!isImagePost && !post.textContent.trim()) {
            setError('Text content cannot be empty for a text post.');
            return;
        }

        // Create form data
        const formData = new FormData();
        if (isImagePost) {
            formData.append('imageFile', imageFile); // Add the uploaded image
        } else {
            formData.append('textContent', post.textContent);
            formData.append('fontSize', post.fontSize);
            formData.append('textColor', post.textColor);
            formData.append('backgroundColor', post.backgroundColor);
        }

        try {
            await createPost(formData); // API call
            UploadSucsess();
            navigate('/'); // Redirect to home
        } catch (error) {
            setError('Failed to create post. Please try again.');
        }
    };

    return (
        <Container>
            <h2 className="my-4">Upload New Post</h2>

            {error && <Alert variant="danger">{error}</Alert>}

            {/* Toggle Button */}
            <div className="mb-4">
                <Button
                    variant={isImagePost ? 'primary' : 'secondary'}
                    onClick={handleTogglePostType}>
                    Switch to {isImagePost ? 'Text Post' : 'Image Post'}
                </Button>
            </div>

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
