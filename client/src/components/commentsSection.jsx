import React, { useState, useEffect } from 'react';
import { fetchCommentsForPost } from '../api/commentApi';
import { timeAgo } from './timeAgo';

const CommentsSection = ({ postId }) => {
    const [comments, setComments] = useState([]);

    useEffect(() => {
        // Fetch comments for the given postId
        fetchCommentsForPost(postId)
            .then((response) => {
                console.log('Fetched comments:', response); // Log the response
                setComments(response); // Set the comments in the state
            })
            .catch((error) => {
                console.error('Error fetching comments:', error);
            });
    }, [postId]);

    return (
        <div>
            {comments.length === 0 ? (
                <p>No comments yet. Be the first to comment!</p>
            ) : (
                comments.map((comment) => (
                    <div
                        key={comment.commentId}
                        style={{
                            marginBottom: '10px',
                            border: '1px solid #ddd',
                            padding: '10px',
                            borderRadius: '5px',
                        }}>
                        <div
                            style={{
                                display: 'inline-flex',
                                marginBottom: '0',
                            }}>
                            <strong className="me-2 mb-0">
                                {comment.authorUsername}
                            </strong>
                            <p className="mb-0">{comment.content}</p>
                        </div>
                        <div style={{ fontSize: '10', marginTop: '0' }}>
                            {timeAgo(comment.dateCommented)}
                        </div>
                    </div>
                ))
            )}
        </div>
    );
};

export default CommentsSection;
