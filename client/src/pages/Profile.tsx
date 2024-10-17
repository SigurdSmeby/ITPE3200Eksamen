import React from 'react';
import { users, User } from '../components/users.tsx'; // Importing the users and User interface from the correct file
import ProfileCard from '../components/ProfileCard.jsx'; // Assuming this is the correct path for ProfileCard

// Find the user "Baifan" from the users array
const user: User | undefined = users.find(user => user.name === 'Baifan');
const numberOfPosts = user?.bodyImages.length;


const HeroSection: React.FC = () => {
  // If the user is not found, we can display a fallback message
  if (!user) return <div>User not found</div>;

  return (
    <div className="hero-section d-flex m-5">
      <div className='me-5' style={{height:'150px', width: '150px'}}>
        <img src={user.profileImage} alt={user.name} className='img-fluid' style={{width: '100%', borderRadius: '50%'}}/>
      </div>
      
      <div>
        <h1>{user.name}</h1>
        <p>A brief description or bio about {user.name}</p> {/* You can customize this part */}
        <p className='btn btn-light'>Number of post: {numberOfPosts}</p>
      </div>
      
    </div>
  );
}

const ImgSection: React.FC = () => {
  // If the user is not found or has no body images
  if (!user || user.bodyImages.length === 0) return <div>No images to display</div>;

  return (
    <div className="img-section">
      {user.bodyImages.map((bodyImage, index) => (
        <ProfileCard
          key={index}
          name={user.name}
          profileImage={user.profileImage}
          bodyImage={bodyImage.url}
          date={bodyImage.date} 
        />
      ))}
    </div>
  );
}

const Profile: React.FC = () => {
  return (
    <>
      <HeroSection />
      <ImgSection />
    </>
  );
};

export default Profile;
