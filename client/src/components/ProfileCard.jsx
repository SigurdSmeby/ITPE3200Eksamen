import React from 'react';
import { Card, Button } from 'react-bootstrap';
import { FaHeart } from "react-icons/fa";


const ProfileCard = ({ name, profileImage, bodyImage, date }) => {

  const [liked, setLiked] = React.useState(false);
  const handleLikeClick = () => {
    setLiked(!liked);
  };

  return (
    <Card style={{ width: '30rem', margin: '1rem auto' }}>
      {/* User Profile Header */}
      <Card.Header className="d-flex align-items-center">
        <Card.Img
          variant="top"
          src={profileImage}
          alt="User profile"
          style={{ width: '50px', height: '50px', borderRadius: '50%', marginRight: '1rem' }}
        />
        <h4>{name}</h4>
      </Card.Header>

      {/* Image in the Card Body */}
      <Card.Body style={{maxHeight: '30rem', alignContent: 'center', overflow: 'hidden', marginBottom: '5px'}}>
        <p style={{margin: '0'}}>{date}</p>
        <Card.Img
          variant="top"
          src={bodyImage}
          alt="Body Image"
          style={{ width: '100%', height: '100%', objectFit: 'contain' }}
        />
      </Card.Body>

      {/* Card Footer with Like and Comment Buttons */}
      <Card.Footer className="text-center">
        {/*<Button variant="primary" className="me-2" style={{backgroundColor: '#f56530', borderColor: '#f56530'}}>Like</Button>*/}
        <div style={{
          display: 'inline-block', // Makes the div behave like a button
          cursor: 'pointer'}}
          onClick={handleLikeClick}>
            <FaHeart color={liked ? 'red' : 'black'} size={24} />
        </div>
        <Button variant="secondary">Comment</Button>
      </Card.Footer>
    </Card>
  );
};

export default ProfileCard;
