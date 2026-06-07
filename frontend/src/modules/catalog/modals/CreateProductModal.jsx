import React from 'react';

const CreateProductModal = ({
  isOpen, onClose, form, setForm, classes, categories,
  attributes, setAttributes, newKey, setNewKey, newValue, setNewValue,
  onSubmit, onAddAttribute, onRemoveAttribute
}) => {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h3>Creating a new product</h3>
        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <input type="text" placeholder="Product Name *" required value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
          <input type="text" placeholder="SKU *" required value={form.sku} onChange={e => setForm({ ...form, sku: e.target.value })} />
          <input type="text" placeholder="Barcode" value={form.barcode} onChange={e => setForm({ ...form, barcode: e.target.value })} />

          <select required value={form.classId} onChange={e => setForm({ ...form, classId: e.target.value })}>
            <option value="">-- Select a class --</option>
            {classes.filter(c => c.isActive).map(cl => (
              <option key={cl.id} value={cl.id}>{cl.name}</option>
            ))}
          </select>

          <select required value={form.categoryId} onChange={e => setForm({ ...form, categoryId: e.target.value })}>
            <option value="">-- Select a category --</option>
            {categories.filter(c => c.isActive).map(cat => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>

          <input type="text" placeholder="Unit" value={form.unit} onChange={e => setForm({ ...form, unit: e.target.value })} />
          <input type="number" placeholder="Shelf life (days)" value={form.defaultShelfLifeDays} onChange={e => setForm({ ...form, defaultShelfLifeDays: e.target.value })} />
          <input type="number" step="0.01" placeholder="Purchase price" value={form.defaultPurchasePrice} onChange={e => setForm({ ...form, defaultPurchasePrice: e.target.value })} />
          <input type="number" step="0.01" placeholder="Retail price *" required value={form.defaultSalePrice} onChange={e => setForm({ ...form, defaultSalePrice: e.target.value })} />

          <div style={{ background: 'var(--bg-secondary)', padding: '12px', borderRadius: '10px' }}>
            <strong>Product Specifications</strong>
            <div style={{ display: 'flex', gap: '8px', margin: '10px 0' }}>
              <input placeholder="Title" value={newKey} onChange={e => setNewKey(e.target.value)} style={{ flex: 1 }} />
              <input placeholder="Value" value={newValue} onChange={e => setNewValue(e.target.value)} style={{ flex: 1 }} />
              <button type="button" className="btn btn-primary" onClick={onAddAttribute}>Add</button>
            </div>

            {attributes.map((attr, i) => (
              <div key={i} style={{ display: 'flex', justifyContent: 'space-between', padding: '6px 10px', background: 'var(--bg-primary)', marginBottom: '6px', borderRadius: '6px' }}>
                <span><b>{attr.key}:</b> {attr.value}</span>
                <span style={{ color: 'var(--danger)', cursor: 'pointer' }} onClick={() => onRemoveAttribute(i)}>✕</span>
              </div>
            ))}
          </div>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '10px' }}>
            <button type="button" className="btn" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-success">Save item</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateProductModal;