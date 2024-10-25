import React from 'react';
import { Card, Button } from 'react-bootstrap';
import { FaHeart } from 'react-icons/fa';
import { Dropdown } from 'react-bootstrap';
import TestApi from '../api/testapi';

// Format date function
const formatDate = (dateString: string): string => {
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
    title,
    dateUploaded,
    author,
    likesCount,
    onDeleted,
}) => {
    const [liked, setLiked] = React.useState(false);

    const handleLikeClick = () => {
        setLiked(!liked);
    };

    const handleDeletePost = (id) => {
        console.log('click');
        TestApi.deletePost(id).then((response) => {
            console.log(response.data);
            onDeleted();
        });
    };

    const authorName = author?.username || 'Unknown Author';
    const profilePicture = author?.profilePictureUrl || 'default-profile.png'; // Fallback image if none is provided
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

                <Dropdown className="ms-auto">
                    <Dropdown.Toggle variant="secondary" id="dropdown-basic">
                        Menu
                    </Dropdown.Toggle>

                    <Dropdown.Menu>
                        <Dropdown.Item href={`/edit-post/${postId}`}>
                            Edit
                        </Dropdown.Item>
                        <Dropdown.Item onClick={() => handleDeletePost(postId)}>
                            Delete
                        </Dropdown.Item>
                    </Dropdown.Menu>
                </Dropdown>
            </Card.Header>

            <Card.Body
                style={{
                    maxHeight: '30rem',
                    alignContent: 'center',
                    overflow: 'hidden',
                    marginBottom: '5px',
                }}>
                {/* Format the date here */}
                <p style={{ margin: '0' }}>{formatDate(dateUploaded)} </p>
                <h4>{title}</h4>
                <Card.Img
                    variant="top"
                    src={imageUrl}
                    alt="Body Image"
                    loading="lazy"
                    style={{
                        width: '100%',
                        height: '100%',
                        objectFit: 'contain',
                    }}
                />
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
