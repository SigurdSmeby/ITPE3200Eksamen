import React, { useState, useEffect } from 'react';
import { fetchCommentsForPost, deleteComment } from '../api/commentApi';
import { timeAgo } from './timeAgo';
import { useAuth } from './shared/AuthContext';

const CommentsSection = ({ postId, refresh, onCommentDelete  }) => {
    const [comments, setComments] = useState([]);
    const { username } = useAuth(); // Get the logged-in username

    const handleDeleteComment = async (commentId) => {
        try {
            await deleteComment(commentId); // Call the API to delete the comment
            setComments((prevComments) =>
                prevComments.filter((comment) => comment.commentId !== commentId)
            ); // Update the state to remove the deleted comment
            if (onCommentDelete) onCommentDelete(); // Notify parent to decrement comments count
        } catch (error) {
            console.error('Error deleting comment:', error);
        }
    };

    useEffect(() => {
        // Fetch comments for the given postId
        fetchCommentsForPost(postId)
            .then((response) => {
                console.log('Fetched comments:', response);
                setComments(response); // Set the comments in the state
            })
            .catch((error) => {
                console.error('Error fetching comments:', error);
            });
    }, [postId, refresh]); // Include 'postId'
    

    return (
        <div>
            {comments.length === 0 ? (  
                <p>No comments yet. Be the first to comment!</p>
            ) : (
                comments.map((comment) => (
                    <div
                        key={comment.commentId}
                        style={{
                            marginBottom: '5px',
                            border: '1px solid #ddd',
                            paddingRight: '10px',
                            paddingLeft: '10px',
                            borderRadius: '5px',
                            display: 'flex',
                            flexDirection: 'column',
                            position: 'relative',
                        }}
                    >
                        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                            <div>
                                <strong className="me-2 mb-0">
                                    {comment.authorUsername}
                                </strong>
                                <p className="mb-0">{comment.content}</p>
                            </div>
                            <div>
                                <p
                                    className="mb-0"
                                    title={new Date(comment.dateCommented).toLocaleString()}
                                    style={{
                                        fontSize: '12px',
                                        color: 'gray',
                                        textAlign: 'right',
                                    }}
                                >
                                    {timeAgo(comment.dateCommented)}
                                </p>
                                {comment.authorUsername === username && (
                                    <button
                                        onClick={() => handleDeleteComment(comment.commentId)}
                                        className="btn btn-danger btn-sm"
                                        style={{
                                            fontSize: '10px',
                                            padding: '2px 5px',
                                        }}
                                    >
                                        Delete
                                    </button>
                                )}
                            </div>
                        </div>
                    </div>
                ))
            )}
        </div>
    );
};

export default CommentsSection;
