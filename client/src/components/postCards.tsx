import React from 'react';
import { Card, Button, Dropdown } from 'react-bootstrap';
import { FaComment, FaEllipsisH, FaTrash } from 'react-icons/fa';
import { deletePost } from '../api/postApi';
import { createComment } from '../api/commentApi';
import CommentsSection from './commentsSection';
import { timeAgo } from './timeAgo';
import LikeButton from './likeCounter.tsx';
import { useAuth } from './shared/AuthContext.tsx';
import { useNavigate } from 'react-router-dom';
import './postCards.css';

const BACKEND_URL = 'http://localhost:5229';
// PostCards component
const PostCards = ({
    postId,
    imagePath: imageUrl,
    textContent,
    dateUploaded,
    author,
    likesCount,
    commentsCount: initialCommentsCount,
    fontSize,
    textColor,
    backgroundColor,
    onDeleted,
}) => {
    const [liked, setLiked] = React.useState(false);
    const [showComments, setShowComments] = React.useState(false);
    const [commentsInput, setCommentsInput] = React.useState('');
    const [refreshComments, setRefreshComments] = React.useState(false); // Track when to refresh comments
    const [commentsCount, setCommentsCount] = React.useState(initialCommentsCount);
    const { isLoggedIn } = useAuth();
    const navigate = useNavigate();

    const loggedInUsername = localStorage.getItem('username');
    const isOwner = loggedInUsername === author?.username;



    const handleToggleComments = () => {
        if (!isLoggedIn) {
            navigate('/login', { state: { from: `/post/${postId}` } }); // Redirect to login
            return;
        }
        setShowComments(!showComments);
    };
    const incrementCommentsCount = () => {
        setCommentsCount((prevCount) => prevCount + 1);
    };

    const decrementCommentsCount = () => {
        setCommentsCount((prevCount) => prevCount - 1);
    };

    const handleSendComment = () => {
        const commentData = { PostId: postId, Content: commentsInput };

        createComment(commentData)
            .then((response) => {
                console.log('Comment created:', response.data);
                setCommentsInput(''); // Clear the input field
                setRefreshComments((prev) => !prev); // Trigger comments refresh
                incrementCommentsCount();
            })
            .catch((error) => {
                console.error('Error creating comment:', error);
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
                        src={BACKEND_URL + profilePicture}
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
                        className="image-container">
                        <img
                            src={BACKEND_URL + imageUrl}
                            loading="lazy"
                            className="post-image"
                        />
                    </div>
                ) : (
                    <p className="text-content mb-0" style={{ whiteSpace: 'pre-wrap' }}>{textContent}</p>
                )}
            </Card.Body>

            <Card.Footer>
                <div className="like-comment-container">
                    <span className="heart-icon-container "><LikeButton/></span>
                    <div
                        className="comment-icon-container"
                        onClick={handleToggleComments}>
                        <FaComment
                            className="comment-icon"
                            color="black"
                            size={24}
                        />
                        <p>{commentsCount}</p>
                    </div>
                </div>
            </Card.Footer>
            {showComments && (
                <>
                    <div className="comments-section">
                        <CommentsSection
                            postId={postId}
                            refresh={refreshComments} // Pass the refresh flag
                            onCommentDelete={decrementCommentsCount}
                        />
                    </div>
                    <form
                        className="comments-section"
                        style={{ display: 'flex' }}
                        onSubmit={(e) => {
                            e.preventDefault();
                            handleSendComment();
                        }}
                    >
                        <input
                            type="text"
                            className="comment-field form-control"
                            placeholder="Comment here"
                            value={commentsInput}
                            onChange={(e) => setCommentsInput(e.target.value)}
                        />
                        <button
                            type="submit"
                            className="comment-button btn"
                        >
                            Comment
                        </button>
                    </form>
                </>
            )}
        </Card>
    );
};

export default PostCards;
