import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import NavMenu from './components/shared/navbar.tsx';
import Home from './pages/Home.tsx';
import About from './pages/About.tsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container } from 'react-bootstrap';
import Footer from './components/shared/footer.tsx';
import Login from './pages/Login.tsx';
import Register from './pages/Register.tsx';
import Profile from './pages/Profile.tsx';


const App: React.FC = () => {
  return (
    <>
    <div>
      <NavMenu />
    </div>
    <Container style={{ marginLeft: '250px'}}>
      <Router>
      <Routes>
        <Route path="/" element={<Home/>} />
        <Route path="/about" element={<About />} />
        <Route path="/login" element={<Login />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/register" element={<Register />} />
      </Routes>
    </Router>
    <Footer />
    </Container>
    
    </>
  );
};

export default App;