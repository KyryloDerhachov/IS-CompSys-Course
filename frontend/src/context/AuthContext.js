import React, { createContext, useState, useEffect } from 'react';
import { userService } from '../services/userService';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const storedUser = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    if (storedUser && token) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const loginAction = async (username, password) => {
    
    const response = await userService.login(username, password);
    
    
    const { token, userId, login, firstName, lastName, roles } = response.data;

    const userData = {
      id: userId,
      login: login,
      fullName: `${firstName} ${lastName}`,
      roles: roles 
    };

    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
  };

  const logoutAction = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login: loginAction, logout: logoutAction, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};