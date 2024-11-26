import React, { useState } from 'react';
import { FaHeart } from 'react-icons/fa';
import { useAuth } from './shared/AuthContext.tsx';
import { useNavigate } from 'react-router-dom';
import { createLike, unLike } from '../api/likeApi.js';

const LikeButton = ({postId, likeCounter, hasLiked: initialHasLiked}) => {
  const [likes, setLikes] = useState(0);
  const [hasLiked, setHasLiked] = useState(false);
  const { isLoggedIn } = useAuth();
  const navigate = useNavigate();

  const logId = postId;
  
  React.useEffect(() => {
    setLikes(likeCounter);
    setHasLiked(initialHasLiked);
}, [likeCounter,initialHasLiked]); 

  const handleLike = () => {
    if (!isLoggedIn) {
      navigate('/login', { state: { from: `/` } }); // Redirect to login
      return;
  }
    if (!hasLiked) {
      setLikes(likes + 1);
      setHasLiked(true);
      createLike(postId)
            .then((response) => {
                console.log('liked:', response);
            })
            .catch((error) => {
                console.error('Error finding likes:', error);
            });
            console.log(logId)
    } else {
      unLike(postId)
      .then((response) => {
          console.log('unliked:', response);
      })
      .catch((error) => {
          console.error('Error finding likes:', error);
      });
      setLikes(likes - 1);
      setHasLiked(false);
    }
  };

  return (
    <>
      <div onClick={handleLike} style={{ display: 'block' }}>
        <FaHeart
          style={{ color: hasLiked ? 'red' : 'black' }}
          size={24}
        />
        <p>{likes}</p>
      </div>
      <div style={{ display: 'block' }}>
      </div>
    </>
  );
};

export default LikeButton;
                        
                 

