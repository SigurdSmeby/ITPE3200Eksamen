import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import NavMenu from './components/shared/navbar';
import Home from './pages/Home';
import About from './pages/About';
import 'bootstrap/dist/css/bootstrap.min.css';
import Footer from './components/shared/footer';
import LoginUser from './pages/Login.jsx';
import Register from './pages/Register.jsx';
import Profile from './pages/profile.jsx';
import UserSettings from './pages/UserSettings';
import PrivateRoute from './components/PrivateRoute.js';
import EditPost from './pages/EditPost.jsx';
import UploadPost from './pages/UploadPost';
import { AuthProvider } from './components/shared/AuthContext';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Search from './pages/Search';

const App = () => {
    console.log('Current environment:', process.env.NODE_ENV);
    return (
        <>
            <main>
                <ToastContainer /> {/* For displaying toast notifications */}
                <AuthProvider>
                    {' '}
                    {/* Provides authentication context to the app */}
                    <Router>
                        {' '}
                        {/* Routes for different pages*/}
                        <div
                            style={{
                                display: 'flex',
                                flexDirection: 'column',
                                minHeight: '100vh',
                            }}>
                            <NavMenu />
                            <div
                                className="Routes-body"
                                style={{
                                    flex: 1,
                                    padding: '1rem',
                                }}>
                                <Routes>
                                    <Route path="/" element={<Home />} />
                                    <Route path="/about" element={<About />} />
                                    <Route
                                        path="/login"
                                        element={<LoginUser />}
                                    />
                                    <Route
                                        path="/register"
                                        element={<Register />}
                                    />

                                    {/* Private route for some pages*/}
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
                                        path="/search"
                                        element={
                                            <PrivateRoute>
                                                <Search />
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
            </main>
        </>
    );
};

export default App;
