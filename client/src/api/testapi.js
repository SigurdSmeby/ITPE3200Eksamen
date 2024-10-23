import axios from "axios";

const API_URL = "http://localhost:5229/api/Users/";

//api/Users/profile
//api/Users/register

const register = (username, email, password) => {
    console.log(API_URL + "register");
  return axios.post(API_URL + "register", {
    username,
    email,
    password,
  }).catch((error) => {
    console.error("Error registering user:", error);
    throw error;  // Re-throw to handle in the calling function if needed
  });
};

// Get user profile (or any other protected route)
const getUsers = () => {
  const token = localStorage.getItem('jwtToken');  // Retrieve the token each time the request is made
  console.log(token);  // For debugging, to see the token being used
  return axios.get(API_URL + "profile", {
    headers: {
      Authorization: `Bearer ${token}`,  // Include token in the Authorization header
    }
  }).catch((error) => {
    console.error("Error fetching user profile:", error);
    throw error;  // Re-throw to handle in the calling function if needed
  });
};

const TestApi = {
    register,
    getUsers,
    };

export default TestApi;