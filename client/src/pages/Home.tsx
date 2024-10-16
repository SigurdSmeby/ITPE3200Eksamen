import React from 'react';
import { Container, Row, Col, Button, Card } from 'react-bootstrap';
import NavMenu from '../components/shared/navbar.tsx';


const Home: React.FC = () => {
  return (
    <>
      <header className="hero-section text-white text-center">
        <Container>
          <h1 className="display-4">Welcome to MySite</h1>
          <p className="lead">Discover amazing features and content</p>
          <Button variant="light" size="lg" href="/about">Learn More</Button>
        </Container>
      </header>
      <Container className="my-5">
        <h2 className="text-center mb-4">Our Features</h2>
        <Row className="text-center">
          <Col md={4}>
            <Card>
              <Card.Body>
                <Card.Title>Feature 1</Card.Title>
                <Card.Text>Detailed information about Feature 1.</Card.Text>
              </Card.Body>
            </Card>
          </Col>
          <Col md={4}>
            <Card>
              <Card.Body>
                <Card.Title>Feature 2</Card.Title>
                <Card.Text>Detailed information about Feature 2.</Card.Text>
              </Card.Body>
            </Card>
          </Col>
          <Col md={4}>
            <Card>
              <Card.Body>
                <Card.Title>Feature 3</Card.Title>
                <Card.Text>Detailed information about Feature 3.</Card.Text>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default Home;
