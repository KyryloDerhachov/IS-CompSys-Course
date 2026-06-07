import React, { useContext } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import AppHeader from './AppHeader';

const ProtectedRoute = ({ allowedRoles }) => {
  const { user } = useContext(AuthContext);

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.some(role => user.roles?.includes(role))) {

    return <Navigate to="/unauthorized" replace />;
  }


  return  <>
    <AppHeader />
    <Outlet />
  </>;
};

export default ProtectedRoute;