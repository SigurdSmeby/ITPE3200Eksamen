import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getUserPosts } from '../api/postApi';
import PostCards from '../components/postCards';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const BACKEND_URL = 'http://localhost:5229';

const Profile = () => {
    const [profileData, setProfileData] = useState({
        userName: '',
        profilePicture: '',
        bio: '',
        dateJoined: '',
    });
    const [posts, setPosts] = useState([]);
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPosts, setTotalPosts] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { username } = useParams();
    const loggedInUsername = localStorage.getItem('username');
    const navigate = useNavigate();
    const notifyDeleteSuccess = () => toast.success("Post deleted successfully!");
    const loader = useRef(null);
    const postsLengthRef = useRef(posts.length);
    const loadingRef = useRef(false); // refrenece to track loading state

    // Update the posts length ref when the posts state changes
    useEffect(() => {
        postsLengthRef.current = posts.length;
    }, [posts.length]);

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
            if (loadingRef.current) return; // Guard against redundant requests

            loadingRef.current = true; // Update ref to reflect loading state
            setLoading(true); // Update state for UI feedback
            try {
                const response = await getUserPosts(username, pageNumber, 10);
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
                loadingRef.current = false; // Reset ref after fetch
                setLoading(false); // Update state
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

        // Callback function for the observer
        const observerCallback = (entries) => {
            const target = entries[0];
            if (target.isIntersecting && postsLengthRef.current < totalPosts && !loading) {
                console.log('Loader is in view, incrementing page number');
                setPageNumber((prevPageNumber) => prevPageNumber + 1);
            }
        };

        // Create the observer and observe the loader
        const loaderNode = loader.current;
        const observer = new IntersectionObserver(observerCallback, observerOptions);

        if (loaderNode){
            observer.observe(loaderNode);
        } 
        return () => {
            if (loaderNode) observer.unobserve(loaderNode); // Use the local variable for cleanup
        };
    }, [totalPosts, loading]);

    // Trigger a refresh when a post is deleted
    const triggerRefresh = (deletedPostId) => {
        notifyDeleteSuccess();
        setPosts((prevPosts) => prevPosts.filter((post) => post.postId !== deletedPostId));
        setTotalPosts((prevTotal) => prevTotal - 1);
    };

    // Hero section for user profile, including profile picture and bio
    const HeroSection = () => {
        const { userName, profilePicture, bio, dateJoined } = profileData;
        const numberOfPosts = totalPosts || 0;

        return (
            <div className="d-flex align-items-center m-5">
                {/* Display the user's profile picture */}
                <img
                    src={`${BACKEND_URL}${profilePicture}`}
                    alt={userName}
                    className="rounded-circle img-fluid me-5"
                    style={{ width: '150px', height: '150px', objectFit: 'cover' }}
                />
                <div>
                    {/* Display the user's name and bio */}
                    <h1>{userName}</h1>
                    <p>{bio}</p>
                    <div className="btn btn-light">
                        Number of posts: {numberOfPosts}
                    </div>
                    <div className="btn btn-light">
                        Member since: {dateJoined}
                    </div>
                    {/* Display the edit profile button for the logged-in user */}
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
            {/* Display the hero section with user profile data */}
            <HeroSection />
            {error ? (
                <h1 className="text-danger text-center">{error}</h1>
            ) : (
                <>
                    {/* Display the user's posts */}
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
        </>
    );
};

export default Profile;
