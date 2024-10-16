import React from "react";
import {Nav, Navbar, NavDropdown, Container} from "react-bootstrap";
import { FaHome, FaInfoCircle, FaEnvelope, FaChevronDown } from 'react-icons/fa';

const NavMenu: React.FC = () => {
  return (
    <Navbar bg="light" expand="lg" className="custom-navbar shadow-sm navbar">
      <Container>
        <Navbar.Brand href="/" className="fw-bold text-primary">
        <FaHome className="me-2" />Home
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
        <Nav className="me-auto">
          <Nav.Link href="/about" className="mx-2 text-secondary">About</Nav.Link>
          <Nav.Link href="/contact" className="mx-2 text-secondary">Contact</Nav.Link>
          <NavDropdown title="Dropdown" id="basic-nav-dropdown" className="mx-2">
            <NavDropdown.Item href="#action/3.1" >ACTION</NavDropdown.Item>
            <NavDropdown.Item href="#action/3.2">ACTION</NavDropdown.Item>
            <NavDropdown.Divider />
            <NavDropdown.Item href="#action/3.3">ACTION</NavDropdown.Item>
          </NavDropdown>
        </Nav>
      </Navbar.Collapse>
    </Container>
  </Navbar>
  );
};
export default NavMenu;