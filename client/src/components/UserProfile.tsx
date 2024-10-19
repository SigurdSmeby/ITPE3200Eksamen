import React from 'react';

const UserProfile: React.FC<{ user: any }> = ({ user }) => {
  return (
    <div className="user-profile">
      <h1>{user.username}</h1>
      <img src={user.profilePic} alt="Profile" />
      <p>{user.bio}</p>
    </div>
  );
};

export default UserProfile;
