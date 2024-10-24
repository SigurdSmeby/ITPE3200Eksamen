import React, { useState, useEffect } from 'react';
import TestApi from '../api/testapi';
import PostCards from '../components/postCards.tsx'; // Import the PostCards component

const TestPage = () => {
  const [title, setTitle] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [posts, setPosts] = useState([]); // State to hold posts data
  const [profilePicture, setProfilePicture] = useState(''); // State to hold profile picture URL

  
  // Fetch posts on component load
  useEffect(() => {
    TestApi.getPosts().then((response) => {
      setPosts(response.data); // Set the posts state with API data
      console.log(response.data);
    });
  }, []); // Empty dependency array to run effect only once on mount

  const handleGetUser = () => {
    console.log("click");
    TestApi.getUsers()
      .then((response) => {
        console.log(response.data);
      });
  };

  const handleRegUser = () => {
    console.log("click");
    TestApi.register("testName", "test@exsample.com", "testPassword")
      .then((response) => {
        console.log(response.data);
      }).catch((error) => {
        console.log(error);
      });
  };

  const handleGetPosts = () => {
    console.log("click");
    TestApi.getPosts().then((response) => {
      console.log(response.data);
      setPosts(response.data);
    });
  };

  const handleLogout = () => {
    console.log("logged out and removed token");
    localStorage.removeItem("jwtToken");
  };

  const handleCreatePost = () => {
    console.log("click");
    console.log(title);
    console.log(imageUrl);
    
    TestApi.createPost(imageUrl, title).then((response) => {
      console.log(response.data);
    });
  };


  return (
    <>
      <div>
        <button onClick={handleGetUser}>getUser</button>
        <button onClick={handleRegUser}>send userdata</button>
        <button onClick={handleGetPosts}>GetPosts</button>
        <a href="/login">login</a>
        <button onClick={handleLogout}>logout</button>
      </div>
      
      <div>
        <input 
          type="text" 
          placeholder="title" 
          onChange={(e) => setTitle(e.target.value)}
        />
        <input 
          type="text" 
          placeholder="imgUrl" 
          onChange={(e) => setImageUrl(e.target.value)}
        />
        <button onClick={handleCreatePost}>submit</button>
      </div>
      <div>
        <input type="text" onChange={(e) => setProfilePicture(e.target.value)} placeholder="profilePictureURL" />
        <button onClick={() => {TestApi.updateProfilePicture(profilePicture)}}>change profilePicture</button>
      </div>
      <h4>picture site:</h4>
      <p>https://picsum.photos/[number]</p>

      {/* Render the post cards at the bottom of the page */}
      <div style={{ marginTop: '2rem' }}>
        {posts.map(post => (
          <PostCards 
            key={post.postId}  // Updated to match JSON structure
            postId={post.postId}
            imageUrl={post.imageUrl}
            title={post.title}
            dateUploaded={post.dateUploaded}
            author={post.author}
            likesCount={post.likesCount}
          />
        ))}
      </div>
    </>
  );
};

export default TestPage;