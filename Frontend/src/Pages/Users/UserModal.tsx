import React, { useState } from 'react';
import * as yup from 'yup';
import '../../styles/Common.css';
import './UserModalStyles.css';
import type { User } from './types';

interface UserModalProps {
  open: boolean;
  user: User;
  loading: boolean;
  editingId: number | null;
  onChange: (user: User) => void;
  onSave: () => void;
  onClose: () => void;
}

const userSchema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Name must be at least 2 characters'),
  username: yup.string().required('Username is required').min(3, 'Username must be at least 3 characters'),
});

const UserModal: React.FC<UserModalProps> = ({ open, user, loading, editingId, onChange, onSave, onClose }) => {
  const [errors, setErrors] = useState<{ name?: string; username?: string }>({});

  if (!open) return null;

  const validate = async () => {
    try {
      await userSchema.validate(user, { abortEarly: false });
      setErrors({});
      return true;
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        const newErrors: { name?: string; username?: string } = {};
        err.inner.forEach((e) => {
          if (e.path) newErrors[e.path as keyof typeof newErrors] = e.message;
        });
        setErrors(newErrors);
      }
      return false;
    }
  };

  const handleKeyDown = async (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !loading) {
      e.preventDefault();
      if (await validate()) onSave();
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!loading && (await validate())) {
      onSave();
    }
  };

  return (
    <div className="common-modal-backdrop">
      <div className="common-modal">
        <h3>{editingId ? 'Edit User' : 'New User'}</h3>
        <form className="common-modal-form" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
          <label>
            Name:
            <input
              type="text"
              value={user.name}
              onChange={e => onChange({ ...user, name: e.target.value })}
              disabled={loading}
            />
            {errors.name && <div className="login-error">{errors.name}</div>}
          </label>
          <label>
            Username:
            <input
              type="text"
              value={user.username}
              onChange={e => onChange({ ...user, username: e.target.value })}
              disabled={loading}
            />
            {errors.username && <div className="login-error">{errors.username}</div>}
          </label>
        </form>
        <div className="common-modal-actions">
          <button onClick={onClose} className="common-modal-cancel" disabled={loading}>Cancel</button>
          <button onClick={async (e) => { e.preventDefault(); if (await validate()) onSave(); }} className="common-modal-save" disabled={loading}>
            {loading ? 'Saving...' : 'Save'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default UserModal; 