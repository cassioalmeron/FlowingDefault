import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { api } from '../../api';
import '../../styles/Common.css';
import './Styles.css';
import UserModal from './UserModal';
import ConfirmDialog from '../../components/ConfirmDialog';
import Grid from '../../components/Grid';
import type { GridColumn } from '../../components/Grid';
import type { User } from './types';

const emptyUser: User = { id: 0, name: '', username: '' };

const columns: GridColumn<User>[] = [
  { header: 'Id', accessor: 'id' },
  { header: 'Name', accessor: 'name' },
  { header: 'Username', accessor: 'username' },
];

const Index = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalUser, setModalUser] = useState<User>(emptyUser);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await api.users.list();
        setUsers(data);
      } catch {
        toast.error('Failed to fetch users');
      } finally {
        setLoading(false);
      }
    };
    fetchUsers();
  }, []);

  const openNewModal = () => {
    setModalUser({ ...emptyUser });
    setEditingId(null);
    setModalOpen(true);
  };

  const openEditModal = (user: User) => {
    setModalUser({ ...user });
    setEditingId(user.id);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setModalUser(emptyUser);
    setEditingId(null);
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      if (editingId) {
        const updated = await api.users.update(editingId, { name: modalUser.name, username: modalUser.username });
        setUsers(users.map(u => (u.id === editingId ? updated : u)));
        toast.success('User updated!');
      } else {
        const created = await api.users.create({ name: modalUser.name, username: modalUser.username });
        setUsers([...users, created]);
        toast.success('User created!');
      }
      closeModal();
    } catch {
      toast.error('Failed to save user');
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
      await api.users.delete(deleteId);
      setUsers(users.filter(user => user.id !== deleteId));
      toast.success('User deleted successfully!');
    } catch {
      toast.error('Failed to delete user');
    } finally {
      setDeleteLoading(false);
      setConfirmOpen(false);
      setDeleteId(null);
    }
  };

  if (loading) return <div>Loading users...</div>;

  return (
    <div>
      <div className="page-header">
        <button className="page-new-btn" onClick={openNewModal}>New</button>
        <h2 className="page-title">Users</h2>
        <div className="page-header-actions"></div>
      </div>
      <Grid
        columns={columns}
        data={users}
        renderActions={user => (
          <>
            <button className="action-btn edit" onClick={() => openEditModal(user)}>Edit</button>
            <button className="action-btn delete" onClick={() => handleDelete(user.id)}>Delete</button>
          </>
        )}
      />
      <UserModal
        open={modalOpen}
        user={modalUser}
        loading={saving}
        editingId={editingId}
        onChange={setModalUser}
        onSave={handleSave}
        onClose={closeModal}
      />
      <ConfirmDialog
        open={confirmOpen}
        message={<p>Are you sure you want to delete this user?</p>}
        onConfirm={confirmDelete}
        onCancel={() => { setConfirmOpen(false); setDeleteId(null); }}
        confirmLabel="Delete"
        cancelLabel="Cancel"
        loading={deleteLoading}
      />
    </div>
  );
};

export default Index;