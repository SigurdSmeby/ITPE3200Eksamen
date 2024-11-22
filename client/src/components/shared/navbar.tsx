// NavMenu.tsx
import React from 'react';
import { Nav, Navbar, Container } from 'react-bootstrap';
import { FaHome } from 'react-icons/fa';
import { useAuth } from './AuthContext.tsx';

const NavMenu: React.FC = () => {
    const { isLoggedIn, username, logout } = useAuth();

    return (
        <Navbar
            bg="light"
            expand="lg"
            className="custom-navbar shadow-sm navbar">
            <Container>
                <Navbar.Brand href="/" className="fw-bold text-primary">
                    <FaHome className="me-2" /> Home
                </Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav className="me-auto">
                        <Nav.Link href="/" className="mx-2 text-secondary">
                            Home
                        </Nav.Link>
                        <Nav.Link href="/about" className="mx-2 text-secondary">
                            About
                        </Nav.Link>
                        {isLoggedIn ? (
                            <>
                                <Nav.Link
                                    href={`/profile/${username}`}
                                    className="mx-2 text-secondary">
                                    Profile
                                </Nav.Link>
                                <Nav.Link
                                    href="/Upload"
                                    className="mx-2 text-secondary">
                                    Upload
                                </Nav.Link>
                                <Nav.Link
                                    href="/settings"
                                    className="mx-2 text-secondary">
                                    Settings
                                </Nav.Link>
                                <Nav.Link
                                    href="/"
                                    onClick={logout}
                                    className="mx-2 text-secondary">
                                    Logout
                                </Nav.Link>
                            </>
                        ) : (
                            <Nav.Link
                                href="/login"
                                className="mx-2 text-secondary">
                                Login
                            </Nav.Link>
                        )}
                        <Nav.Link href="/test" className="mx-2 text-secondary">
                            Test Page
                        </Nav.Link>
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};

export default NavMenu;
