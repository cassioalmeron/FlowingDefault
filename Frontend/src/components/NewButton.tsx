import React from 'react';

interface NewButtonProps {
  onClick: () => void;
  loading?: boolean;
  children?: React.ReactNode;
}

const NewButton: React.FC<NewButtonProps> = ({ onClick, loading = false, children = 'New' }) => {
  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault();
    onClick();
  };

  return (
    <button 
      className="page-new-btn" 
      onClick={handleClick} 
      disabled={loading}
      title="Create new"
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
        <line x1="12" y1="5" x2="12" y2="19"></line>
        <line x1="5" y1="12" x2="19" y2="12"></line>
      </svg>
      {children}
    </button>
  );
};

export default NewButton; 