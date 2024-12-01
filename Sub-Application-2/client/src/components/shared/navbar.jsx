import React from 'react';
import { Nav, Navbar, Container } from 'react-bootstrap';
import logo from './icons/webappLogo2.png';
import { useAuth } from './AuthContext';
import './navbar.css'; // Ensure CSS is loaded

const NavMenu = () => {
    const { isLoggedIn, username, logout } = useAuth(); // Access authentication state

    return (
        <Navbar
            bg="light"
            expand="lg"
            className="navbar-main custom-navbar shadow-sm"
        >
            <Container>
                <Navbar.Brand href="/" className="fw-bold text-primary">
                    <img role="heading" aria-level="1" //instead of having a H1 header for this page
                        src={logo}
                        alt="Page logo"
                        className="logo"
                    />
                </Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav" /> {/* Button to toggle the navbar in mobile view */}
                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav className="d-flex justify-content-center align-items-center navbar-nav">
                        <Nav.Link href="/" className="nav-link">
                            Home
                        </Nav.Link>
                        {/* Conditional rendering for logged-in users */}
                        {isLoggedIn && (
                            <>
                                {/* Profile link for logged-in user */}
                                <Nav.Link href={`/profile/${username}`} className="nav-link">
                                    Profile
                                </Nav.Link>
                                {/* Upload link for logged-in user */}
                                <Nav.Link href="/upload" className="nav-link">
                                    Upload
                                </Nav.Link>
                                {/* Search link for logged-in user */}
                                <Nav.Link href="/search" className="nav-link">
                                    Search
                                </Nav.Link>
                                {/* Settings link for logged-in user */}
                                <Nav.Link href="/settings" className="nav-link">
                                    Settings
                                </Nav.Link>
                            </>
                        )}
                        {/* Show Login link when not logged in */}
                        {!isLoggedIn && (
                            <Nav.Link href="/login" className="nav-link">
                                Login
                            </Nav.Link>
                        )}
                        {/* Show Register link when not logged in */}
                        {!isLoggedIn && (
                            <Nav.Link href="/register" className="nav-link">
                                Register
                            </Nav.Link>
                        )}
                        {/* About link always visible */}
                        <Nav.Link href="/about" className="nav-link">
                            About
                        </Nav.Link>
                        {/* Logout link for logged-in users */}
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
