import React from 'react';
import PostCards from '../components/postCards.tsx';
import TestApi from '../api/testapi.js';
import { getPosts } from '../api/postApi.js';

const Home = () => {
    const [posts, setPosts] = React.useState([]); // State to hold posts data
    const [refresh, setRefresh] = React.useState(false); // State to trigger a re-fetch

    React.useEffect(() => {
        getPosts().then((response) => {
            setPosts(response); // Set the posts state with API data
            console.log(response);
        });
    }, [refresh]); // Empty dependency array to run effect only once on mount

    // Function to trigger a refresh after deletion
    const triggerRefresh = () => {
        setRefresh(!refresh); // Toggle refresh state to re-trigger useEffect
    };

    return (
        <>
            <h1>Home</h1>
            {posts
                .sort(
                    (a, b) =>
                        // sortering på nyeste først
                        new Date(b.dateUploaded) - new Date(a.dateUploaded),
                )
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
                        onDeleted={triggerRefresh}
                    />
                ))}
        </>
    );
};

export default Home;