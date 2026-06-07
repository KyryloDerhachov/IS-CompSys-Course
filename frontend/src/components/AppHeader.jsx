import React, { useContext } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

import './AppHeader.css';

const AppHeader = () => {
  const { user, logout } = useContext(AuthContext);
  const navigate = useNavigate();
  const location = useLocation();

  const isDashboard = location.pathname === '/';

  return (
    <header className="app-header">
      <div className="app-header-left">
        {!isDashboard && (
          <button
            className="header-btn"
            onClick={() => navigate('/')}
          >
            Home
          </button>
        )}

        <div>
          <h1>Store system</h1>
          <span>
            {user?.fullName} ({user?.login})
          </span>
        </div>
      </div>

      <button
        className="header-btn logout-btn"
        onClick={logout}
      >
        Log out
      </button>
    </header>
  );
};

export default AppHeader;