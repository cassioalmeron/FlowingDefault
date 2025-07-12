import React, { useState, useEffect } from 'react';
import * as yup from 'yup';
import { toast } from 'react-toastify';
import { api } from '../../api';
import '../../styles/Common.css';

const profileSchema = yup.object({
  name: yup.string().required('Name is required'),
  username: yup.string().required('Username is required'),
});

const passwordSchema = yup.object({
  newPassword: yup.string().required('New password is required'),
  confirmPassword: yup
    .string()
    .required('Confirm password is required')
    .oneOf([yup.ref('newPassword')], 'Passwords must match'),
});

const Profile = () => {
  const [name, setName] = useState('');
  const [username, setUsername] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [profileErrors, setProfileErrors] = useState<{ name?: string; username?: string }>({});
  const [passwordErrors, setPasswordErrors] = useState<{ newPassword?: string; confirmPassword?: string }>({});
  const [loading, setLoading] = useState(false);
  const [passwordLoading, setPasswordLoading] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        const data = await api.profile.getProfile();
        setName(data.name);
        setUsername(data.username);
      } catch (error: unknown) {
        if (typeof error === 'object' && error !== null && 'response' in error) {
          const err = error as { response?: { data?: { message?: string } } };
          toast.error(err.response?.data?.message || 'Failed to load profile');
        } else {
          toast.error('Failed to load profile');
        }
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const validateProfile = async () => {
    try {
      await profileSchema.validate({ name, username }, { abortEarly: false });
      setProfileErrors({});
      return true;
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        const newErrors: { name?: string; username?: string } = {};
        err.inner.forEach((e) => {
          if (e.path) newErrors[e.path as keyof typeof newErrors] = e.message;
        });
        setProfileErrors(newErrors);
      }
      return false;
    }
  };

  const validatePassword = async () => {
    try {
      await passwordSchema.validate({ newPassword, confirmPassword }, { abortEarly: false });
      setPasswordErrors({});
      return true;
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        const newErrors: { newPassword?: string; confirmPassword?: string } = {};
        err.inner.forEach((e) => {
          if (e.path) newErrors[e.path as keyof typeof newErrors] = e.message;
        });
        setPasswordErrors(newErrors);
      }
      return false;
    }
  };

  const handleSaveProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    if (await validateProfile()) {
      try {
        setLoading(true);
        await api.profile.updateProfile({ name, username });
        toast.success('Profile updated successfully!');
      } catch (error: unknown) {
        if (typeof error === 'object' && error !== null && 'response' in error) {
          const err = error as { response?: { data?: { message?: string } } };
          toast.error(err.response?.data?.message || 'Failed to update profile');
        } else {
          toast.error('Failed to update profile');
        }
      } finally {
        setLoading(false);
      }
    }
  };

  const handleOpenPasswordModal = () => {
    setModalOpen(true);
    setNewPassword('');
    setConfirmPassword('');
    setPasswordErrors({});
  };

  const handleClosePasswordModal = () => {
    setModalOpen(false);
    setNewPassword('');
    setConfirmPassword('');
    setPasswordErrors({});
  };

  const handleSavePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (await validatePassword()) {
      try {
        setPasswordLoading(true);
        await api.profile.changePassword(newPassword);
        toast.success('Password changed successfully!');
        setModalOpen(false);
      } catch (error: unknown) {
        if (typeof error === 'object' && error !== null && 'response' in error) {
          const err = error as { response?: { data?: { message?: string } } };
          toast.error(err.response?.data?.message || 'Failed to change password');
        } else {
          toast.error('Failed to change password');
        }
      } finally {
        setPasswordLoading(false);
      }
    }
  };

  return (
    <div style={{ maxWidth: 400, margin: '40px auto' }}>
      <h2 style={{ textAlign: 'center', marginBottom: 32 }}>Profile</h2>
      <form className="common-modal-form" onSubmit={handleSaveProfile}>
        <label>
          Name:
          <input
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            disabled={loading}
          />
          {profileErrors.name && <div className="login-error">{profileErrors.name}</div>}
        </label>
        <label>
          Username:
          <input
            type="text"
            value={username}
            onChange={e => setUsername(e.target.value)}
            disabled={loading}
          />
          {profileErrors.username && <div className="login-error">{profileErrors.username}</div>}
        </label>
        <button type="button" className="common-modal-save" style={{ marginBottom: 16 }} onClick={handleOpenPasswordModal} disabled={loading}>
          Change Password
        </button>
        <button type="submit" className="common-modal-save" disabled={loading}>
          {loading ? 'Saving...' : 'Save'}
        </button>
      </form>

      {/* Change Password Modal */}
      {modalOpen && (
        <div className="common-modal-backdrop">
          <div className="common-modal" style={{ minWidth: 320, maxWidth: 400 }}>
            <h3>Change Password</h3>
            <form className="common-modal-form" onSubmit={handleSavePassword}>
              <label>
                New password:
                <input
                  type="password"
                  value={newPassword}
                  onChange={e => setNewPassword(e.target.value)}
                  disabled={passwordLoading}
                />
                {passwordErrors.newPassword && <div className="login-error">{passwordErrors.newPassword}</div>}
              </label>
              <label>
                Confirm password:
                <input
                  type="password"
                  value={confirmPassword}
                  onChange={e => setConfirmPassword(e.target.value)}
                  disabled={passwordLoading}
                />
                {passwordErrors.confirmPassword && <div className="login-error">{passwordErrors.confirmPassword}</div>}
              </label>
              <div className="common-modal-actions">
                <button type="button" className="common-modal-cancel" onClick={handleClosePasswordModal} disabled={passwordLoading}>
                  Cancel
                </button>
                <button type="submit" className="common-modal-save" disabled={passwordLoading}>
                  {passwordLoading ? 'Saving...' : 'Save'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Profile;