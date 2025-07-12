import React from 'react';
import './LabelModal.css';
import type { Label } from './types';

interface LabelModalProps {
  open: boolean;
  label: Label;
  loading: boolean;
  editingId: number | null;
  onChange: (label: Label) => void;
  onSave: () => void;
  onClose: () => void;
  errors?: { name?: string };
}

const LabelModal: React.FC<LabelModalProps> = ({ open, label, loading, editingId, onChange, onSave, onClose, errors }) => {
  if (!open) return null;

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !loading) {
      e.preventDefault();
      onSave();
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!loading) {
      onSave();
    }
  };

  return (
    <div className="label-modal-backdrop">
      <div className="label-modal">
        <h3>{editingId ? 'Edit Label' : 'New Label'}</h3>
        <form className="label-modal-form" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
          <label>
            Name:
            <input
              type="text"
              value={label.name}
              onChange={e => onChange({ ...label, name: e.target.value })}
              disabled={loading}
            />
            {errors?.name && <div className="label-modal-error">{errors.name}</div>}
          </label>
        </form>
        <div className="label-modal-actions">
          <button onClick={onClose} className="label-modal-cancel" disabled={loading}>Cancel</button>
          <button onClick={onSave} className="label-modal-save" disabled={loading}>
            {loading ? 'Saving...' : 'Save'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default LabelModal; 