import React from 'react';

const ExpiringTab = ({
  expiringBatches,
  daysThreshold,
  onThresholdChange,
  onReduceClick
}) => {
  return (
    <div>
      <div
        style={{
          display: 'flex',
          gap: '15px',
          alignItems: 'center',
          marginBottom: '15px'
        }}
      >
        <h3>Batches with a critical expiration date</h3>

        <label>
          Warning threshold (days):
          <input
            type="number"
            value={daysThreshold}
            onChange={e => onThresholdChange(e.target.value)}
            className="form-control"
            style={{
              width: '70px',
              display: 'inline-block',
              marginLeft: '8px'
            }}
          />
        </label>
      </div>

      <table className="stock-table">
        <thead>
          <tr className="row-warning-header">
            <th>Product</th>
            <th>Current balance</th>
            <th>Deadline</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {expiringBatches.map(b => (
            <tr key={b.id} className="row-warning-body">
              <td>
                <b>{b.productName}</b>
              </td>

              <td>
                {b.remainingQuantity || b.quantityReceived}
              </td>

              <td className="text-danger">
                {b.expirationDate || b.expiryDate
                  ? new Date(
                      b.expirationDate || b.expiryDate
                    ).toLocaleDateString()
                  : 'Завершується'}
              </td>

              <td>
                <button
                  className="btn btn-danger"
                  disabled={(b.remainingQuantity || 0) <= 0}
                  onClick={() => onReduceClick(b)}
                >
                  Write off
                </button>
              </td>
            </tr>
          ))}

          {expiringBatches.length === 0 && (
            <tr>
              <td
                colSpan="4"
                className="text-muted"
                style={{
                  padding: '15px',
                  textAlign: 'center'
                }}
              >
                No critical batches were found for the specified period.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default ExpiringTab;