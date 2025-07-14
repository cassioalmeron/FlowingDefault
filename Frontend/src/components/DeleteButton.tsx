import React, { useState } from 'react';

interface DeleteButtonProps {
  onConfirm: () => void;
  loading?: boolean;
  children?: React.ReactNode;
  confirmMessage?: React.ReactNode;
}

const DeleteButton: React.FC<DeleteButtonProps> = ({ onConfirm, loading = false, children = 'Delete', confirmMessage = 'Are you sure you want to delete this item?' }) => {
  const [open, setOpen] = useState(false);

  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault();
    setOpen(true);
  };

  const handleCancel = () => {
    setOpen(false);
  };

  const handleConfirm = () => {
    setOpen(false);
    onConfirm();
  };

  return (
    <>
      <button className="action-btn delete" onClick={handleClick} disabled={loading}>
        {children}
      </button>
      {open && (
        <div className="common-modal-backdrop">
          <div className="common-modal" style={{ minWidth: 320, maxWidth: 400 }}>
            <div style={{ marginBottom: 24 }}>{confirmMessage}</div>
            <div className="common-modal-actions">
              <button className="common-modal-cancel" onClick={handleCancel} disabled={loading}>Cancel</button>
              <button className="common-modal-save" onClick={handleConfirm} disabled={loading}>
                {loading ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default DeleteButton; 