import React, { useState } from 'react';
import TestApi from '../api/testapi';;

const TestPage = () => {

  const handleClick = () => {
    console.log("click");
    TestApi.getUsers()
    .then((response) => {
      console.log(response.data);
    });
  }

  const handleClick2 = () => {
    console.log("click");
    TestApi.register("testName", "test@exsample.com", "testPassword").then((response) => {
      console.log(response.data);
    });
  }
  return (
    <>
    <button onClick={handleClick} >getUser</button>
    <button onClick={handleClick2} >send userdata</button>
    <a href="/login">login</a>
    </>
    )
}

export default TestPage;