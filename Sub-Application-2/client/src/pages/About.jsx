import React from 'react';
import { Container } from 'react-bootstrap';

// About Us Page Component
const About = () => {
    return (
        <Container className="my-5">
            <h1>About Us</h1>
            <p>
            This is a web application project which is mainly about interacting and sharing pictures and notes with others. 
            The target users include photographers, writers, artists, students, and anyone interested in documenting 
            their thoughts and experiences through notes and images. The platform aims to foster a community where users 
            can engage with each other's content through comments and discussions.
            </p>
        </Container>
    );
};

export default About;
