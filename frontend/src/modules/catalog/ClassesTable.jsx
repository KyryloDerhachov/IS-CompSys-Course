import React from 'react';

const ClassesTable = ({ classes, onCreate, onEdit, onToggleStatus }) => {
  return (
    <>
      <div style={{ padding: '20px 24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h3>Product Class</h3>
        <button className="btn btn-success" onClick={onCreate}>
          Create a class
        </button>
      </div>

      <table>
        <thead>
          <tr>
            <th>Code</th>
            <th>Title</th>
            <th>Description</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {classes.map(cl => (
            <tr key={cl.id} style={{ opacity: cl.isActive ? 1 : 0.6 }}>
              <td><code>{cl.code}</code></td>
              <td><b>{cl.name}</b></td>
              <td>{cl.description || <span style={{ color: 'var(--text-secondary)' }}>—</span>}</td>
              <td>
                {cl.isActive ? (
                  <span style={{ color: 'var(--success)' }}>Active</span>
                ) : (
                  <span style={{ color: 'var(--danger)' }}>Inactive</span>
                )}
              </td>
              <td>
                <button className="btn btn-small" style={{ marginRight: '8px' }} onClick={() => onEdit(cl)}>
                   Edit Description
                </button>
                <button
                  className={`btn btn-small ${cl.isActive ? 'btn-danger' : ''}`}
                  onClick={() => onToggleStatus(cl)}
                >
                  {cl.isActive ? 'Block' : 'Activate'}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

export default ClassesTable;