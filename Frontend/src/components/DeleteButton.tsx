import React, { useState } from 'react';

interface DeleteButtonProps {
  onConfirm: () => void;
  loading?: boolean;
  children?: React.ReactNode;
  confirmMessage?: React.ReactNode;
}

const DeleteButton: React.FC<DeleteButtonProps> = ({ onConfirm, loading = false, confirmMessage = 'Are you sure you want to delete this item?' }) => {
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
      <button 
        className="action-btn delete" 
        onClick={handleClick} 
        disabled={loading}
        title="Delete"
      >
        <svg 
          width="16" 
          height="16" 
          viewBox="0 0 24 24" 
          fill="none" 
          stroke="currentColor" 
          strokeWidth="2" 
          strokeLinecap="round" 
          strokeLinejoin="round"
        >
          <path d="M3 6h18"></path>
          <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path>
          <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path>
        </svg>
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