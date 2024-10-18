import React, { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import ProfileCard from '../components/ProfileCard.tsx';
import { User, fetchUsers } from '../api.ts'; // Import fetchUsers from the API file

const Home: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch user data from the server
  useEffect(() => {
    const loadUsers = async () => {
      try {
        const data = await fetchUsers(); // Fetch all users
        setUsers(data);
        setLoading(false);
      } catch (err) {
        console.error('Error fetching user data:', err);  // Log the full error for debugging
        setError('Error fetching user data');  // Set the error state
        setLoading(false);
      }
    };
  
    loadUsers();
  }, []);
  

  // Flatten the bodyImages and attach user details
  const allImages = users.flatMap((user) =>
    user.images.map((bodyImage) => ({
      url: bodyImage.imageUrl,  // Matches 'imageUrl' from the API response
      date: bodyImage.uploadedAt,  // Matches 'uploadedAt' from the API response
      userName: user.profileName,  // Matches 'profileName' from the API response
      profileImage: user.profilePicture,  // Matches 'profilePicture' from the API response
    }))
  );

  // Sort the images by date
  const sortedImages = allImages
    .filter((image) => image.url)
    .sort((b, a) => new Date(a.date).getTime() - new Date(b.date).getTime());

  if (loading) return <div>Loading...</div>;
  if (error) return <div>{error}</div>;

  return (
    <Container>
      {sortedImages.map((image, index) => (
        <ProfileCard
          key={index}
          name={image.userName}
          profileImage={image.profileImage}
          bodyImage={image.url}
          date={image.date} // Pass the date if you want to display it
        />
      ))}
    </Container>
  );
};

export default Home;
