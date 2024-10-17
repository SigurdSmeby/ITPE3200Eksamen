import React from "react";
import { Nav, Navbar} from "react-bootstrap";
import { FaHome, FaInfoCircle, FaEnvelope, FaRegUser} from 'react-icons/fa'; // Importing Font Awesome icons
import 'bootstrap/dist/css/bootstrap.min.css'; // Ensure Bootstrap's CSS is imported

const NavMenu: React.FC = () => {
  return (
    <Navbar bg="light" expand="lg" className="flex-column vh-100" style={{ position: 'fixed', left: 0, top: 0, width: '250px' }}>
      {/* Navbar brand (logo) with an icon */}
      <Navbar.Brand href="/" className="fw-bold d-flex align-items-center mb-4" style={{color: 'rgb(245, 101, 48)'}}>
        <FaHome className="me-2" /> Home
      </Navbar.Brand>

      {/* Navbar toggle button for mobile view */}
      <Navbar.Toggle aria-controls="basic-navbar-nav" className="mb-3" />

      {/* Collapsible section for the navigation links */}
      <Navbar.Collapse id="basic-navbar-nav" className="w-100">
        <Nav className="flex-column w-100">
          {/* Home link with icon */}
          <Nav.Link href="/" className="text-secondary">
            <FaHome className="me-2" /> Home
          </Nav.Link>

          {/* Profile link with icon */}
          <Nav.Link href="/profile" className="text-secondary">
            <FaRegUser className="me-2" /> Profile
          </Nav.Link>

          {/* About link with icon */}
          <Nav.Link href="/about" className="text-secondary">
            <FaInfoCircle className="me-2" /> About
          </Nav.Link>

          {/* Login link with icon */}
          <Nav.Link href="/login" className="text-secondary">
            <FaEnvelope className="me-2" /> Login
          </Nav.Link>
          
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default NavMenu;



/*
import React from "react";
import { Nav, Navbar, NavDropdown } from "react-bootstrap";
import { FaHome, FaInfoCircle, FaEnvelope, FaChevronDown } from 'react-icons/fa';
import 'bootstrap/dist/css/bootstrap.min.css';

const NavMenu: React.FC = () => {
  return (
    <Navbar bg="light" expand="lg" className="flex-column vh-100" style={{ position: 'fixed', left: 0, top: 0, width: '250px' }}>
      <Navbar.Brand href="/" className="fw-bold text-primary d-flex align-items-center mb-4">
        <FaHome className="me-2" /> Home
      </Navbar.Brand>
      <Navbar.Toggle aria-controls="basic-navbar-nav" className="mb-3" />
      <Navbar.Collapse id="basic-navbar-nav" className="w-100">
        <Nav className="flex-column w-100">
          <Nav.Link href="/">
            <FaHome className="me-2" /> Home
          </Nav.Link>
          <Nav.Link href="/about" >
            <FaInfoCircle className="me-2" /> About
          </Nav.Link>
          <Nav.Link href="/login" >
            <FaEnvelope className="me-2" /> Login
          </Nav.Link>
          <NavDropdown title={<span className="text-secondary d-flex align-items-center"><FaChevronDown className="me-2" /> Dropdown</span>} id="basic-nav-dropdown" className="mx-2">
            <NavDropdown.Item href="#action/3.1">ACTION 1</NavDropdown.Item>
            <NavDropdown.Item href="#action/3.2">ACTION 2</NavDropdown.Item>
            <NavDropdown.Divider />
            <NavDropdown.Item href="#action/3.3">ACTION 3</NavDropdown.Item>
          </NavDropdown>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default NavMenu;
*/