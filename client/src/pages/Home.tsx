import React from 'react';
import { Container, Row, Col, Button, Card } from 'react-bootstrap';
import ProfileCard from '../components/ProfileCard.jsx';
import { users } from '../components/users.tsx';


const Home: React.FC = () => {

  // Step 1: Flatten the bodyImages with corresponding user details
  const allImages = users.flatMap((user) =>
    user.bodyImages.map((bodyImage) => ({
      ...bodyImage,
      userName: user.name,
      profileImage: user.profileImage
    }))
  );

  // Step 2: Sort the images by date
  const sortedImages = allImages.filter(image => image.url).sort((b, a) => new Date(a.date).getTime() - new Date(b.date).getTime());

  return (
    <>
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
    </>
  );
};

export default Home;
