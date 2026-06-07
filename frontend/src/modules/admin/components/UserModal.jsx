import './UserModal.css';

const UserModal = ({
  isEditMode,
  formData,
  setFormData,
  rolesList,
  onRoleChange,
  onSubmit,
  onClose
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <h3 className="modal-title">
          {isEditMode
            ? 'Редагування користувача'
            : 'Створення нового користувача'}
        </h3>

        <form onSubmit={onSubmit}>
          <div className="form-group">
            <label>Login</label>

            <input
              className="form-input"
              type="text"
              disabled={isEditMode}
              value={formData.login}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  login: e.target.value
                })
              }
              required
            />
          </div>

          <div className="form-group">
            <label>Email</label>

            <input
              className="form-input"
              type="email"
              value={formData.email}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  email: e.target.value
                })
              }
              required
            />
          </div>

          {!isEditMode && (
            <div className="form-group">
              <label>Password</label>

              <input
                className="form-input"
                type="password"
                value={formData.password}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    password: e.target.value
                  })
                }
                required
              />
            </div>
          )}

          <div className="form-group">
            <label>Name</label>

            <input
              className="form-input"
              type="text"
              value={formData.firstName}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  firstName: e.target.value
                })
              }
              required
            />
          </div>

          <div className="form-group">
            <label>Last name</label>

            <input
              className="form-input"
              type="text"
              value={formData.lastName}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  lastName: e.target.value
                })
              }
              required
            />
          </div>

          <div className="form-group">
            <label>Roles</label>

            <div className="roles-container">
              {rolesList.map(role => (
                <label
                  key={role.id}
                  className="role-item"
                >
                  <input
                    type="checkbox"
                    checked={formData.roleIds.includes(role.id)}
                    onChange={() => onRoleChange(role.id)}
                  />

                  <span>{role.name}</span>
                </label>
              ))}

              {rolesList.length === 0 && (
                <span className="roles-empty">
                  No roles found
                </span>
              )}
            </div>
          </div>

          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onClose}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="btn btn-primary"
            >
              Save
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default UserModal;