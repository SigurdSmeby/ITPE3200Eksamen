import React from 'react';
import { Nav, Navbar } from 'react-bootstrap';
import { FaHome } from 'react-icons/fa';
import { useAuth } from './AuthContext.tsx';
import logo from './icons/webappLogo.png';

const NavMenu: React.FC = () => {
    const { isLoggedIn, username, logout } = useAuth();

    return (
        <Navbar
            bg="light"
            expand="lg"
            className="d-flex flex-column"
            style={{
                position: 'fixed',
                height: '100vh',
                width: '250px',
                backgroundColor: '#f8f9fa',
                borderRight: '1px solid #ddd',
                padding: '1rem',
                zIndex: 1000,
            }}>
            <Navbar.Brand
                href="/"
                className="fw-bold text-primary mb-4 d-flex align-items-center">
                <img src={logo} className="me-2" 
                style={{
                    width: '150px',
                    height: 'auto',
                    alignItems:'start'
                }} alt="Home" />
            </Navbar.Brand>
            <Nav className="flex-column w-100">
                <Nav.Link href="/" className="py-2 px-3">
                    Home
                </Nav.Link>
                <Nav.Link href="/about" className="py-2 px-3">
                    About
                </Nav.Link>
                {isLoggedIn ? (
                    <>
                        <Nav.Link
                            href={`/profile/${username}`}
                            className="py-2 px-3">
                            Profile
                        </Nav.Link>
                        <Nav.Link href="/upload" className="py-2 px-3">
                            Upload
                        </Nav.Link>
                        <Nav.Link href="/settings" className="py-2 px-3">
                            Settings
                        </Nav.Link>
                        <Nav.Link
                            href="/"
                            onClick={logout}
                            className="py-2 px-3">
                            Logout
                        </Nav.Link>
                    </>
                ) : (
                    <Nav.Link href="/login" className="py-2 px-3">
                        Login
                    </Nav.Link>
                )}
            </Nav>
        </Navbar>
    );
};

export default NavMenu;

/*import React from 'react';
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
*/
