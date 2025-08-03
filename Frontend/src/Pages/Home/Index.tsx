import { session } from '../../session';

const Index = () => {
  const user = session.getUser();
  
  return (
    <div>
      <div>Welcome to {import.meta.env.VITE_APP_NAME} <strong>Cassio Almeron</strong></div>
      <div style={{ marginTop: '10px', fontSize: '12px', color: '#666' }}>
        Token expires: {user?.expires ? new Date(user.expires).toLocaleString() : 'Unknown'}
      </div>
    </div>
  )
}

export default Index