import React from 'react';

interface EditButtonProps {
  onClick: () => void;
  loading?: boolean;
  children?: React.ReactNode;
}

const EditButton: React.FC<EditButtonProps> = ({ onClick, loading = false }) => {
  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault();
    onClick();
  };

  return (
    <button 
      className="action-btn edit" 
      onClick={handleClick} 
      disabled={loading}
      title="Edit"
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
        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
      </svg>
    </button>
  );
};

export default EditButton; 