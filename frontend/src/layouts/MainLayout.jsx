import React from 'react';
import AppHeader from '../components/AppHeader';

const MainLayout = ({ children }) => {
  return (
    <>
      <AppHeader />
      {children}
    </>
  );
};

export default MainLayout;