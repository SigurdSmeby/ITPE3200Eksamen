import React, { useEffect, useState } from 'react';
import { User, fetchUserById } from '../components/api.ts';
import ProfileCard from '../components/ProfileCard.tsx';
import { flattenAndSortImages } from '../components/imageUtils.ts'; // Import the utility function

const userId = 1; // Change this ID manually to fetch different users

const Profile: React.FC = () => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadUser = async () => {
      try {
        const data = await fetchUserById(userId);
        setUser(data);
        setLoading(false);
      } catch (err) {
        console.error(err);
        setError('Error fetching user data');
        setLoading(false);
      }
    };

    loadUser();
  }, []);

  // If user exists, use the utility function to flatten and sort images
  const sortedImages = user ? flattenAndSortImages([user]) : [];

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;
  if (!user) return <div>User not found</div>;

  const HeroSection: React.FC<{ user: User }> = ({ user }) => {
    const numberOfPosts = user.images.length;
  
    return (
      <div className="hero-section d-flex m-5">
        <div className='me-5' style={{ height: '150px', width: '150px' }}>
          <img src={user.profilePicture} alt={user.profileName} className='img-fluid' style={{ width: '100%', borderRadius: '50%' }} />
        </div>
        
        <div>
          <h1>{user.profileName}</h1>
          <p>A brief description or bio about {user.profileName}</p> {/* You can customize this part */}
          <p className='btn btn-light'>Number of posts: {numberOfPosts}</p>
        </div>
      </div>
    );
  }
  
  const ImgSection: React.FC<{ user: User }> = ({ user }) => {
    return (
      <div>
      {sortedImages.map((image, index) => (
        <ProfileCard
          key={index}
          name={image.userName}
          profileImage={image.profileImage}
          bodyImage={image.url}
          date={image.date}
        />
      ))}
    </div>
    );
  }

  return (
    <>
      <HeroSection user={user}/>
      <ImgSection user={user}/>
    </>
    
  );
};

export default Profile;
