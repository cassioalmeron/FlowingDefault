import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { api } from '../../api';
import '../../styles/Common.css';
import './Styles.css';
import LabelModal from './LabelModal';
import type { Label } from './types';

const emptyLabel: Label = { id: 0, name: '' };

const Labels: React.FC = () => {
  const [labels, setLabels] = useState<Label[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalLabel, setModalLabel] = useState<Label>(emptyLabel);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [errors, setErrors] = useState<{ name?: string }>({});

  useEffect(() => {
    const fetchLabels = async () => {
      try {
        const data = await api.labels.list();
        setLabels(data);
      } catch (error: unknown) {
        if (typeof error === 'object' && error !== null && 'response' in error) {
          const err = error as { response?: { data?: { message?: string } } };
          toast.error(err.response?.data?.message || 'Failed to fetch labels');
        } else {
          toast.error('Failed to fetch labels');
        }
      } finally {
        setLoading(false);
      }
    };
    fetchLabels();
  }, []);

  const openNewModal = () => {
    setModalLabel({ ...emptyLabel });
    setEditingId(null);
    setErrors({});
    setModalOpen(true);
  };

  const openEditModal = (label: Label) => {
    setModalLabel({ ...label });
    setEditingId(label.id);
    setErrors({});
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setModalLabel(emptyLabel);
    setEditingId(null);
    setErrors({});
  };

  const validate = () => {
    const newErrors: { name?: string } = {};
    if (!modalLabel.name || modalLabel.name.trim().length < 2) {
      newErrors.name = 'Name is required and must be at least 2 characters';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editingId) {
        const updated = await api.labels.update(editingId, { name: modalLabel.name });
        setLabels(labels.map(l => (l.id === editingId ? updated : l)));
        toast.success('Label updated!');
      } else {
        const created = await api.labels.create({ name: modalLabel.name });
        setLabels([...labels, created]);
        toast.success('Label created!');
      }
      closeModal();
    } catch (error: unknown) {
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const err = error as { response?: { data?: { message?: string } } };
        toast.error(err.response?.data?.message || 'Failed to save label');
      } else {
        toast.error('Failed to save label');
      }
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = (id: number) => {
    setDeleteId(id);
    setConfirmOpen(true);
  };

  const confirmDelete = async () => {
    if (deleteId == null) return;
    setDeleteLoading(true);
    try {
      await api.labels.delete(deleteId);
      setLabels(labels.filter(label => label.id !== deleteId));
      toast.success('Label deleted successfully!');
    } catch (error: unknown) {
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const err = error as { response?: { data?: { message?: string } } };
        toast.error(err.response?.data?.message || 'Failed to delete label');
      } else {
        toast.error('Failed to delete label');
      }
    } finally {
      setDeleteLoading(false);
      setConfirmOpen(false);
      setDeleteId(null);
    }
  };

  if (loading) return <div>Loading labels...</div>;

  return (
    <div>
      <div className="labels-header">
        <button className="labels-new-btn" onClick={openNewModal}>New</button>
        <h2 className="labels-title">Labels</h2>
        <div className="labels-header-actions"></div>
      </div>
      <table className="labels-table">
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th className="labels-actions-header">Actions</th>
          </tr>
        </thead>
        <tbody>
          {labels.map(label => (
            <tr key={label.id}>
              <td>{label.id}</td>
              <td>{label.name}</td>
              <td className="labels-actions-cell">
                <button className="labels-action-btn edit" onClick={() => openEditModal(label)}>Edit</button>
                <button className="labels-action-btn delete" onClick={() => handleDelete(label.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Modal for create/edit */}
      <LabelModal
        open={modalOpen}
        label={modalLabel}
        loading={saving}
        editingId={editingId}
        onChange={setModalLabel}
        onSave={handleSave}
        onClose={closeModal}
        errors={errors}
      />

      {/* Confirm Delete Modal */}
      {confirmOpen && (
        <div className="common-modal-backdrop">
          <div className="common-modal" style={{ minWidth: 320, maxWidth: 400 }}>
            <div style={{ marginBottom: 24 }}>Are you sure you want to delete this label?</div>
            <div className="common-modal-actions">
              <button className="common-modal-cancel" onClick={() => setConfirmOpen(false)} disabled={deleteLoading}>Cancel</button>
              <button className="common-modal-save" onClick={confirmDelete} disabled={deleteLoading}>
                {deleteLoading ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Labels;