import React from 'react';

const CategoriesTable = ({ categories, onCreate, onEdit, onToggleStatus }) => {
  return (
    <>
      <div style={{ padding: '20px 24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h3>Categories</h3>
        <button className="btn btn-success" onClick={onCreate}>
          Create a category
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
          {categories.map(c => (
            <tr key={c.id} style={{ opacity: c.isActive ? 1 : 0.6 }}>
              <td><code>{c.code}</code></td>
              <td><b>{c.name}</b></td>
              <td>{c.description || <span style={{ color: 'var(--text-secondary)' }}>—</span>}</td>
              <td>
                {c.isActive ? (
                  <span style={{ color: 'var(--success)' }}>Active</span>
                ) : (
                  <span style={{ color: 'var(--danger)' }}>Inactive</span>
                )}
              </td>
              <td>
                <button className="btn btn-small" style={{ marginRight: '8px' }} onClick={() => onEdit(c)}>
                  Edit Description
                </button>
                <button
                  className={`btn btn-small ${c.isActive ? 'btn-danger' : ''}`}
                  onClick={() => onToggleStatus(c)}
                >
                  {c.isActive ? 'Блокувати' : 'Активувати'}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

export default CategoriesTable;