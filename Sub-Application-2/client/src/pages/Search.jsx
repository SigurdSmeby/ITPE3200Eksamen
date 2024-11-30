import React, { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import { Link } from 'react-router-dom';
import { getAllUsers } from '../api/postApi.js';

const BACKEND_URL = 'http://localhost:5229';

const Search = () => {
    const [filterUsername, setFilterUsername] = useState(''); // State for the search input
    const [loading, setLoading] = useState(false); // Indicates if users are being loaded
    const [users, setUsers] = useState([]); // List of all users
    const [showDropdown, setShowDropdown] = useState(false); // Controls visibility of autocomplete dropdown

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const usersData = await getAllUsers(); // Fetch all users
                setUsers(usersData);
            } catch (error) {
                console.error('Error fetching users:', error);
                toast.error('Failed to load users.');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    // Filter users dynamically based on the search input
    const filteredUsers = users.filter((user) =>
        filterUsername.trim() === ''
            ? false
            : user.username?.toLowerCase().includes(filterUsername.toLowerCase())
    );

    const handleBlur = () => {
        setTimeout(() => setShowDropdown(false), 200); // Delay to allow click events to register
    };

    const handleFocus = () => {
        setShowDropdown(true); // Show dropdown when input is focused
    };

    return (
        <div className="container py-5">
            <div className="d-flex flex-column align-items-center mb-4 position-relative">
                <h1 className="text-center mb-3">Search Profiles</h1>
                <input
                    type="text"
                    placeholder="Search by username"
                    value={filterUsername}
                    onChange={(e) => setFilterUsername(e.target.value)}
                    onFocus={handleFocus}
                    onBlur={handleBlur}
                    className="form-control"
                    style={{
                        maxWidth: '400px',
                        textAlign: 'center',
                        boxShadow: '0px 2px 4px rgba(0, 0, 0, 0.1)',
                    }}
                />

                {/* Autocomplete Dropdown */}
                {filterUsername && showDropdown && (
                    <div
                        className="autocomplete-container"
                        style={{
                            position: 'absolute',
                            top: '100%',
                            left: '50%',
                            transform: 'translateX(-50%)',
                            zIndex: 10,
                            maxWidth: '400px',
                            width: '100%',
                            background: '#fff',
                            border: '1px solid #ccc',
                            borderRadius: '5px',
                            boxShadow: '0px 2px 8px rgba(0, 0, 0, 0.2)',
                            marginTop: '5px',
                        }}
                    >
                        {filteredUsers.map((user) => (
                            <div
                                key={user.username}
                                className="autocomplete-item"
                                style={{
                                    padding: '10px',
                                    cursor: 'pointer',
                                    borderBottom: '1px solid #eee',
                                }}
                                onClick={() => setFilterUsername(user.username)} // Fill input with selected username
                                onMouseDown={(e) => e.preventDefault()} // Prevent blur on click
                            >
                                {user.username}
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {loading && <p className="text-center">Loading profiles...</p>}
            {!loading && filteredUsers.length === 0 && filterUsername.trim() !== '' && (
                <p className="text-center text-muted">No profiles found for "{filterUsername}".</p>
            )}

			{/* Render Profile Cards */}
			<div className="row">
				{filteredUsers.map((user) => (
					<div key={user.username} className="col-md-4 mb-4 d-flex justify-content-center">
						<div
							className="card"
							style={{
								width: '18rem',
								borderRadius: '10px',
								boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)', // Faint drop shadow
								overflow: 'hidden', // Ensure content stays within rounded corners
							}}
						>
							<Link to={`/profile/${user.username}`}>
								<img
									src={BACKEND_URL + user.profilePictureUrl}
									alt="Profile image"
									className="card-img-top"
									style={{
										color: 'black',
										width: '100%',
										height: '200px',
										objectFit: 'cover',
									}}
								/>
							</Link>
							<div className="card-body">
								<h5
									className="card-title"
									style={{
										padding: '10px', // Padding around the username
									}}
								>
									<Link
										to={`/profile/${user.username}`}
										className="text-decoration-none"
										style={{
											color: 'black', // Black text color
											fontWeight: 'bold', // Bold font for emphasis
											textDecoration: 'none', // No underline
										}}
									>
										{user.username}
									</Link>
								</h5>
							</div>
						</div>
					</div>
				))}
			</div>

        </div>
    );
};

export default Search;