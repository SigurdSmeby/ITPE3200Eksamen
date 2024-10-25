import React, { useEffect, useState } from 'react';
import { Nav, Navbar, NavDropdown, Container } from 'react-bootstrap';
import { FaHome } from 'react-icons/fa';

const NavMenu: React.FC = () => {
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);

    // Check if token exists in localStorage
    useEffect(() => {
        const token = localStorage.getItem('jwtToken');
        setIsLoggedIn(!!token); // Set isLoggedIn to true if token exists
    }, []);

    // Handle Logout
    const handleLogout = () => {
        localStorage.removeItem('jwtToken'); // Remove the token from localStorage
        setIsLoggedIn(false); // Update the state to re-render the Navbar
    };

    return (
        <Navbar
            bg="light"
            expand="lg"
            className="custom-navbar shadow-sm navbar">
            <Container>
                <Navbar.Brand href="/" className="fw-bold text-primary">
                    <FaHome className="me-2" />
                    Home
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

                        {/* Conditionally render Login/Logout based on isLoggedIn */}
                        {isLoggedIn ? (
                            <>
                                {/* Show these links only when logged in */}
                                <Nav.Link
                                    href="/profile"
                                    className="mx-2 text-secondary">
                                    Profile
                                </Nav.Link>
                                <Nav.Link
                                    href="/settings"
                                    className="mx-2 text-secondary">
                                    Settings
                                </Nav.Link>
                                <Nav.Link
                                    href="/"
                                    onClick={handleLogout}
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

                        <NavDropdown
                            title="Dropdown"
                            id="basic-nav-dropdown"
                            className="mx-2">
                            <NavDropdown.Item href="/test">
                                TestPage
                            </NavDropdown.Item>
                            <NavDropdown.Item href="#action/3.2">
                                Action
                            </NavDropdown.Item>
                            <NavDropdown.Divider />
                            <NavDropdown.Item href="#action/3.3">
                                Action
                            </NavDropdown.Item>
                        </NavDropdown>
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};

export default NavMenu;
