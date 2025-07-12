import React, { useState } from 'react';
import * as yup from 'yup';
import '../../styles/Common.css';
import './ProjectModalStyles.css';
import type { Project } from './types';

interface ProjectModalProps {
  open: boolean;
  project: Project;
  loading: boolean;
  editingId: number | null;
  onChange: (project: Project) => void;
  onSave: () => void;
  onClose: () => void;
}

const projectSchema = yup.object({
  name: yup.string().required('Name is required').min(2, 'Name must be at least 2 characters'),
});

const ProjectModal: React.FC<ProjectModalProps> = ({ open, project, loading, editingId, onChange, onSave, onClose }) => {
  const [errors, setErrors] = useState<{ name?: string }>({});

  if (!open) return null;

  const validate = async () => {
    try {
      await projectSchema.validate(project, { abortEarly: false });
      setErrors({});
      return true;
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        const newErrors: { name?: string } = {};
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
        <h3>{editingId ? 'Edit Project' : 'New Project'}</h3>
        <form className="common-modal-form" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
          <label>
            Name:
            <input
              type="text"
              value={project.name}
              onChange={e => onChange({ ...project, name: e.target.value })}
              disabled={loading}
            />
            {errors.name && <div className="login-error">{errors.name}</div>}
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

export default ProjectModal; 