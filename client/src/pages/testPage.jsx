import React, { useState, useEffect } from "react";
import TestApi from "../api/testapi";
import PostCards from "../components/postCards.tsx"; // Import the PostCards component

const TestPage = () => {
	const [title, setTitle] = useState("");
	const [imageUrl, setImageUrl] = useState("");
	const [textContent, setTextContent] = useState(""); // State to hold text content
	const [isImagePost, setIsImagePost] = useState(true); // Toggle between image and text post
	const [textColor, setTextColor] = useState("#000000"); // Default text color
	const [backgroundColor, setBackgroundColor] = useState("#FFFFFF"); // Default background color
	const [fontSize, setFontSize] = useState(16); // Default font size
	const [posts, setPosts] = useState([]); // State to hold posts data
	const [profilePicture, setProfilePicture] = useState(""); // State to hold profile picture URL

	// Fetch posts on component load
	useEffect(() => {
		TestApi.getPosts().then((response) => {
			setPosts(response.data); // Set the posts state with API data
			console.log(response.data);
		});
	}, []); // Empty dependency array to run effect only once on mount

	const handleGetUser = () => {
		console.log("click");
		TestApi.getUsers().then((response) => {
			console.log(response.data);
		});
	};

	const handleRegUser = () => {
		console.log("click");
		TestApi.register("testName", "test@exsample.com", "testPassword")
			.then((response) => {
				console.log(response.data);
			})
			.catch((error) => {
				console.log(error);
			});
	};

	const handleGetPosts = () => {
		console.log("click");
		TestApi.getPosts().then((response) => {
			console.log(response.data);
			setPosts(response.data);
		});
	};

	const handleLogout = () => {
		console.log("logged out and removed token");
		localStorage.removeItem("jwtToken");
	};

	const handleCreatePost = () => {
		const post = isImagePost
			? { title, imageUrl } // For image posts
			: { title, textContent, fontSize, textColor, backgroundColor }; // For text posts
	
		TestApi.createPost(post)
			.then((response) => {
				console.log(response.data);
			})
			.catch((error) => {
				console.error("Error creating post:", error.response?.data || error.message);
			});
	};
	
	

	return (
		<>
			<div>
				<button onClick={handleGetUser}>getUser</button>
				<button onClick={handleRegUser}>send userdata</button>
				<button onClick={handleGetPosts}>GetPosts</button>
				<a href="/login">login</a>
				<button onClick={handleLogout}>logout</button>
			</div>

			{/* Toggle between creating an image post or a text post */}
			<div>
				<h4>Create a Post</h4>
				<label>
					<input
						type="radio"
						name="postType"
						value="image"
						checked={isImagePost}
						onChange={() => setIsImagePost(true)}
					/>{" "}
					Image Post
				</label>
				<label>
					<input
						type="radio"
						name="postType"
						value="text"
						checked={!isImagePost}
						onChange={() => setIsImagePost(false)}
					/>{" "}
					Text Post
				</label>

				<input
					type="text"
					placeholder="Title"
					onChange={(e) => setTitle(e.target.value)}
				/>

				{/* Conditionally render image URL or text content input based on post type */}
				{isImagePost ? (
					<input
						type="text"
						placeholder="Image URL"
						onChange={(e) => setImageUrl(e.target.value)}
					/>
				) : (
					<div>
						<textarea
							placeholder="Text Content"
							onChange={(e) => setTextContent(e.target.value)}
						/>
						<div>
							<label>Font Size:</label>
							<input
								type="number"
								min="10"
								max="72"
								value={fontSize}
								onChange={(e) => setFontSize(Number(e.target.value))}
							/>
						</div>
						<div>
							<label>Text Color:</label>
							<input
								type="color"
								value={textColor}
								onChange={(e) => setTextColor(e.target.value)}
							/>
						</div>
						<div>
							<label>Background Color:</label>
							<input
								type="color"
								value={backgroundColor}
								onChange={(e) => setBackgroundColor(e.target.value)}
							/>
						</div>
					</div>
				)}
				<button onClick={handleCreatePost}>Submit</button>
			</div>

			<div>
				<input
					type="text"
					onChange={(e) => setProfilePicture(e.target.value)}
					placeholder="Profile Picture URL"
				/>
				<button
					onClick={() => {
						TestApi.updateProfilePicture(profilePicture);
					}}
				>
					Change Profile Picture
				</button>
			</div>
			<h4>Picture Site:</h4>
			<p>https://picsum.photos/seed/[number]/300/400</p>
			<p>
				Seed controls the image, and the final part controls size. A
				single number creates a square.
			</p>

			{/* Render the post cards at the bottom of the page */}
			<div style={{ marginTop: "2rem" }}>
				{posts.map((post) => (
					<PostCards
						key={post.postId}
						postId={post.postId}
						imageUrl={post.imageUrl}
						textContent={post.textContent}
						title={post.title}
						dateUploaded={post.dateUploaded}
						author={post.author}
						likesCount={post.likesCount}
						fontSize={post.fontSize}
						textColor={post.textColor}
						backgroundColor={post.backgroundColor}
					/>
				))}
			</div>
		</>
	);
};

export default TestPage;
