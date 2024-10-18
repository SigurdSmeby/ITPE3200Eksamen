// Define the interfaces to match server-side model
interface BodyImage {
    imageUrl: string;  // Matches 'ImageUrl' in the server response
    uploadedAt: string;  // Matches 'UploadedAt' in the server response
  }
  
  export interface User {
    profileName: string;  // Matches 'ProfileName' in the server response
    profilePicture: string;  // Matches 'ProfilePicture' in the server response
    images: BodyImage[];  // Matches 'Images' in the server response
  }
  
  // Fetch all users
  export const fetchUsers = async (): Promise<User[]> => {
    const response = await fetch('http://localhost:5229/api/Users');
    if (!response.ok) {
      throw new Error('Failed to fetch users');
    }
    const data = await response.json();
    return data;
  };
  
  // Fetch user by ID
  export const fetchUserById = async (userId: number): Promise<User> => {
    const response = await fetch(`http://localhost:5229/api/Users/${userId}`);
    if (!response.ok) {
      throw new Error(`Failed to fetch user with ID: ${userId}`);
    }
    const data = await response.json();
    return data;
  };
  