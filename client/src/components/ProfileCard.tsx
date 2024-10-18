import React from 'react';
import { Card, Button } from 'react-bootstrap';
import { FaHeart } from "react-icons/fa";
import { Nav, Dropdown} from "react-bootstrap";

// Format date function
const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = String(date.getFullYear()).slice(-2);
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${day}.${month}.${year} - ${hours}.${minutes}`;
};

const ProfileCard = ({ name, profileImage, bodyImage, date }) => {
  const [liked, setLiked] = React.useState(false);
  const handleLikeClick = () => {
    setLiked(!liked);
  };

  return (
    <Card style={{ width: '30rem', margin: '1rem auto' }}>
      <Card.Header className="d-flex align-items-center">
        <Card.Img
          variant="top"
          src={profileImage}
          alt="User profile"
          loading="lazy"
          style={{ width: '50px', height: '50px', borderRadius: '50%', marginRight: '1rem' }}
        />
        <h4>{name}</h4>

        <Dropdown className="ms-auto">
          <Dropdown.Toggle variant="secondary" id="dropdown-basic">
           menu
          </Dropdown.Toggle>

          <Dropdown.Menu>
            <Dropdown.Item href="#/action-1">Edit</Dropdown.Item>
            <Dropdown.Item href="#/action-2">Delete</Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      </Card.Header>

      <Card.Body style={{ maxHeight: '30rem', alignContent: 'center', overflow: 'hidden', marginBottom: '5px' }}>
        {/* Format the date here */}
        <p style={{ margin: '0' }}>{formatDate(date)}</p>
        <Card.Img
          variant="top"
          src={bodyImage}
          alt="Body Image"
          loading="lazy"
          style={{ width: '100%', height: '100%', objectFit: 'contain' }}
        />
      </Card.Body>

      <Card.Footer className="text-center">
        <div
          style={{ display: 'inline-block', cursor: 'pointer' }}
          onClick={handleLikeClick}
        >
          <FaHeart color={liked ? 'red' : 'black'} size={24} />
        </div>
        <Button variant="secondary">Comment</Button>
      </Card.Footer>
    </Card>
  );
};

export default ProfileCard;
