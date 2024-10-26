// App.tsx
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import NavMenu from './components/shared/navbar.tsx';
import Home from './pages/Home';
import About from './pages/About.tsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import Footer from './components/shared/footer.tsx';
import LoginUser from './pages/Login.tsx';
import Register from './pages/Register.tsx';
import TestPage from './pages/testPage';
import Profile from './pages/profile';
import UserSettings from './pages/UserSettings';
import PrivateRoute from './components/PrivateRoute';
import EditPost from './pages/EditPost';
import { AuthProvider } from './components/shared/AuthContext.tsx'; // Import AuthProvider

const App: React.FC = () => {
    return (
        <AuthProvider>
            <Router>
                <NavMenu />
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/about" element={<About />} />
                    <Route path="/login" element={<LoginUser />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/test" element={<TestPage />} />
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
                        path="/settings"
                        element={
                            <PrivateRoute>
                                <UserSettings />
                            </PrivateRoute>
                        }
                    />
                </Routes>
                <Footer />
            </Router>
        </AuthProvider>
    );
};

export default App;
