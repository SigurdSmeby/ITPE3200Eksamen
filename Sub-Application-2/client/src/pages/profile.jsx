import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserPosts } from '../api/postApi';
import PostCards from '../components/postCards';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const BACKEND_URL = 'http://localhost:5229';

const Profile = () => {
    const [posts, setPosts] = useState([]);
    const [error, setError] = useState(null);
    const [userName, setUserName] = useState('');
    const [profilePicture, setProfilePicture] = useState('');
    const [bio, setBio] = useState('');
    const [refresh, setRefresh] = React.useState(false);
    const [dateJoined, setDateJoined] = React.useState('');

    const { username } = useParams(); // Get the username from the URL
    const navigate = useNavigate(); // Use navigate for redirection
    const loggedInUsername = localStorage.getItem('username'); // Get logged-in user's username
    const notifyDeleteSucsess = () => toast.success("Post deleted successfully!");

    useEffect(() => {
        const fetchUserPosts = async () => {
            try {
                const response = await getUserPosts(username);
                setPosts(response.posts);
                setUserName(response.username);
                setProfilePicture(response.profilePictureUrl);
                setBio(response.bio);
                setDateJoined(
                    new Date(response.dateJoined)
                        .toLocaleString('en-US')
                        .split(',')[0],
                );
            } catch (err) {
                setError(err.response.data);
            }
        };

        fetchUserPosts(); // Call the async function
    }, [refresh, username]);

    const triggerRefresh = () => {
        notifyDeleteSucsess();
        setRefresh(!refresh);
    };

    const HeroSection = () => {
        const numberOfPosts = posts.length;

        return (
            <div className="hero-section d-flex m-5">
                <img
                    src={BACKEND_URL + profilePicture}
                    alt={userName}
                    className="img-fluid me-5"
                    style={{
                        width: '150px',
                        height: '150px',
                        borderRadius: '50%',
                        objectFit: 'cover',
                    }}
                />

                <div>
                    <h1>{userName}</h1>
                    <p>{bio}</p>
                    <div className="btn btn-light">
                        Number of posts: {numberOfPosts}
                    </div>
                    <div className="btn btn-light">
                        Member since: {dateJoined}
                    </div>

                    {/* Conditionally render the Edit Profile button if this is the logged-in user's profile */}
                    {loggedInUsername === username && (
                        <div
                            className="btn btn-primary ms-2"
                            onClick={() => navigate('/settings')}>
                            Edit Profile
                        </div>
                    )}
                </div>
            </div>
        );
    };


    const ImgSection = () => {
        if (error) {
            return <h1>{error}</h1>;
        }
        if (posts.length === 0) {
            return <h1>The user has not posted any images yet</h1>;
        }
        return (
            <div>
                {posts
                    .sort((a, b) => new Date(b.dateUploaded) - new Date(a.dateUploaded))
                    .map((post) => (
                        <PostCards
                            key={post.postId}
                            post={post} // Pass the entire post object
                            onDeleted={triggerRefresh} // Handle delete action
                        />
                    ))}
            </div>
        );
    };

    return (
        <>
            <HeroSection />
            <ImgSection />
        </>
    );
};

export default Profile;
