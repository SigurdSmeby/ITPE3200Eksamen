import React from 'react';
import { Card, Button, Dropdown } from 'react-bootstrap';
import { FaHeart, FaComment } from 'react-icons/fa';
import { deletePost } from '../api/postApi';
import { createComment, fetchCommentsForPost } from '../api/commentApi';
import CommentsSection from './commentsSection';
import { timeAgo } from './timeAgo';
import './postCards.css';

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
    const [commentsInput, setCommentsInput] = React.useState(String);
    const [comments, setComments] = React.useState([]);
    // Get logged-in username from localStorage
    const loggedInUsername = localStorage.getItem('username');
    const isOwner = loggedInUsername === author?.username;

    const handleLikeClick = () => {
        setLiked(!liked);
    };
    const handleToggleComments = () => {
        setShowComments(!showComments);
    };
    const handleSendComment = () => {
        // send comment to backend
        const commentData = { PostId: postId, Content: commentsInput };
        createComment(commentData)
            .then((response) => {
                console.log(response.data);
            })
            .catch((error) => {
                console.log(error);
            });
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
            <Card.Header>
                <a href={profileUrl} className="profile-link">
                    <Card.Img
                        variant="top"
                        src={profilePicture}
                        alt="User profile"
                        loading="lazy"
                        className="profile-img"
                    />
                    <h4>{authorName}</h4>
                </a>
                <p
                    className="date"
                    title={new Date(dateUploaded).toLocaleString()}>
                    {timeAgo(dateUploaded)}
                </p>

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
                    fontSize: fontSize ? `${fontSize}px` : undefined,
                    color: textColor || undefined,
                    backgroundColor: backgroundColor || undefined,
                }}>
                {imageUrl ? (
                    <div
                        className="image-container"
                        style={{ backgroundImage: `url(${imageUrl})` }}>
                        <img
                            src={imageUrl}
                            alt={title} //bruker tittel her litt for å bruke det, kan evt velge å bruke filnavn
                            loading="lazy"
                            className="post-image"
                        />
                    </div>
                ) : (
                    <p className="text-content">{textContent}</p>
                )}
            </Card.Body>

            <Card.Footer>
                <div className="like-comment-container">
                    <div
                        className="heart-icon-container"
                        onClick={handleLikeClick}>
                        <FaHeart
                            className={
                                liked ? 'heart-icon-red' : 'heart-icon-black'
                            }
                            size={24}
                        />
                        <p>{likesCount}</p>
                    </div>
                    <div
                        className="comment-icon-container"
                        onClick={handleToggleComments}>
                        <FaComment
                            className="comment-icon"
                            color="black"
                            size={24}
                        />
                        <p>{comments.length}</p>
                    </div>
                </div>
            </Card.Footer>
            {showComments && (
                <div className="comments-section">
                    {/*comments.map((comment, index) => (
                        <div key={index} className="comment">
                            <a
                                href={comment.author.profileUrl}
                                className="comment-author">
                                {comment.author.username}
                            </a>
                            <span className="comment-text">{comment.text}</span>
                        </div>
                    ))*/}
                    <CommentsSection postId={postId} />
                    <div style={{ display: 'flex' }}>
                        <input
                            type="text"
                            className="comment-field form-control"
                            placeholder="comment here"
                            onChange={(e) => setCommentsInput(e.target.value)}
                        />
                        <button
                            className="comment-button btn"
                            onClick={handleSendComment}>
                            Comment
                        </button>
                    </div>
                </div>
            )}
        </Card>
    );
};

export default PostCards;
