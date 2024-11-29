import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserPosts } from '../api/postApi';
import PostCards from '../components/postCards';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const BACKEND_URL = 'http://localhost:5229';

const Profile = () => {
    // State to store profile data and posts
    const [profileData, setProfileData] = useState({
        userName: '',
        profilePicture: '',
        bio: '',
        dateJoined: '',
        posts: [],
    });

    // State for handling errors and triggering refresh
    const [error, setError] = useState(null);
    const [refresh, setRefresh] = useState(false);

    // Get username from URL parameters and setup navigation
    const { username } = useParams();
    const navigate = useNavigate();

    // Get the logged-in user's username from localStorage
    const loggedInUsername = localStorage.getItem('username');

    // Toast notification for post deletion
    const notifyDeleteSucsess = () => toast.success("Post deleted successfully!");

    // Fetch user posts and profile data when component mounts or refresh triggers
    useEffect(() => {
        const fetchUserPosts = async () => {
            try {
                const response = await getUserPosts(username);
                // Update profile data with API response
                setProfileData({
                    userName: response.username,
                    profilePicture: response.profilePictureUrl,
                    bio: response.bio,
                    dateJoined: new Date(response.dateJoined).toLocaleDateString('en-US'),
                    posts: response.posts,
                });
            } catch (err) {
                // Set error message if API call fails
                setError(err.response?.data || 'An error occurred');
            }
        };

        fetchUserPosts();
    }, [refresh, username]);

    // Trigger a refresh to reload posts after deletion
    const triggerRefresh = () => {
        notifyDeleteSucsess();
        setRefresh(!refresh);
    };

    // Render the profile's hero section with user info
    const HeroSection = () => {
        const { userName, profilePicture, bio, dateJoined, posts } = profileData;
        const numberOfPosts = posts.length;

        return (
            <div className="d-flex align-items-center m-5">
                <img
                    src={`${BACKEND_URL}${profilePicture}`}
                    alt={userName}
                    className="rounded-circle img-fluid me-5"
                    style={{ width: '150px', height: '150px', objectFit: 'cover' }}
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
                    {loggedInUsername === username && (
                        <button
                            className="btn btn-primary ms-2"
                            onClick={() => navigate('/settings')}>
                            Edit Profile
                        </button>
                    )}
                </div>
            </div>
        );
    };

    // Render the user's posts or appropriate message
    const ImgSection = () => {
        if (error) {
            return <h1 className="text-danger text-center">{error}</h1>; // Show error if present
        }
        if (profileData.posts.length === 0) {
            return <h1>The user has not posted any images yet</h1>; // Show message if no posts
        }

        return (
            <div>
                {profileData.posts
                    .sort((a, b) => new Date(b.dateUploaded) - new Date(a.dateUploaded)) // Sort posts by date
                    .map((post) => (
                        <PostCards
                            key={post.postId}
                            postId={post.postId}
                            imagePath={post.imagePath}
                            textContent={post.textContent}
                            dateUploaded={post.dateUploaded}
                            author={post.author}
                            likesCount={post.likesCount}
                            commentsCount={post.commentsCount}
                            fontSize={post.fontSize}
                            textColor={post.textColor}
                            backgroundColor={post.backgroundColor}
                            onDeleted={triggerRefresh} // Refresh posts after deletion
                        />
                    ))}
            </div>
        );
    };

    return (
        <>
            <HeroSection /> {/* Render hero section */}
            <ImgSection /> {/* Render image posts section */}
            <ToastContainer /> {/* Render toast notifications */}
        </>
    );
};

export default Profile;
