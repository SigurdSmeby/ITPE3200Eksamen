import React, { useState } from 'react';
import { Container, Form, Button, Row, Col, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { createPost } from '../api/postApi'; // Add a createPost method to handle file uploads

const UploadPost = () => {
    const navigate = useNavigate();
    const [post, setPost] = useState({
        title: '',
        textContent: '',
        fontSize: 16,
        textColor: '#000000',
        backgroundColor: '#FFFFFF',
    });
    const [imageFile, setImageFile] = useState(null); // State to hold the uploaded file
    const [isImagePost, setIsImagePost] = useState(true); // Toggle between image and text post
    const [success, setSuccess] = useState('');
    const [error, setError] = useState('');

    const handleChange = (e) => {
        const { name, value } = e.target;
        setPost({ ...post, [name]: value });
    };

    const handleFileChange = (e) => {
        setImageFile(e.target.files[0]); // Capture the uploaded file
		console.log(e.target.files[0]);
    };

    const handleTogglePostType = () => {
        setIsImagePost((prev) => !prev);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
    
        const formData = new FormData();
        formData.append("textContent", post.textContent || ""); // Optional
        formData.append("fontSize", post.fontSize || 16);
        formData.append("textColor", post.textColor || "#000000");
        formData.append("backgroundColor", post.backgroundColor || "#FFFFFF");
    
        if (imageFile) {
            formData.append("imageFile", imageFile); // Append the uploaded image
        }
    
        try {
            await createPost(formData);
            alert("Post created successfully!");
        } catch (error) {
            console.error(error);
            alert("Failed to create post.");
        }
    };
    

    return (
        <Container>
            <h2 className="my-4">Upload New Post</h2>

            {error && <Alert variant="danger">{error}</Alert>}
            {success && <Alert variant="success">{success}</Alert>}

            {/* Toggle Button */}
            <div className="mb-4">
                <Button
                    variant={isImagePost ? 'primary' : 'secondary'}
                    onClick={handleTogglePostType}
                >
                    Switch to {isImagePost ? 'Text Post' : 'Image Post'}
                </Button>
            </div>

            <Form onSubmit={handleSubmit}>

                {/* Conditional Rendering: Image Post */}
                {isImagePost && (
                    <Form.Group as={Row} className="mb-3" controlId="formPostImageFile">
                        <Form.Label column sm="2">
                            Upload Image
                        </Form.Label>
                        <Col sm="10">
                            <Form.Control
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                            />
                            {imageFile && (
                                <p className="mt-2">Selected file: {imageFile.name}</p>
                            )}
                        </Col>
                    </Form.Group>
                )}

                {/* Conditional Rendering: Text Post */}
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
                            Upload Post
                        </Button>
                    </Col>
                </Form.Group>
            </Form>
        </Container>
    );
};

export default UploadPost;
