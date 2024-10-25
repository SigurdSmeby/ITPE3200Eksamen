import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import NavMenu from './components/shared/navbar.tsx';
import Home from './pages/Home.jsx';
import About from './pages/About.tsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container } from 'react-bootstrap';
import Footer from './components/shared/footer.tsx';
import Login from './pages/Login.tsx';
import Register from './pages/Register.tsx';
import TestPage from './pages/testPage.jsx';
import Profile from './pages/profile.jsx';
import UserSettings from './pages/UserSettings.jsx';

const App: React.FC = () => {
    return (
        <>
            <NavMenu />
            <Router>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/about" element={<About />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/test" element={<TestPage />} />
                    <Route path="/profile/:username" element={<Profile />} />
                    <Route path="/settings" element={<UserSettings />} />
                </Routes>
            </Router>
            <Footer />
        </>
    );
};

export default App;
