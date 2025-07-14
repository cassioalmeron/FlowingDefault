import React, { useEffect, useState } from 'react';
import { api } from '../../api';
import { toast } from 'react-toastify';
import '../../styles/Common.css';
import './Styles.css';
import ProjectModal from './ProjectModal';
import ConfirmDialog from '../../components/ConfirmDialog';
import Grid from '../../components/Grid';
import type { GridColumn } from '../../components/Grid';
import type { Project } from './types';

const emptyProject: Project = { id: 0, name: '' };

const columns: GridColumn<Project>[] = [
  { header: 'Id', accessor: 'id' },
  { header: 'Name', accessor: 'name' },
];

const Projects: React.FC = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalProject, setModalProject] = useState<Project>(emptyProject);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<number | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  useEffect(() => {
    const fetchProjects = async () => {
      try {
        const data = await api.projects.list();
        setProjects(data);
      } catch {
        toast.error('Failed to fetch projects');
      } finally {
        setLoading(false);
      }
    };
    fetchProjects();
  }, []);

  const openNewModal = () => {
    setModalProject({ ...emptyProject });
    setEditingId(null);
    setModalOpen(true);
  };

  const openEditModal = (project: Project) => {
    setModalProject({ ...project });
    setEditingId(project.id);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setModalProject(emptyProject);
    setEditingId(null);
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      if (editingId) {
        const updated = await api.projects.update(editingId, { name: modalProject.name });
        setProjects(projects.map(p => (p.id === editingId ? updated : p)));
        toast.success('Project updated!');
      } else {
        const created = await api.projects.create({ name: modalProject.name });
        setProjects([...projects, created]);
        toast.success('Project created!');
      }
      closeModal();
    } catch {
      toast.error('Failed to save project');
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
      await api.projects.delete(deleteId);
      setProjects(projects.filter(project => project.id !== deleteId));
      toast.success('Project deleted successfully!');
    } catch {
      toast.error('Failed to delete project');
    } finally {
      setDeleteLoading(false);
      setConfirmOpen(false);
      setDeleteId(null);
    }
  };

  if (loading) return <div>Loading projects...</div>;

  return (
    <div>
      <div className="page-header">
        <button className="page-new-btn" onClick={openNewModal}>New</button>
        <h2 className="page-title">Projects</h2>
        <div className="page-header-actions"></div>
      </div>
      <Grid
        columns={columns}
        data={projects}
        renderActions={project => (
          <>
            <button className="action-btn edit" onClick={() => openEditModal(project)}>Edit</button>
            <button className="action-btn delete" onClick={() => handleDelete(project.id)}>Delete</button>
          </>
        )}
      />
      <ProjectModal
        open={modalOpen}
        project={modalProject}
        loading={saving}
        editingId={editingId}
        onChange={setModalProject}
        onSave={handleSave}
        onClose={closeModal}
      />
      <ConfirmDialog
        open={confirmOpen}
        message={<p>Are you sure you want to delete this project?</p>}
        onConfirm={confirmDelete}
        onCancel={() => { setConfirmOpen(false); setDeleteId(null); }}
        confirmLabel="Delete"
        cancelLabel="Cancel"
        loading={deleteLoading}
      />
    </div>
  );
};

export default Projects;