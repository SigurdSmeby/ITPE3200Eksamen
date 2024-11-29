import { useState, useEffect } from 'react';
import PostCards from '../components/postCards';
import { getPosts } from '../api/postApi.js';
import { toast } from 'react-toastify';

const Home = () => {
    const [posts, setPosts] = useState([]); // Stores the list of posts
    const [refresh, setRefresh] = useState(false); // Tracks when to reload posts

    const notifyDeleteSucsess = () => toast.success("Post deleted successfully!"); // Notification for successful deletion

    useEffect(() => {
        getPosts().then((response) => {
            setPosts(response); // Load posts from API
        });
    }, [refresh]); // Re-fetch posts when `refresh` changes

    const triggerRefresh = () => {
        notifyDeleteSucsess();
        setRefresh(!refresh); // Toggle `refresh` to reload posts
    };

    return (
        <>
            {// Loop through the posts and render PostCards component
            posts
                .sort((a, b) => new Date(b.dateUploaded) - new Date(a.dateUploaded)) // Sort by newest first
                .map((post) => (
                    <PostCards
                        key={post.postId}
                        post={post} // Pass the post object to the PostCards component
                        onDeleted={triggerRefresh} // Refresh posts after deletion
                    />
                ))}
        </>
    );
};

export default Home;
