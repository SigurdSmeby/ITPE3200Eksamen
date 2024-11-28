import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useParams, useNavigate } from 'react-router-dom';
import { getPost, updatePost } from '../api/postApi';

const EditPost = () => {
    const { postId } = useParams(); // Get postId from the URL parameters
    const navigate = useNavigate(); // To redirect after editing
    const [post, setPost] = useState({
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
        previewUrl: null, // Add previewUrl to manage image preview
    });
    const [isImagePost, setIsImagePost] = useState(true); // Set the post type based on content
    const [imageFile, setImageFile] = useState(null); // State to store the uploaded file
    const [success, setSuccess] = useState('');
    const [error, setError] = useState('');

    // Fetch the post details when component mounts
    useEffect(() => {
        const fetchPost = async () => {
            try {
                const response = await getPost(postId); // Fetch post by ID from postApi
                setPost({
                    textContent: response.textContent || '',
                    fontSize: response.fontSize || 16,
                    textColor: response.textColor || '#000000',
                    backgroundColor: response.backgroundColor || '#FFFFFF',
                    previewUrl: `http://localhost:5229/${response.imagePath}`, // Set full path to image
                });
                setIsImagePost(!!response.imagePath); // Determine post type based on imagePath
            } catch (error) {
                setError('Error fetching post details');
            }
        };
        fetchPost();
    }, [postId]);

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
                    previewUrl: reader.result, // Set preview URL for uploaded file
                }));
            };
            reader.readAsDataURL(file);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        // Validation: Ensure the required field is not empty
        if (isImagePost && !imageFile) {
            setError('Please upload an image file for the post.');
            return;
        }
        if (!isImagePost && !post.textContent.trim()) {
            setError('Text Content cannot be blank for a text post.');
            return;
        }

        // Prepare form data
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
            await updatePost(postId, formData); // Update the post using postApi
            setSuccess('Post updated successfully!');
            setTimeout(() => {
                navigate('/'); // Redirect to home
            }, 1000); // Delay before redirecting to home
        } catch (error) {
            setError('Error updating post. Please try again.');
        }
    };

    return (
        <Container>
            <h2 className="my-4">
                {isImagePost ? 'Edit Image Post' : 'Edit Text Post'}
            </h2>

            {error && <Alert variant="danger">{error}</Alert>}
            {success && <Alert variant="success">{success}</Alert>}

            <Form onSubmit={handleSubmit}>
                {/* Conditional Rendering: Image Post */}
                {isImagePost && (
                    <Form.Group
                        as={Row}
                        className="mb-3"
                        controlId="formPostImageFile">
                        <Form.Label column sm="2">
                            Upload New Image
                        </Form.Label>
                        <Col sm="10">
                            <Form.Control
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                                required
                            />
                            {post.previewUrl && (
                                <img
                                    src={post.previewUrl}
                                    alt="Preview"
                                    className="mt-3"
                                    style={{
                                        maxHeight: '200px',
                                        maxWidth: '200px',
                                    }}
                                />
                            )}
                            {imageFile && (
                                <p className="mt-2">
                                    Selected file: {imageFile.name}
                                </p>
                            )}
                        </Col>
                    </Form.Group>
                )}

                {/* Conditional Rendering: Text Post */}
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
                                    required
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
                                    required
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
                                    required
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
                                    required
                                />
                            </Col>
                        </Form.Group>
                    </>
                )}

                {/* Submit Button */}
                <Form.Group as={Row} className="mb-3">
                    <Col sm={{ span: 10, offset: 2 }}>
                        <Button type="submit" variant="primary">
                            Save Changes
                        </Button>
                    </Col>
                </Form.Group>
            </Form>
        </Container>
    );
};

export default EditPost;
