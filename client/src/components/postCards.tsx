import React from 'react';
import { Card, Button, Dropdown } from 'react-bootstrap';
import { FaHeart, FaComment } from 'react-icons/fa';
import { deletePost } from '../api/postApi';
import './postCards.css';

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
const timeAgo = (dateString) => {
    const now = new Date();
    const postDate = new Date(dateString);
    const diffInSeconds = Math.floor((now.getTime() - postDate.getTime()) / 1000);

    const minutes = Math.floor(diffInSeconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const weeks = Math.floor(days / 7);

    if (weeks > 0) return `${weeks}w`;
    if (days > 0) return `${days}d`;
    if (hours > 0) return `${hours}h`;
    if (minutes > 0) return `${minutes}m`;
    return `${diffInSeconds}s`;
};

// PostCards component
const PostCards = ({
    postId,
    imageUrl,
    textContent,
    title, // kan fjernes om vi bestemmer oss for å ikke bruke tittel
    dateUploaded,
    author,
    likesCount,
    fontSize,
    textColor,
    backgroundColor,
    onDeleted,
}) => {
    const [liked, setLiked] = React.useState(false);
    const [showComments, setShowComments] = React.useState(false);
    // Get logged-in username from localStorage
    const loggedInUsername = localStorage.getItem('username');
    const isOwner = loggedInUsername === author?.username;

        // hardkoda kommentarer som vi kan fikse senere
    const comments = [
        { author: { username: 'user1', profileUrl: '/profile/user1' }, text: 'Great post!' },
        { author: { username: 'user2', profileUrl: '/profile/user2' }, text: 'Amazing picture!' },
        { author: { username: 'user3', profileUrl: '/profile/user3' }, text: 'Thanks for sharing!' }
    ];

    const handleLikeClick = () => {
        setLiked(!liked);
    };
    const handleToggleComments = () => {
        setShowComments(!showComments);
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
        <Card className="card-container">
            
            <Card.Header >
                <a href={profileUrl} className="profile-link">
                    <Card.Img
                        variant="top"
                        src={profilePicture}
                        alt="User profile"
                        loading="lazy"
                        className="profile-img"
                    />
                    <h4>{authorName}</h4>
                    
                </a><p className="date" title={new Date(dateUploaded).toLocaleString()}>
                    {timeAgo(dateUploaded)}
                </p>
    
                {isOwner && (
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
                )}
            </Card.Header>
    
            <Card.Body 
            style={{
                fontSize: fontSize ? `${fontSize}px` : undefined,
                color: textColor || undefined,
                backgroundColor: backgroundColor || undefined,
            }}>
                

    
                {imageUrl ? (
                    <div className="image-container" style={{backgroundImage: `url(${imageUrl})`,}}>
                    <img
                        src={imageUrl}
                        alt={title} //bruker tittel her litt for å bruke det, kan evt velge å bruke filnavn
                        loading="lazy"
                        className="post-image"
                    />
                    </div>
                ) : (
                    <p
                        className="text-content"
                        >
                        {textContent}
                    </p>
                )}
            </Card.Body>
    
            <Card.Footer >
                <div className="like-comment-container" >
                    <div className="heart-icon" onClick={handleLikeClick}>
                        <FaHeart color={liked ? 'red' : 'black'} size={24} />
                        <p>{likesCount}</p>
                        </div>
                    <div className="comment-icon" onClick={handleToggleComments}>
                        <FaComment color='black' size={24} />
                        <p>{comments.length}</p>
                    </div>
                </div>
            </Card.Footer>
            {showComments && (
                <div className="comments-section">
                    {comments.map((comment, index) => (
                        <div key={index} className="comment">
                            <a href={comment.author.profileUrl} className="comment-author">
                                {comment.author.username}
                            </a>
                            <span className="comment-text">{comment.text}</span>
                        </div>
                    ))}
                </div>
            )}
        </Card>
    );
};

export default PostCards;
