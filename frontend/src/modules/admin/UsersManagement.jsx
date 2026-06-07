import React, { useState, useEffect } from 'react';
import { userService } from '../../services/userService';

import UsersTable from './components/UsersTable';
import UserModal from './components/UserModal';

import './UsersManagement.css';

const UsersManagement = () => {
  const [users, setUsers] = useState([]);
  const [rolesList, setRolesList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState(null);

  const [formData, setFormData] = useState({
    login: '',
    email: '',
    firstName: '',
    lastName: '',
    password: '',
    roleIds: []
  });

  const loadInitialData = async () => {
    try {
      setLoading(true);

      const [usersResponse, rolesResponse] = await Promise.all([
        userService.getAllUsers(true),
        userService.getAvailableRoles()
      ]);

      setUsers(usersResponse.data);
      setRolesList(rolesResponse.data);
      setError('');
    } catch {
      setError('Unable to retrieve data from the server');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadInitialData();
  }, []);

  const handleOpenCreateModal = () => {
    setIsEditMode(false);
    setSelectedUserId(null);

    setFormData({
      login: '',
      email: '',
      firstName: '',
      lastName: '',
      password: '',
      roleIds: []
    });

    setIsModalOpen(true);
  };

  const handleOpenEditModal = (user) => {
    setIsEditMode(true);
    setSelectedUserId(user.id);

    const userRoleIds = user.userRoles
      ? user.userRoles.map(r => r.roleId)
      : rolesList
          .filter(r => user.roles?.includes(r.name))
          .map(r => r.id);

    setFormData({
      login: user.login || user.username,
      email: user.email || '',
      firstName: user.firstName,
      lastName: user.lastName,
      password: '',
      roleIds: userRoleIds
    });

    setIsModalOpen(true);
  };

  const handleRoleCheckboxChange = (roleId) => {
    setFormData(prev => ({
      ...prev,
      roleIds: prev.roleIds.includes(roleId)
        ? prev.roleIds.filter(id => id !== roleId)
        : [...prev.roleIds, roleId]
    }));
  };

  const handleDeactivate = async (id) => {
    if (!window.confirm('Are you sure you want to deactivate this user?')) {
      return;
    }

    try {
      await userService.deactivateUser(id);
      alert('The user has been successfully deactivated');
      loadInitialData();
    } catch (err) {
      alert(err.response?.data?.detail || 'Deactivation error');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      if (isEditMode) {
        
        await userService.updateUserProfile(selectedUserId, {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email
        });
      
        await userService.updateUserRoles(
          selectedUserId,
          formData.roleIds
        );

        alert('Your user data has been successfully updated');
      } else {
        await userService.createUser({
          login: formData.login,
          email: formData.email,
          firstName: formData.firstName,
          lastName: formData.lastName,
          password: formData.password,
          roleIds: formData.roleIds
        });

        alert('The user has been successfully created');
      }

      setIsModalOpen(false);
      loadInitialData();
    } catch (err) {
      alert(
        err.response?.data?.detail ||
        'An error occurred while saving the user'
      );
    }
  };

  return (
    <div className="users-management">
      <div className="users-header">
        <h2 className="users-title">
          Administrator Panel: User Management
        </h2>

        <button
          className="btn btn-success"
          onClick={handleOpenCreateModal}
        >
          Create a user
        </button>
      </div>

      {error && (
        <div className="users-error">
          {error}
        </div>
      )}

      <div className="users-card">
        {loading ? (
          <div className="users-loading">
            Loading data...
          </div>
        ) : (
          <UsersTable
            users={users}
            onEdit={handleOpenEditModal}
            onDeactivate={handleDeactivate}
          />
        )}
      </div>

      {isModalOpen && (
        <UserModal
          isEditMode={isEditMode}
          formData={formData}
          setFormData={setFormData}
          rolesList={rolesList}
          onRoleChange={handleRoleCheckboxChange}
          onSubmit={handleSubmit}
          onClose={() => setIsModalOpen(false)}
        />
      )}
    </div>
  );
};

export default UsersManagement;