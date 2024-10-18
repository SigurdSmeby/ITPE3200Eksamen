import React, { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import ProfileCard from '../components/ProfileCard.tsx';
import { User, fetchUsers } from '../components/api.ts';
import { flattenAndSortImages } from '../components/imageUtils.ts'; // Import the utility function

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
        console.error(err);  // Log any error for debugging
        setError('Error fetching user data');
        setLoading(false);
      }
    };

    loadUsers();
  }, []);

  // Use the utility function to flatten and sort images
  const sortedImages = flattenAndSortImages(users);

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
