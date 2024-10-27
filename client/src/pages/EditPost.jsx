import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useParams, useNavigate } from 'react-router-dom';
import { getPost, updatePost } from '../api/postApi'; // Use your existing postApi methods

const EditPost = () => {
    const { postId } = useParams(); // Get postId from the URL parameters
    const navigate = useNavigate(); // To redirect after editing
    const [post, setPost] = useState({
        title: '',
        imageUrl: '',
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
    });
    const [isImagePost, setIsImagePost] = useState(true); // Set the post type based on content
    const [success, setSuccess] = useState('');
    const [error, setError] = useState('');

    // Fetch the post details when component mounts
    useEffect(() => {
        const fetchPost = async () => {
            try {
                const response = await getPost(postId); // Fetch post by ID from postApi
                setPost({
                    title: response.title,
                    imageUrl: response.imageUrl || '',
                    textContent: response.textContent || '',
                    fontSize: response.fontSize || 16,
                    textColor: response.textColor || '#000000',
                    backgroundColor: response.backgroundColor || '#FFFFFF',
                });
                setIsImagePost(!!response.imageUrl); // Lock the post type based on whether imageUrl is present
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

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        // Validation: Abort if the required field is empty based on post type
        if (isImagePost && !post.imageUrl.trim()) {
            setError('Image URL cannot be blank for an image post');
            return;
        }
        if (!isImagePost && !post.textContent.trim()) {
            setError('Text content cannot be blank for a text post');
            return;
        }

        try {
            await updatePost(postId, post); // Update the post using postApi
            setSuccess('Post updated successfully');
            setTimeout(() => {
                navigate('/'); // Redirect to home
            }, 2000); // Delay before redirecting to home
        } catch (error) {
            setError('Error updating post');
        }
    };

    return (
        <Container>
            <h2 className="my-4">Edit Post</h2>

            {error && <Alert variant="danger">{error}</Alert>}
            {success && <Alert variant="success">{success}</Alert>}

            <Form onSubmit={handleSubmit}>
                {/* Title */}
                <Form.Group as={Row} className="mb-3" controlId="formPostTitle">
                    <Form.Label column sm="2">
                        Post Title
                    </Form.Label>
                    <Col sm="10">
                        <Form.Control
                            type="text"
                            name="title"
                            value={post.title}
                            onChange={handleChange}
                            required
                        />
                    </Col>
                </Form.Group>

                {/* Image URL - Locked for image posts */}
                {isImagePost && (
                    <Form.Group as={Row} className="mb-3" controlId="formPostImageUrl">
                        <Form.Label column sm="2">
                            Image URL
                        </Form.Label>
                        <Col sm="10">
                            <Form.Control
                                type="text"
                                name="imageUrl"
                                value={post.imageUrl}
                                onChange={handleChange}
                                placeholder="Enter the URL of your post image"
                            />
                            {post.imageUrl && (
                                <img
                                    src={post.imageUrl}
                                    alt="Post"
                                    className="mt-2"
                                    style={{
                                        maxHeight: '400px',
                                        maxWidth: '400px',
                                    }}
                                />
                            )}
                        </Col>
                    </Form.Group>
                )}

                {/* Text Content - Locked for text posts */}
                {!isImagePost && (
                    <>
                        <Form.Group as={Row} className="mb-3" controlId="formPostTextContent">
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
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} className="mb-3" controlId="formPostFontSize">
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

                        <Form.Group as={Row} className="mb-3" controlId="formPostTextColor">
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

                        <Form.Group as={Row} className="mb-3" controlId="formPostBackgroundColor">
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
                            Save Changes
                        </Button>
                    </Col>
                </Form.Group>
            </Form>
        </Container>
    );
};

export default EditPost;
