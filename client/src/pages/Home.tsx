import React, { useEffect, useState } from 'react';
import PostList from '../components/PostList.tsx';
import { getPosts } from '../services/api.ts';

const Home: React.FC = () => {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    getPosts().then(data => setPosts(data));
  }, []);

  return (
    <div>
      <h1>Feed</h1>
      <PostList posts={posts} />
    </div>
  );
};

export default Home;
