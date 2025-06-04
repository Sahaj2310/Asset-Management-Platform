import React, { useState, useEffect } from 'react';
import RegistrationForm from './RegistrationForm';
import LoginForm from './LoginForm';
import CompanyRegistrationForm from './CompanyRegistrationForm';
import Dashboard from './Dashboard'; // Your main application content

function App() {
  const [user, setUser] = useState(null); // Store user info and token
  const [loadingUser, setLoadingUser] = useState(true); // To check login status initially
  const [currentView, setCurrentView] = useState('login'); // 'login', 'register', 'company-register', 'dashboard'

  useEffect(() => {
    // Check if user is already logged in (e.g., by checking local storage for a token)
    const token = localStorage.getItem('jwtToken');
    const hasCompany = localStorage.getItem('hasCompany') === 'true';

    if (token) {
      // In a real app, you might want to validate the token with the backend
      // and fetch fresh user data, including the actual hasCompany status.
      setUser({ token, hasCompany });
       if (!hasCompany) {
         setCurrentView('company-register');
       } else {
         setCurrentView('dashboard');
       }
    } else {
       setCurrentView('login');
    }
     setLoadingUser(false);
  }, []);

  const handleLoginSuccess = (loginResponse) => {
    localStorage.setItem('jwtToken', loginResponse.token);
    localStorage.setItem('hasCompany', loginResponse.hasCompany);
    setUser({ token: loginResponse.token, hasCompany: loginResponse.hasCompany });

     if (!loginResponse.hasCompany) {
       setCurrentView('company-register');
     } else {
       setCurrentView('dashboard');
     }
  };

   const handleRegistrationSuccess = () => {
       // After successful registration, redirect to login
       setCurrentView('login');
   };

   const handleCompanyRegistrationSuccess = (companyResponse) => {
       // After successful company registration, update user state and go to dashboard
       localStorage.setItem('hasCompany', 'true');
       setUser(prevUser => ({ ...prevUser, hasCompany: true }));
       setCurrentView('dashboard');
        // Optionally store companyResponse data if needed
   };

   const handleNavigateToRegister = () => {
       setCurrentView('register');
   };

   const handleNavigateToLogin = () => {
       setCurrentView('login');
   };


  if (loadingUser) {
      return <div>Loading application...</div>; // Or a loading spinner
  }

  // Render the appropriate component based on the currentView state
  switch (currentView) {
      case 'login':
          return <LoginForm onLoginSuccess={handleLoginSuccess} onNavigateToRegister={handleNavigateToRegister} />;
      case 'register':
          return <RegistrationForm onRegistrationSuccess={handleRegistrationSuccess} />;
      case 'company-register':
           // Pass the logged-in user's ID to the company registration form if needed,
           // though the form component itself will likely get it from the token.
          return <CompanyRegistrationForm onRegistrationSuccess={handleCompanyRegistrationSuccess} />;
      case 'dashboard':
          // Pass user data to the dashboard
          return <Dashboard user={user} />;
      default:
          return <div>Something went wrong.</div>; // Fallback
  }
}

export default App; 