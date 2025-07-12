import React from 'react';
import '../styles/Common.css';

interface ConfirmDialogProps {
  open: boolean;
  message: React.ReactNode;
  onConfirm: () => void;
  onCancel: () => void;
  confirmLabel?: string;
  cancelLabel?: string;
  loading?: boolean;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  open,
  message,
  onConfirm,
  onCancel,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  loading = false,
}) => {
  if (!open) return null;

  return (
    <div className="common-modal-backdrop">
      <div className="common-modal" style={{ minWidth: 320, maxWidth: 400 }}>
        <div style={{ marginBottom: 24 }}>{message}</div>
        <div className="common-modal-actions">
          <button
            className="common-modal-cancel"
            onClick={onCancel}
            disabled={loading}
          >
            {cancelLabel}
          </button>
          <button
            className="common-modal-save"
            onClick={onConfirm}
            disabled={loading}
          >
            {loading ? 'Please wait...' : confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmDialog; 