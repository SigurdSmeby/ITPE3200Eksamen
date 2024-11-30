import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserPosts } from '../api/postApi';
import PostCards from '../components/postCards';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const BACKEND_URL = 'http://localhost:5229';

const Profile = () => {
    const [profileData, setProfileData] = useState({
        userName: '',
        profilePicture: '',
        bio: '',
        dateJoined: '',
    });

    const [posts, setPosts] = useState([]); // Stores the list of posts
    const [pageNumber, setPageNumber] = useState(1); // Tracks the current page number
    const [totalPosts, setTotalPosts] = useState(0); // Total number of posts available
    const [loading, setLoading] = useState(false); // Indicates if data is being loaded
    const loader = useRef(null); // Reference to the loader div

    const postsLengthRef = useRef(posts.length);

    useEffect(() => {
        postsLengthRef.current = posts.length;
    }, [posts.length]);

    const [error, setError] = useState(null);

    const { username } = useParams();
    const navigate = useNavigate();

    const loggedInUsername = localStorage.getItem('username');

    const notifyDeleteSuccess = () => toast.success("Post deleted successfully!");

    // Fetch user profile data
    useEffect(() => {
        const fetchUserProfile = async () => {
            try {
                const response = await getUserPosts(username, 1, 1); // Fetch just one post to get profile info
                setProfileData({
                    userName: response.username,
                    profilePicture: response.profilePictureUrl,
                    bio: response.bio,
                    dateJoined: new Date(response.dateJoined).toLocaleDateString('en-US'),
                });
                setTotalPosts(response.totalPosts);
            } catch (err) {
                setError(err.response?.data || 'An error occurred');
            }
        };

        fetchUserProfile();
    }, [username]);

    // Fetch user posts with pagination
    useEffect(() => {
        const fetchUserPostsData = async () => {
            setLoading(true);
            try {
                const response = await getUserPosts(username, pageNumber, 10);
                console.log('Fetched posts:', response.posts.map((post) => post.postId));
                setPosts((prevPosts) => {
                    const newPosts = response.posts.filter(
                        (newPost) => !prevPosts.some((prevPost) => prevPost.postId === newPost.postId)
                    );
                    return [...prevPosts, ...newPosts];
                });
                setTotalPosts(response.totalPosts);
            } catch (err) {
                setError(err.response?.data || 'An error occurred');
            } finally {
                setLoading(false);
            }
        };

        fetchUserPostsData();
    }, [username, pageNumber]);

    // IntersectionObserver for lazy loading
    useEffect(() => {
        const observerOptions = {
            root: null,
            rootMargin: '0px',
            threshold: 1.0,
        };

        const observerCallback = (entries) => {
            const target = entries[0];
            if (target.isIntersecting && postsLengthRef.current < totalPosts && !loading) {
                console.log('Loader is in view, incrementing page number');
                setPageNumber((prevPageNumber) => prevPageNumber + 1);
            }
        };

        const observer = new IntersectionObserver(observerCallback, observerOptions);

        if (loader.current) observer.observe(loader.current);

        return () => {
            if (loader.current) observer.unobserve(loader.current);
        };
    }, [loader.current, totalPosts, loading]);

    const triggerRefresh = (deletedPostId) => {
        notifyDeleteSuccess();
        setPosts((prevPosts) => prevPosts.filter((post) => post.postId !== deletedPostId));
        setTotalPosts((prevTotal) => prevTotal - 1);
    };

    // Render the profile's hero section with user info
    const HeroSection = () => {
        const { userName, profilePicture, bio, dateJoined } = profileData;
        const numberOfPosts = totalPosts || 0;

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

    return (
        <>
            <HeroSection /> {/* Render hero section */}
            {error ? (
                <h1 className="text-danger text-center">{error}</h1>
            ) : (
                <>
                    {posts.map((post) => (
                        <PostCards
                            key={post.postId}
                            post={post}
                            onDeleted={() => triggerRefresh(post.postId)}
                        />
                    ))}
                    {loading && <p>Loading more posts...</p>}
                    {!loading && posts.length >= totalPosts && totalPosts !== 0 && (
                        <p>You've reached the end!</p>
                    )}
                    <div ref={loader}></div>
                </>
            )}
            <ToastContainer /> {/* Render toast notifications */}
        </>
    );
};

export default Profile;
