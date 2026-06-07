import React from 'react';

const CreateClassModal = ({
  isOpen,
  onClose,
  form,
  setForm,
  onSubmit
}) => {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal" style={{ width: '420px' }}>
        <h3>Creating a new product category</h3>
        
        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <input
            type="text"
            placeholder="Class code *"
            required
            value={form.code}
            onChange={e => setForm({ ...form, code: e.target.value })}
          />
          
          <input
            type="text"
            placeholder="Class name *"
            required
            value={form.name}
            onChange={e => setForm({ ...form, name: e.target.value })}
          />

          <textarea
            placeholder="Class description"
            value={form.description}
            onChange={e => setForm({ ...form, description: e.target.value })}
            style={{ height: '100px', resize: 'vertical' }}
          />

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '10px' }}>
            <button type="button" className="btn" onClick={onClose}>
              Cancel
            </button>
            <button type="submit" className="btn btn-success">
              Create a class
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateClassModal;