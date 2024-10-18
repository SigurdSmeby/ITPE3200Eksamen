import React, { useEffect, useState } from 'react';
import ProfileCard from '../components/ProfileCard.tsx'; // Assuming this is the correct path for ProfileCard
import { User, fetchUserById } from '../api.ts'; // Import the fetchUserById function from the API file

// Manually define the userId
const userId = 1; // Change this ID manually to fetch different users

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
    <div className="img-section">
      {user.images.map((bodyImage, index) => (
        <ProfileCard
          key={index}
          name={user.profileName}
          profileImage={user.profilePicture}
          bodyImage={bodyImage.imageUrl}  // Matches 'imageUrl' from the API response
          date={bodyImage.uploadedAt}  // Matches 'uploadedAt' from the API response
        />
      ))}
    </div>
  );
}

const Profile: React.FC = () => {
  const [user, setUser] = useState<User | null>(null); // Initialize user state
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadUser = async () => {
      try {
        const data = await fetchUserById(userId); // Fetch user by ID
        setUser(data);
        setLoading(false);
      } catch (err) {
        console.error(err); // Log the error
        setError('Error fetching user data');
        setLoading(false);
      }
    };

    loadUser();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;
  if (!user) return <div>User not found</div>;

  return (
    <>
      <HeroSection user={user} />
      <ImgSection user={user} />
    </>
  );
};

export default Profile;
