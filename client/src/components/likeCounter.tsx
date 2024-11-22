import React, { useState } from 'react';
import { FaHeart } from 'react-icons/fa';

const LikeButton: React.FC = () => {
  const [likes, setLikes] = useState<number>(0);
  const [hasLiked, setHasLiked] = useState<boolean>(false);

  const handleLike = () => {
    if (!hasLiked) {
      setLikes(likes + 1);
      setHasLiked(true);
    } else {
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
        <p>{likes} {likes === 1 ? 'like' : 'likes'}</p>
      </div>
      <div style={{ display: 'block' }}>
        
      </div>
    </>
  );
};

export default LikeButton;
                        
                 

