import { useState, useEffect } from 'react';
import { FaHeart } from 'react-icons/fa';
import { useAuth } from './shared/AuthContext';
import { useNavigate } from 'react-router-dom';
import { createLike, unLike } from '../api/likeApi.js';

const LikeButton = ({ postId, likeCounter, hasLiked: initialHasLiked }) => {
    // State to track likes and whether the user has liked the post
    const [likes, setLikes] = useState(0);
    const [hasLiked, setHasLiked] = useState(false);
    const { isLoggedIn } = useAuth(); // Check if the user is logged in
    const navigate = useNavigate();

    // Initialize likes and liked status when props change
    useEffect(() => {
        setLikes(likeCounter);
        setHasLiked(initialHasLiked);
    }, [likeCounter, initialHasLiked]);

    // Handle like/unlike logic
    const handleLike = () => {
        if (!isLoggedIn) {
            // Redirect to login if the user is not logged in
            navigate('/login', { state: { from: `/` } });
            return;
        }

        if (!hasLiked) {
            // Like the post
            setLikes(likes + 1);
            setHasLiked(true);
            createLike(postId);
        } else {
            // Unlike the post
            unLike(postId);
            setLikes(likes - 1);
            setHasLiked(false);
        }
    };

    return (
        <>
            {/* Like button with dynamic color based on liked status */}
            <div
                onClick={handleLike}
                style={{ display: 'block', cursor: 'pointer' }}>
                <FaHeart
                    style={{ color: hasLiked ? 'red' : 'black' }}
                    size={24}
                />
                <p>{likes}</p>
            </div>
        </>
    );
};

export default LikeButton;
