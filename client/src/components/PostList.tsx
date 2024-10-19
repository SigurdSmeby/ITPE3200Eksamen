import React from 'react';
import Post from './Post.tsx';

const PostList: React.FC<{ posts: any[] }> = ({ posts }) => {
  return (
    <div>
      {posts.map(post => (
        <Post key={post.id} post={post} />
      ))}
    </div>
  );
};

export default PostList;
