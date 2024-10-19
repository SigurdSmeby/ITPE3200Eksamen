// Define the Image interface (modify as needed based on your data structure)
interface ImageData {
    url: string;
    date: string;
    userName: string;
    profileImage: string;
  }
  
  // Utility function to flatten and sort images by date and time
  export const flattenAndSortImages = (users: any[]): ImageData[] => {
    // Flatten the bodyImages and attach user details
    const allImages = users.flatMap((user) =>
      user.images.map((bodyImage) => ({
        url: bodyImage.imageUrl,  // Matches 'imageUrl' from the API response
        date: bodyImage.uploadedAt,  // Matches 'uploadedAt' from the API response
        userName: user.profileName,  // Matches 'profileName' from the API response
        profileImage: user.profilePicture,  // Matches 'profilePicture' from the API response
      }))
    );
  
    // Sort the images by both date and time (using .getTime() to compare timestamps)
    return allImages
      .filter((image) => image.url)
      .sort((b, a) => new Date(a.date).getTime() - new Date(b.date).getTime());
  };
  