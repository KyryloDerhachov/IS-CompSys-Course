import React from 'react';

const NewSupplyTab = ({ 
  supplyForm, 
  setSupplyForm,
  newItem, 
  setNewItem, 
  products, 
  onCSVUpload, 
  onAddItem, 
  onRemoveItem, 
  onSubmit 
}) => {
  return (
    <form onSubmit={onSubmit}>
      <div className="supply-header-actions">
        <h3 style={{ margin: 0 }}>Recording the receipt of goods into the warehouse</h3>
        
        <div className="csv-import-box">
          <label>Import a shipping from a CSV file</label>
          <input type="file" accept=".csv" onChange={onCSVUpload} />
        </div>
      </div>
      
      <div className="form-row">
        <div className="form-group">
          <label>Supplier:</label>
          <input 
            type="text" 
            value={supplyForm.supplierName} 
            onChange={e => setSupplyForm({ ...supplyForm, supplierName: e.target.value })} 
            required 
            className="form-control"
            placeholder="" 
          />
        </div>
        <div className="form-group">
          <label>Supply date:</label>
          <input 
            type="date" 
            value={supplyForm.supplyDate} 
            onChange={e => setSupplyForm({ ...supplyForm, supplyDate: e.target.value })} 
            required 
            className="form-control" 
          />
        </div>
      </div>

      <fieldset className="fieldset-manual">
        <legend>Add an item manually</legend>
        <div className="inline-form">
          <div style={{ flex: 2 }}>
            <label>Select a product:</label>
            <select 
              value={newItem.productId} 
              onChange={e => setNewItem({ ...newItem, productId: e.target.value })} 
              className="form-control"
            >
              <option value="">Select SKU</option>
              {products.map(p => <option key={p.id} value={p.id}>{p.name} ({p.sku})</option>)}
            </select>
          </div>
          <div style={{ flex: 1 }}>
            <label>Quantity:</label>
            <input 
              type="number" 
              step="1" 
              min="1"
              value={newItem.quantity} 
              onChange={e => setNewItem({ ...newItem, quantity: e.target.value })} 
              className="form-control" 
            />
          </div>
          <div style={{ flex: 1 }}>
            <label>Purchase price (₴):</label>
            <input 
              type="number" 
              step="0.01"
              min="0.01"
              value={newItem.purchasePrice} 
              onChange={e => setNewItem({ ...newItem, purchasePrice: e.target.value })} 
              className="form-control" 
            />
          </div>
          <div style={{ flex: 1 }}>
            <label>Shelf life (days):</label>
            <input 
              type="number" 
              value={newItem.shelfLifeDays} 
              onChange={e => setNewItem({ ...newItem, shelfLifeDays: e.target.value })} 
              className="form-control" 
            />
          </div>
          <button type="button" onClick={onAddItem} className="btn btn-primary">
            Add
          </button>
        </div>
      </fieldset>

      <h4>Delivery Note Specifications:</h4>
      <table className="stock-table">
        <thead>
          <tr style={{ backgroundColor: '#f8f9fa' }}>
            <th>Product</th>
            <th>Quantity</th>
            <th>Purchase price</th>
            <th>Expiration date</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {supplyForm.items.map((item, idx) => (
            <tr key={idx}>
              <td>{item.productName}</td>
              <td>{item.quantity} pcs</td>
              <td>{item.purchasePrice} ₴</td>
              <td>{item.shelfLifeDays} days</td>
              <td>
                <button type="button" onClick={() => onRemoveItem(idx)} className="btn-link">
                  Delete
                </button>
              </td>
            </tr>
          ))}
          {supplyForm.items.length === 0 && (
            <tr>
              <td colSpan="5" className="text-muted" style={{ padding: '15px', textAlign: 'center' }}>
                The shipping label is empty. Add items above or upload a CSV file.
              </td>
            </tr>
          )}
        </tbody>
      </table>

      <button type="submit" className="btn btn-success" style={{ marginTop: '20px', width: '100%', padding: '12px', fontSize: '16px' }}>
        Register and process the shipment
      </button>
    </form>
  );
};

export default NewSupplyTab;