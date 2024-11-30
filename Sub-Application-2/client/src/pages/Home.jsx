import React, { useState, useEffect, useRef } from 'react';
import PostCards from '../components/postCards';
import { getPosts } from '../api/postApi.js';
import { toast } from 'react-toastify';

const Home = () => {
    const [posts, setPosts] = useState([]); // Stores the list of posts
    const [pageNumber, setPageNumber] = useState(1); // Tracks the current page number
    const [totalPosts, setTotalPosts] = useState(null); // Total number of posts available
    const [loading, setLoading] = useState(false); // Indicates if data is being loaded
    const loader = useRef(null); // Reference to the loader div

    const notifyDeleteSuccess = () => toast.success("Post deleted successfully!"); // Notification for successful deletion

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const data = await getPosts(pageNumber, 10); // Fetch 10 posts per page
                setPosts((prevPosts) => [...prevPosts, ...data.posts]); // Append new posts
                setTotalPosts(data.totalPosts);
            } catch (error) {
                console.error('Error fetching posts:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [pageNumber]);

    useEffect(() => {
        const observerOptions = {
            root: null,
            rootMargin: '20px',
            threshold: 1.0,
        };

        const observerCallback = (entries) => {
            const target = entries[0];
            if (target.isIntersecting && posts.length < totalPosts && !loading) {
                setPageNumber((prevPageNumber) => prevPageNumber + 1);
            }
        };

        const observer = new IntersectionObserver(observerCallback, observerOptions);

        if (loader.current) observer.observe(loader.current);

        return () => {
            if (loader.current) observer.unobserve(loader.current);
        };
    }, [loader, posts, totalPosts, loading]);

    const triggerRefresh = () => {
        notifyDeleteSuccess();
        setPosts([]);
        setPageNumber(1);
        setTotalPosts(null);
    };

    return (
        <>
            {posts.map((post) => (
                <PostCards
                    key={post.postId}
                    post={post}
                    onDeleted={triggerRefresh}
                />
            ))}
            {loading && <p>Loading more posts...</p>}
            {!loading && posts.length >= totalPosts && <p>You've reached the end!</p>}
            <div ref={loader}></div>
        </>
    );
};

export default Home;
