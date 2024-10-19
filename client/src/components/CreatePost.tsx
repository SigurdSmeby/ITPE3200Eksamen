import React, { useState } from 'react';
import { createPost } from '../services/api.ts';

const CreatePost: React.FC = () => {
  const [caption, setCaption] = useState('');
  const [imageUrl, setImageUrl] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const post = { caption, imageUrl, userId: 1 }; // Hent userId fra kontekst eller state
    await createPost(post);
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>Caption</label>
        <input value={caption} onChange={e => setCaption(e.target.value)} />
      </div>
      <div>
        <label>Image URL</label>
        <input value={imageUrl} onChange={e => setImageUrl(e.target.value)} />
      </div>
      <button type="submit">Create Post</button>
    </form>
  );
};

export default CreatePost;
