import React from 'react';

function Dashboard({ user }) {
  return (
    <div>
      <h2>Welcome to the Dashboard!</h2>
      {user && <p>Hello, {user.email}!</p>}
      <p>Your company is registered.</p>
      {/* Add your main application content here */}
    </div>
  );
}

export default Dashboard; 