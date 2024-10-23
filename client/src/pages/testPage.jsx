import React, { useState } from 'react';
import TestApi from '../api/testapi';;

const TestPage = () => {

  const handleGetUser = () => {
    console.log("click");
    TestApi.getUsers()
    .then((response) => {
      console.log(response.data);
    });
  }

  const handleRegUser = () => {
    console.log("click");
    TestApi.register("testName", "test@exsample.com", "testPassword").then((response) => {
      console.log(response.data);
    });
  }

  const handleGetPosts = () => {
    console.log("click");
    TestApi.getPosts().then((response) => {
      console.log(response.data);
    });
  }

  const handleLogout = () => {
    console.log("logged out and removed token");
    localStorage.removeItem("jwtToken");
  }

  const [title, setTitle] = useState();
  const [imageUrl, setImageUrl] = useState();

  const handleCreatePost = () => {
    console.log("click");
    console.log(title);
    console.log(imageUrl);
    
    TestApi.createPost(title, imageUrl).then((response) => {
      console.log(response.data);
    });
  }

  return (
    <>
    <div>
      <button onClick={handleGetUser} >getUser</button>
      <button onClick={handleRegUser} >send userdata</button>
      <button onClick={handleGetPosts} >GetPosts</button>
      <a href="/login">login</a>
      <button onClick={handleLogout}>logout</button>
    </div>
    <div>
      <input type="text" placeholder="title" onChange={(e) => setTitle(e.target.value)}/>
      <input type="text" placeholder="imgUrl" onChange={(e) => setImageUrl(e.target.value)}/>
      <button onClick={()=> {setImageUrl("https://picsum.photos/600"); console.log("picsum is set");}}>setpicksum</button>
      <button onClick={handleCreatePost}>submit</button>

    </div>
    
    </>
    )
}

export default TestPage;