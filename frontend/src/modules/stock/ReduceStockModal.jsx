import React from 'react';

const ReduceStockModal = ({ modalData, onQuantityChange, onClose, onSubmit }) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h3>Write-off / adjustment of balances</h3>
        <p>Batch: <b>{modalData.batch?.productName}</b></p>
        <p className="text-muted" style={{ fontSize: '13px' }}>
          In stock: {modalData.batch?.remainingQuantity} pcs. (Version: {modalData.version})
        </p>
        
        <form onSubmit={onSubmit}>
          <div style={{ marginBottom: '15px' }}>
            <label style={{ display: 'block', marginBottom: '5px' }}>Quantity to be written off:</label>
            <input 
              type="number" 
              step="1"
              min="0"
              max={modalData.batch?.remainingQuantity}
              value={modalData.quantity} 
              onChange={e => onQuantityChange(e.target.value)} 
              required 
              className="form-control" 
            />
          </div>
          <div className="modal-actions">
            <button type="button" onClick={onClose} className="btn">
              Cancel
            </button>
            <button type="submit" className="btn btn-danger">
              Confirm the write-off
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ReduceStockModal;