import React from 'react';

const EditClassModal = ({
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
        <h3>Chang class description</h3>
        
        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
          <textarea
            placeholder="Class Description"
            value={form.description}
            onChange={e => setForm({ description: e.target.value })}
            style={{ height: '140px', resize: 'vertical' }}
            required
          />

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '10px' }}>
            <button type="button" className="btn" onClick={onClose}>
              Cancel
            </button>
            <button type="submit" className="btn btn-primary">
              Save changes
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditClassModal;