import React from 'react';
import { Card, Button, Dropdown } from 'react-bootstrap';
import { FaHeart } from 'react-icons/fa';
import { deletePost } from '../api/postApi';

// Format date function
const formatDate = (dateString) => {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = String(date.getFullYear()).slice(-2);
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}.${month}.${year} - ${hours}.${minutes}`;
};

// PostCards component
const PostCards = ({
    postId,
    imageUrl,
    textContent,
    title,
    dateUploaded,
    author,
    likesCount,
    fontSize,
    textColor,
    backgroundColor,
    onDeleted,
}) => {
    const [liked, setLiked] = React.useState(false);

    // Get logged-in username from localStorage
    const loggedInUsername = localStorage.getItem('username');
    const isOwner = loggedInUsername === author?.username;

    const handleLikeClick = () => {
        setLiked(!liked);
    };

    const handleDeletePost = (id) => {
        deletePost(id)
            .then((response) => {
                console.log(response.data);
                onDeleted();
            })
            .catch((error) => {
                console.error(error);
            });
    };

    const authorName = author?.username || 'Unknown Author';
    const profilePicture = author?.profilePictureUrl || 'default-profile.png';
    const profileUrl = `/profile/${authorName}`;

    return (
        <Card style={{ width: '30rem', margin: '1rem auto' }}>
            <Card.Header className="d-flex align-items-center">
                <a
                    href={profileUrl}
                    style={{
                        display: 'inline-flex',
                        alignItems: 'center',
                        textDecoration: 'none',
                        color: 'inherit',
                    }}>
                    <Card.Img
                        variant="top"
                        src={profilePicture}
                        alt="User profile"
                        loading="lazy"
                        style={{
                            width: '50px',
                            height: '50px',
                            borderRadius: '50%',
                            marginRight: '1rem',
                        }}
                    />
                    <h4>{authorName}</h4>
                </a>

                {isOwner && (
                    <Dropdown className="ms-auto">
                        <Dropdown.Toggle
                            variant="secondary"
                            id="dropdown-basic">
                            Menu
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            <Dropdown.Item href={`/edit-post/${postId}`}>
                                Edit
                            </Dropdown.Item>
                            <Dropdown.Item
                                onClick={() => handleDeletePost(postId)}>
                                Delete
                            </Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                )}
            </Card.Header>

            <Card.Body
                style={{
                    maxHeight: '30rem',
                    alignContent: 'center',
                    overflow: 'hidden',
                    marginBottom: '5px',
                }}>
                <p style={{ margin: '0' }}>{formatDate(dateUploaded)}</p>
                <h4>{title}</h4>

                {/* Conditionally render image or styled text content */}
                {imageUrl ? (
                    <Card.Img
                        variant="top"
                        src={imageUrl}
                        alt="Post Image"
                        loading="lazy"
                        style={{
                            width: '100%',
                            height: '100%',
                            objectFit: 'contain',
                        }}
                    />
                ) : (
                    <p
                        style={{
                            fontSize: fontSize ? `${fontSize}px` : '16px', // Default to 16px if not provided
                            color: textColor || '#000000', // Default to black if not provided
                            backgroundColor: backgroundColor || '#FFFFFF', // Default to white if not provided
                        }}>
                        {textContent}
                    </p>
                )}
            </Card.Body>

            <Card.Footer className="text-center">
                <div
                    style={{ display: 'inline-block', cursor: 'pointer' }}
                    onClick={handleLikeClick}>
                    <FaHeart color={liked ? 'red' : 'black'} size={24} />
                    <p>{likesCount}</p>
                </div>
                <Button variant="secondary">Comment</Button>
            </Card.Footer>
        </Card>
    );
};

export default PostCards;
