import React, { useState, useEffect } from 'react';
import { fetchCommentsForPost, deleteComment } from '../api/commentApi';
import { timeAgo } from './timeAgo';
import { useAuth } from './shared/AuthContext';

const CommentsSection = ({ postId, refresh, onCommentDelete }) => {
    const [comments, setComments] = useState([]);
    const { username } = useAuth();

    // Function to handle the deletion of a comment
    const handleDeleteComment = async (commentId) => {
        await deleteComment(commentId);
        setComments(
            (
                prevComments, //uppdates the comments array
            ) =>
                prevComments.filter(
                    (comment) => comment.commentId !== commentId,
                ),
        );
        if (onCommentDelete) onCommentDelete();
    };

    // Fetch comments for the given post whenever postId or refresh changes
    useEffect(() => {
        fetchCommentsForPost(postId).then((response) => {
            setComments(response); //updates the comments array
        });
    }, [postId, refresh]);

    return (
        <div>
            {/* Display a message if there are no comments*/}
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
                        }}>
                        <div
                            style={{
                                display: 'flex',
                                justifyContent: 'space-between',
                            }}>
                            {/* Display the author's username and comment content */}
                            <div>
                                <strong
                                    className="me-2 mb-0"
                                    style={{ wordBreak: 'break-word' }}>
                                    {comment.authorUsername}
                                </strong>
                                <p
                                    className="mb-0"
                                    style={{ wordBreak: 'break-word' }}>
                                    {comment.content}
                                </p>
                            </div>
                            <div>
                                <p
                                    className="mb-0"
                                    title={new Date(
                                        comment.dateCommented,
                                    ).toLocaleString()}
                                    style={{
                                        fontSize: '12px',
                                        color: 'gray',
                                        textAlign: 'right',
                                    }}>
                                    {timeAgo(comment.dateCommented)}
                                </p>
                                {/* Display a delete button if the comment author is the current user */}
                                {comment.authorUsername === username && (
                                    <button
                                        onClick={() =>
                                            handleDeleteComment(
                                                comment.commentId,
                                            )
                                        }
                                        className="btn btn-danger btn-sm"
                                        style={{
                                            fontSize: '10px',
                                            padding: '2px 5px',
                                        }}>
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
