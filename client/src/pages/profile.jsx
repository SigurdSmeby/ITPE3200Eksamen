import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getUserPosts } from '../api/postApi';
import PostCards from '../components/postCards.tsx';

const userId = 2; // Change this ID manually to fetch different users

const Profile = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  const [userName, setUserName] = useState('');
  const [profilePicture, setProfilePicture] = useState('');
  const [bio, setBio] = useState('');


  const { username } = useParams();
  //console.log(username);
  //console.log(posts);

  useEffect(() => {
    const fetchUserPosts = async () => {
      try {
        const response = await getUserPosts(username);
        console.log("response: "+response);
        setPosts(response.posts);
        setUserName(response.username);
        setProfilePicture(response.profilePictureUrl);
        setBio(response.bio);
      } catch (err) {
        setError(err.response.data);
      }
    };

    fetchUserPosts(); // Call the async function
  }, []); 


  const HeroSection = () => {
    const numberOfPosts = posts.length;
  
    return (
      <div className="hero-section d-flex m-5">
        <div className='me-5' style={{ height: '150px', width: '150px' }}>
          <img src={profilePicture} alt={userName} className='img-fluid' style={{ width: '100%', borderRadius: '50%' }} />
        </div>
        
        <div>
          <h1>{userName}</h1>
          <p>{bio}</p> {/* You can customize this part */}
          <p className='btn btn-light'>Number of posts: {numberOfPosts}</p>
        </div>
      </div>
    );
  }
  
  const ImgSection= () => {
    if (error) {
      return <h1>{error}</h1>;
    }
    if (posts.length === 0) {
      return <h1>the user have not posted any images yet</h1>;
    }
    return (
      <div>
        {posts
          .sort((a, b) => new Date(b.dateUploaded) - new Date(a.dateUploaded))
          .map(post => (
            <PostCards 
              key={post.postId}
              postId={post.postId}
              imageUrl={post.imageUrl}
              title={post.title}
              dateUploaded={post.dateUploaded}
              author={post.author}
              likesCount={post.likesCount}
              />
          ))}
    </div>
    );
  }
  

  return (
    <>
      <HeroSection />
      <ImgSection />  
    </>
    
  );
};

export default Profile;
