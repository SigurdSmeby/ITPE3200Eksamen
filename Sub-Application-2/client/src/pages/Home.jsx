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

// Update the posts length ref when the posts state changes
useEffect(() => {
    postsLengthRef.current = posts.length;
}, [posts.length]);


// Fetch posts when the page number changes
useEffect(() => {
    const fetchData = async () => {
        setLoading(true);
        try {
            console.log('Fetching page:', pageNumber);
            const data = await getPosts(pageNumber, 10); // Fetch 10 posts per page
            console.log('Received posts:', data.posts.map((post) => post.postId));
            console.log('Total posts:', data.totalPosts);

            setPosts((prevPosts) => {
                const newPosts = data.posts.filter((newPost) => !prevPosts.some((prevPost) => prevPost.postId === newPost.postId));
                return [...prevPosts, ...newPosts];
            });

            setTotalPosts(data.totalPosts);
            console.log('Updated posts length:', postsLengthRef.current);
        } catch (error) {
            console.error('Error fetching posts:', error);
        } finally {
        setLoading(false);
            console.log('Loading state:', loading);
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
    console.log('Loader is in view, incrementing page number');
    setPageNumber((prevPageNumber) => prevPageNumber + 1);
    }
};

const observer = new IntersectionObserver(observerCallback, observerOptions);

if (loader.current) observer.observe(loader.current);

return () => {
    if (loader.current) observer.unobserve(loader.current);
};
}, [loader.current, totalPosts, loading]); // Remove posts from dependencies

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