import React from 'react';
import { Container } from 'react-bootstrap';


const Footer: React.FC = () => {
  return (
    <footer className="footer bg-primary text-white text-center py-4">
        <Container>
          <p>&copy; 2024 MySite. All rights reserved.</p>
        </Container>
      </footer>
  );
};

export default Footer;