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
    });
    const [success, setSuccess] = useState('');
    const [error, setError] = useState('');

    // Fetch the post details when component mounts
    useEffect(() => {
        const fetchPost = async () => {
            try {
                const response = await getPost(postId); // Fetch post by ID from postApi
                console.log(response);
                setPost({
                    title: response.title,
                    imageUrl: response.imageUrl,
                });
            } catch (error) {
                setError('Error fetching post details');
            }
        };
        fetchPost();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setPost({ ...post, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        try {
            await updatePost(postId, post); // Update the post using postApi
            setSuccess('Post updated successfully');
            setTimeout(() => {
                navigate('/'); // Redirect to home
            }, 2000); // Delay before redirecting to home
        } catch (error) {
            setError(error);
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

                {/* Image URL */}
                <Form.Group
                    as={Row}
                    className="mb-3"
                    controlId="formPostImageUrl">
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
                            required
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
