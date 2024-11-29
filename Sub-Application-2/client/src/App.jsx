import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import NavMenu from './components/shared/navbar';
import Home from './pages/Home.jsx';
import About from './pages/About.jsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import Footer from './components/shared/footer';
import LoginUser from './pages/Login.jsx';
import Register from './pages/Register.jsx';
import Profile from './pages/profile.jsx';
import UserSettings from './pages/UserSettings';
import PrivateRoute from './components/PrivateRoute.js';
import EditPost from './pages/EditPost.jsx';
import UploadPost from './pages/UploadPost.jsx';
import { AuthProvider } from './components/shared/AuthContext';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const App = () => {
    return (
        <>
        
        <ToastContainer />
        <AuthProvider>
            <Router>
                <div
                    style={{
                        display: 'flex',
                        flexDirection: 'column',
                        minHeight: '100vh', // Sørger for at høyden dekker hele skjermen
                    }}>
                    <NavMenu />
                    <div
                        style={{
                            flex: 1, // Sørger for at dette området strekker seg for å fylle mellomrom
                            marginLeft: '250px', // Juster for NavMenu-bredden
                            padding: '1rem',
                        }}>
                        <Routes>
                            <Route path="/" element={<Home />} />
                            <Route path="/about" element={<About />} />
                            <Route path="/login" element={<LoginUser />} />
                            <Route path="/register" element={<Register />} />
                            <Route
                                path="/edit-post/:postId"
                                element={
                                    <PrivateRoute>
                                        <EditPost />
                                    </PrivateRoute>
                                }
                            />
                            <Route
                                path="/profile/:username"
                                element={
                                    <PrivateRoute>
                                        <Profile />
                                    </PrivateRoute>
                                }
                            />
                            <Route
                                path="/upload"
                                element={
                                    <PrivateRoute>
                                        <UploadPost />
                                    </PrivateRoute>
                                }
                            />
                            <Route
                                path="/settings"
                                element={
                                    <PrivateRoute>
                                        <UserSettings />
                                    </PrivateRoute>
                                }
                            />
                        </Routes>
                    </div>
                    <Footer />
                </div>
            </Router>
        </AuthProvider>
        </>
    );
};

export default App;
