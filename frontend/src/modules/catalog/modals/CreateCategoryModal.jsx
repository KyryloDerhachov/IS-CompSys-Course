import React from 'react';

const CreateCategoryModal = ({
  isOpen,
  onClose,
  form,
  setForm,
  categories,
  onSubmit
}) => {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h3>Creating a new category</h3>
        
        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <input
            type="text"
            placeholder="Category code *"
            required
            value={form.code}
            onChange={e => setForm({ ...form, code: e.target.value })}
          />
          
          <input
            type="text"
            placeholder="Category Name *"
            required
            value={form.name}
            onChange={e => setForm({ ...form, name: e.target.value })}
          />

          <select
            value={form.parentId}
            onChange={e => setForm({ ...form, parentId: e.target.value })}
          >
            <option value="">-- Root category --</option>
            {categories.filter(c => c.isActive).map(cat => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>

          <textarea
            placeholder="Category Description"
            value={form.description}
            onChange={e => setForm({ ...form, description: e.target.value })}
            style={{ height: '100px', resize: 'vertical' }}
          />

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '10px' }}>
            <button type="button" className="btn" onClick={onClose}>
              Cancel
            </button>
            <button type="submit" className="btn btn-success">
              Create a category
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateCategoryModal;