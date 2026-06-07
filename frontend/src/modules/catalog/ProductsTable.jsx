import React from 'react';

const ProductsTable = ({ products, onCreate, onEdit, onToggleStatus }) => {
  return (
    <>
      <div style={{ padding: '20px 24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h3>Products</h3>
        <button className="btn btn-success" onClick={onCreate}>
          Add a new product
        </button>
      </div>

      <table>
        <thead>
          <tr>
            <th>SKU / Barcode</th>
            <th>Title</th>
            <th>Class / Category</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {products.map(p => (
            <tr key={p.id} style={{ opacity: p.isActive ? 1 : 0.6 }}>
              <td>
                <div><b>SKU:</b> <code>{p.sku}</code></div>
                <div style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>
                  <b>BC:</b> {p.barcode || '—'}
                </div>
              </td>
              <td>
                <b>{p.name}</b> <span style={{ color: 'var(--text-secondary)' }}></span>
              </td>
              <td>
                <div>{p.className}</div>
                <div style={{ fontSize: '13px', color: 'var(--text-secondary)', marginTop: '4px' }}>
                  {p.categoryName}
                </div>
              </td>
              <td>
                {p.isActive ? (
                  <span style={{ color: 'var(--success)', fontWeight: '600' }}>Active</span>
                ) : (
                  <span style={{ color: 'var(--danger)', fontWeight: '600' }}>Archive</span>
                )}
              </td>
              <td>
                <button className="btn btn-small" style={{ marginRight: '8px' }} onClick={() => onEdit(p)}>
                  Edit
                </button>
                <button
                  className={`btn btn-small ${p.isActive ? 'btn-danger' : ''}`}
                  style={{ background: p.isActive ? 'var(--danger)' : '#0ea5e9' }}
                  onClick={() => onToggleStatus(p)}
                >
                  {p.isActive ? 'Block' : 'Activate'}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

export default ProductsTable;