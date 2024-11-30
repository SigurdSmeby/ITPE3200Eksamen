import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useParams, useNavigate } from 'react-router-dom';
import { getPost, updatePost } from '../api/postApi';
import { toast } from 'react-toastify';

const EditPost = () => {
    const { postId } = useParams(); // Extract the post ID from the URL
    const [post, setPost] = useState({
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
        previewUrl: null, // Manage image preview for uploaded files
    });
    const [isImagePost, setIsImagePost] = useState(true);
    const [imageFile, setImageFile] = useState(null); 
    const [error, setError] = useState('');
    const editPostSucsess = () => toast.success("Post updated successfully!"); // Toast message on success
    const navigate = useNavigate(); // Navigation object for redirection

    // Fetch the post details when the component mounts
    useEffect(() => {
        const fetchPost = async () => {
            try {
                const response = await getPost(postId);
                // Set the post details in the state
                setPost({
                    textContent: response.textContent || '',
                    fontSize: response.fontSize || 16,
                    textColor: response.textColor || '#000000',
                    backgroundColor: response.backgroundColor || '#FFFFFF',
                    previewUrl: `http://localhost:5229/${response.imagePath}`, // Set image URL for preview
                });
                setIsImagePost(!!response.imagePath);
            } catch (error) {
                setError('Error fetching post details');
            }
        };
        // Fetch the post details
        fetchPost();
    }, [postId]);

    // Update the state on input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        setPost({ ...post, [name]: value });
    };

    // Handle file selection for image upload
    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file.size > 10 * 1024 * 1024) {
            setError('File size exceeds 10MB, please upload a smaller file.');
            e.target.value = null;
            return;
        }
        setImageFile(file);
        setError('');

        // Create a preview for the uploaded image
        if (file) {
            const reader = new FileReader();
            reader.onload = () => {
                setPost((prev) => ({
                    ...prev,
                    previewUrl: reader.result,
                }));
            };
            reader.readAsDataURL(file);
        }
    };

    // Handle form submission to update the post
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        // Validate fields based on post type
        if (isImagePost && !imageFile) {
            setError('Please upload an image file for the post.');
            return;
        }
        if (!isImagePost && !post.textContent.trim()) {
            setError('Text Content cannot be blank for a text post.');
            return;
        }

        // Prepare form data for the API eather image or text post
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
            // Update the post with the form data
            await updatePost(postId, formData);
            editPostSucsess();
            navigate('/'); // Redirect to home
        } catch (error) {
            setError('Error updating post. Please try again.');
        }
    };

    return (
        <Container>
            <h2 className="my-4">
                {/* Display the appropriate title based on post type */}
                {isImagePost ? 'Edit Image Post' : 'Edit Text Post'}
            </h2>

            {error && <Alert variant="danger">{error}</Alert>} {/* Error message */}

            <Form onSubmit={handleSubmit}>
                {/* Image Post Fields */}
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
                            {/* Display the image preview */}
                            {post.previewUrl && (
                                <img
                                    src={post.previewUrl}
                                    alt="Preview"
                                    className="mt-3 img-thumbnail"
                                    style={{ maxWidth: '200px', height: 'auto' }}
                                />
                            )}
                            {imageFile && (
                                <p className="mt-2 text-muted">
                                    Selected file: {imageFile.name}
                                </p>
                            )}
                        </Col>
                    </Form.Group>
                )}

                {/* Text Post Fields */}
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
                                {/* Font size input */}
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
                                {/* Text color input */}
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
                                {/* Background color input */}
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
