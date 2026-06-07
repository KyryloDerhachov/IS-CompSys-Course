import './UsersTable.css';

const UsersTable = ({
  users,
  onEdit,
  onDeactivate
}) => {
  return (
    <table className="users-table">
      <thead>
        <tr>
          <th>Login</th>
          <th>First and last name</th>
          <th>Email</th>
          <th>Roles</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>

      <tbody>
        {users.map(user => (
          <tr
            key={user.id}
            className={!user.isActive ? 'user-inactive' : ''}
          >
            <td className="user-login">
              {user.login || user.username}
            </td>

            <td>
              {user.firstName} {user.lastName}
            </td>

            <td>
              {user.email || (
                <span className="user-empty">
                  none
                </span>
              )}
            </td>

            <td>
              <div className="roles-list">
                {user.roles?.map(role => (
                  <span
                    key={role}
                    className="role-badge"
                  >
                    {role}
                  </span>
                ))}
              </div>
            </td>

            <td>
              {user.isActive ? (
                <span className="status-active">
                  Active
                </span>
              ) : (
                <span className="status-inactive">
                  Deactivated
                </span>
              )}
            </td>

            <td>
              <div className="table-actions">
                <button
                  className="btn btn-edit"
                  onClick={() => onEdit(user)}
                >
                  Edit
                </button>

                {user.isActive && (
                  <button
                    className="btn btn-danger"
                    onClick={() => onDeactivate(user.id)}
                  >
                    Deactivate
                  </button>
                )}
              </div>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default UsersTable;