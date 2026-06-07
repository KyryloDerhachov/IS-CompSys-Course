import React from 'react';

const BatchesTab = ({ batches, onReduceClick }) => {
  return (
    <div>
      <h3>Inventory balances by batch</h3>
      <table className="stock-table">
        <thead>
          <tr>
            <th>Batch ID / Number</th>
            <th>Product (SKU)</th>
            <th>Received</th>
            <th>Balance (Quantity)</th>
            <th>Purchase price</th>
            <th>Date created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {batches.map(b => (
            <tr key={b.id}>
              <td><code>{b.id}</code></td>
              <td><b>{b.productName || 'Товар'}</b></td>
              <td>{b.quantityReceived}</td>
              <td>
                <span className={b.remainingQuantity === 0 ? 'qty-empty' : 'qty-available'}>
                  {b.remainingQuantity}
                </span>
              </td>
              <td>{b.purchasePrice} ₴</td>
              <td>{new Date(b.createdAt || b.supplyDate).toLocaleDateString()}</td>
              <td>
                <button 
                  disabled={b.remainingQuantity <= 0}
                  onClick={() => onReduceClick(b)}
                  className="btn btn-danger"
                >
                  Write off
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default BatchesTab;