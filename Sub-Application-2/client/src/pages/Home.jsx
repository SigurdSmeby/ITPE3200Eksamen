import React, { useState, useEffect, useRef } from 'react';
import PostCards from '../components/postCards';
import { getPosts } from '../api/postApi.js';
import { toast } from 'react-toastify';

const Home = () => {
    const [posts, setPosts] = useState([]);
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPosts, setTotalPosts] = useState(0);
    const [loading, setLoading] = useState(false);
    const loader = useRef(null);
    const postsLengthRef = useRef(posts.length);
    const notifyDeleteSuccess = () => toast.success('Post deleted successfully!');


    // constants set to avoid querying the server too much, can be fine-tuned for a non-MVP
    const REQUEST_LIMIT = 1; // Max number of requests per minute
    const LOCKOUT_TIME = 30000; // Lockout duration in milliseconds (30 seconds)
    const [lockedOut, setLockedOut] = useState(false);
    const [requestTimestamps, setRequestTimestamps] = useState([]);

    useEffect(() => {
        postsLengthRef.current = posts.length;
    }, [posts.length]);

    useEffect(() => {
        const fetchData = async () => {
            if (lockedOut) {
                // add notifyLockedOut(); if we have time
                return;
            }
            setLoading(true);
            try {
                const now = Date.now();
                const recentRequests = requestTimestamps.filter(
                    (timestamp) => now - timestamp < 60000
                );
                if (recentRequests.length >= REQUEST_LIMIT) {
                    setLockedOut(true);
                    setTimeout(() => setLockedOut(false), LOCKOUT_TIME);
                    // add notifyLockedOut(); if we have time
                    return;
                }

                const data = await getPosts(pageNumber, 10);

                if (data.posts.length === 0) {
                    // No more posts to fetch
                    setTotalPosts(posts.length); // Explicitly set totalPosts to prevent further fetches
                    return;
                }

                setPosts((prevPosts) => {
                    const newPosts = data.posts.filter(
                        (newPost) => !prevPosts.some((prevPost) => prevPost.postId === newPost.postId)
                    );
                    return [...prevPosts, ...newPosts];
                });
                setTotalPosts(data.totalPosts);
            } catch (error) {
                console.error('Error fetching posts:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [pageNumber, loading]); // Added 'loading' to the dependency array

    useEffect(() => {
        const observerOptions = {
            root: null,
            rootMargin: '0px',
            threshold: 1.0,
        };

        const observerCallback = (entries) => {
            const target = entries[0];
            if (
                target.isIntersecting &&
                postsLengthRef.current < totalPosts &&
                postsLengthRef.current !== totalPosts && // Ensure no repeat fetching
                !loading
            ) {
                setPageNumber((prevPageNumber) => prevPageNumber + 1);
            }
        };


        const loaderNode = loader.current; // Copy the mutable ref to a local variable
        const observer = new IntersectionObserver(observerCallback, observerOptions);

        if (loaderNode) observer.observe(loaderNode);

        return () => {
            if (loaderNode) observer.unobserve(loaderNode); // Use the local variable in cleanup
        };
    }, [totalPosts, loading]); // Removed 'loader.current' from dependencies

    const triggerRefresh = (deletedPostId) => {
        notifyDeleteSuccess();
        setPosts((prevPosts) => prevPosts.filter((post) => post.postId !== deletedPostId));
        setTotalPosts((prevTotal) => prevTotal - 1);
    };

    return (
        <>
            {posts.map((post) => (
                <PostCards key={post.postId} post={post} onDeleted={() => triggerRefresh(post.postId)} />
            ))}
            {loading && <p>Loading more posts...</p>}
            {!loading && posts.length >= totalPosts && totalPosts !== 0 && (
                <p>You've reached the end!</p>
            )}
            <div ref={loader}></div>
        </>
    );
};

export default Home;
