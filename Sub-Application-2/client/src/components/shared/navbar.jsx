import React from 'react';
import { Nav, Navbar, Container } from 'react-bootstrap';
import logo from './icons/webappLogo2.png';
import { useAuth } from './AuthContext';
import './navbar.css'; // Ensure CSS is loaded

const NavMenu = () => {
    const { isLoggedIn, username, logout } = useAuth();

    return (
        <Navbar
            bg="light"
            expand="lg"
            className="navbar-main custom-navbar shadow-sm"
        >
            <Container>
                <Navbar.Brand href="/" className="fw-bold text-primary">
                    <img
                        src={logo}
                        alt="Home"
                        className="logo"
                    />
                </Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav className="d-flex justify-content-center align-items-center navbar-nav">
                        <Nav.Link href="/" className="nav-link">
                            Home
                        </Nav.Link>
                        {isLoggedIn && (
                            <>
                                <Nav.Link href={`/profile/${username}`} className="nav-link">
                                    Profile
                                </Nav.Link>
                                <Nav.Link href="/upload" className="nav-link">
                                    Upload
                                </Nav.Link>
                                <Nav.Link href="/settings" className="nav-link">
                                    Settings
                                </Nav.Link>
                            </>
                        )}
                        {!isLoggedIn && (
                            <Nav.Link href="/login" className="nav-link">
                                Login
                            </Nav.Link>
                        )}
                        <Nav.Link href="/about" className="nav-link">
                            About
                        </Nav.Link>
                        {isLoggedIn && (
                            <Nav.Link href="/" onClick={logout} className="nav-link">
                                Logout
                            </Nav.Link>
                        )}
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};

export default NavMenu;
