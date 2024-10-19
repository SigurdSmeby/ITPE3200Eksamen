import React from 'react';

const Post: React.FC<{ post: any }> = ({ post }) => {
  return (
    <div className="post">
      <h3>{post.user.username}</h3>
      <img src={post.imageUrl} alt="Post content" />
      <p>{post.caption}</p>
    </div>
  );
};

export default Post;
