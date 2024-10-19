import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom'; // Import the useParams hook
import UserProfile from '../components/UserProfile.tsx';
import { getUserProfile } from '../services/api.ts';

const Profile: React.FC = () => {
  const { id } = useParams<{ id: string }>(); // useParams gives you the route parameters
  const [user, setUser] = useState<any | null>(null);

  useEffect(() => {
    if (id) {
      getUserProfile(Number(id)).then(data => setUser(data));
    }
  }, [id]);

  if (!user) return <p>Loading...</p>;

  return <UserProfile user={user} />;
};

export default Profile;
