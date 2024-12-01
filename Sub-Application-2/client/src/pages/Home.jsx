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
    const loadingRef = useRef(false); // refrenece to track loading state

    
    const notifyDeleteSuccess = () => toast.success('Post deleted successfully!');
    
    
    useEffect(() => {
        postsLengthRef.current = posts.length;
    }, [posts.length]);

    useEffect(() => {
        const fetchData = async () => {
            if (loadingRef.current) return; // Guard against redundant requests

            loadingRef.current = true; // Update ref to reflect loading state
            setLoading(true); // Update state for UI feedback
            try {

                const data = await getPosts(pageNumber, 10);
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
                loadingRef.current = false;
            }
        };

        fetchData();
    }, [pageNumber]);

    useEffect(() => {
        const observerOptions = {
            root: null,
            rootMargin: '0px',
            threshold: 1.0,
        };

        const observerCallback = (entries) => {
            const target = entries[0];
            if (target.isIntersecting && postsLengthRef.current < totalPosts && !loading) {
                setPageNumber((prevPageNumber) => prevPageNumber + 1);
            }
        };

        const loaderNode = loader.current; // Copy the mutable ref to a local variable
        const observer = new IntersectionObserver(observerCallback, observerOptions);

        if (loaderNode) observer.observe(loaderNode);

        return () => {
            if (loaderNode) observer.unobserve(loaderNode); // Use the local variable in cleanup
        };
    }, [totalPosts, loading]); 

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
